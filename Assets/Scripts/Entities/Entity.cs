using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Vector3Int gridPosition;
    public bool turn = false;
    public int rotation = 0; // most likely unused besides player
    protected Vector3 positionOffset; // offset from grid position for visuals
    [SerializeField] protected bool canBeAttacked = true;
    [SerializeField] public int maxHp = 1;
    public int hp { get; protected set; }
    [SerializeField] protected List<Damage> weaknesses = new();
    [SerializeField] protected Damage baseAttack = new(Damage.Type.PHYSICAL, 1);

    // REPLACE WITH ACTION QUEUE
    protected Queue<int> rotationQueue;
    protected Queue<Movement> movementQueue;
    protected float MOVE_SPEED = 10.0f;

    public AudioSource audioSource;

    public Action<int, int> onTakeDamage;

    public void Start()
    {
        rotationQueue = new();
        movementQueue = new();
        gridPosition = Vector3Int.FloorToInt(transform.position);
        positionOffset = transform.position - gridPosition;
        hp = maxHp;
        StartCoroutine(RunMovementQueue());
        TryGetComponent<AudioSource>(out audioSource);
    }

    public virtual void TurnUpdate()
    {
        EndTurn();
    }

    public virtual bool Move(Vector3Int newPosition, bool instant = false, bool teleport = false)
    {
        if (!teleport)
        {
            RaycastHit hit;
            if (Physics.Raycast(gridPosition + new Vector3(0.5f, 0.0f, 0.5f), newPosition - gridPosition, out hit, 0.5f))
            {
                if (hit.transform.TryGetComponent<Entity>(out Entity entity))
                {
                    Attack(baseAttack, entity);
                }
                movementQueue.Enqueue(new Movement(newPosition, true));
                return false;
            }
            else
            {
                if (Physics.Raycast(newPosition + new Vector3(0.5f, 0.0f, 0.5f), Vector3.down, 1f))
                {
                    gridPosition = newPosition;
                    movementQueue.Enqueue(new Movement(newPosition, false));
                    return true;
                }
                else
                {
                    movementQueue.Enqueue(new Movement(newPosition, true));
                    return false;
                }
            }
        }
        else
        {
            gridPosition = newPosition;
            if (instant)
            {
                transform.position = newPosition + positionOffset;
            }
            else
            {
                movementQueue.Enqueue(new Movement(newPosition, false));
            }
        }
        return true;
    }

    // might go unused outside of player
    public virtual void Rotate(ref int newRotation)
    {
        rotation = (newRotation + 4) % 4;

        rotationQueue.Enqueue(rotation);
    }

    public virtual IEnumerator RunTurn()
    {
        StartTurn();

        while (turn)
        {
            TurnUpdate();

            //if (name == "Player") yield return new WaitForEndOfFrame(); // dont wait frame if action taken
        }

        yield break;
    }

    public virtual void StartTurn()
    {
        turn = true;
    }

    public virtual void EndTurn()
    {
        turn = false;
    }

    public virtual void Attack(Damage damage, Entity target)
    {
        if (target.canBeAttacked)
        {
            target.TakeDamage(damage);
        }
    }

    public virtual void TakeDamage(Damage damage)
    {
        int value = damage.value;

        for(int i = 0; i < weaknesses.Count; i++)
        {
            if (weaknesses[i].type == damage.type)
            {
                value *= weaknesses[i].value;
            }
        }

        hp -= value;

        if(value == damage.value)
        {
            // normal hit text
            TakeDamageText(value, damage.type, 0);
        }
        else if(value > damage.value)
        {
            // supereffective text
            TakeDamageText(value, damage.type, 1);
        }
        else
        {
            // immune text
            TakeDamageText(value, damage.type, 2);
        }

        if(value > 0)
        {
            onTakeDamage?.Invoke(hp, maxHp);
        }

        if(hp <= 0)
        {
            OnDeath();
        }
    }

    public virtual void TakeDamageText(int damage, Damage.Type type, int hitType)
    {
        switch(hitType)
        {
            case 0:
                GameManager.Instance.UpdateTextlog(name + " takes " + damage + " " + Damage.GetTypeName(type) + " damage!" );
                break;
            case 1:
                GameManager.Instance.UpdateTextlog("It's super effective! " + name + " takes " + damage + " " + Damage.GetTypeName(type) + " damage!");
                break;
            case 2:
                GameManager.Instance.UpdateTextlog(name + " is immune to " + Damage.GetTypeName(type) + " damage!");
                break;
        }
    }

    public virtual void OnDeath()
    {
        Destroy(this.gameObject);
    }

    public virtual bool OnSpellHit(Spell spell, Entity caster)
    {
        if(canBeAttacked)
        {
            TakeDamage(spell.damage);
            return true;
        }
        return false;
    }

    public IEnumerator RunMovementQueue()
    {
        while (true)
        {
            if (movementQueue.Count > 0)
            {
                yield return MovementVisual(movementQueue.Dequeue());
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MovementVisual(Movement movement)
    {
        if (movement.bump)
        {
            Vector3 startpos = transform.position;
            // first loop
            while (Vector3.Distance(transform.position - positionOffset, movement.position) > 0.75f)
            {
                transform.position = Vector3.MoveTowards(transform.position, movement.position + positionOffset, Time.deltaTime * MOVE_SPEED);
                yield return new WaitForEndOfFrame();
            }

            // move back from bump
            while (transform.position != startpos)
            {
                transform.position = Vector3.MoveTowards(transform.position, startpos, Time.deltaTime * MOVE_SPEED);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (transform.position - positionOffset != movement.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, movement.position + positionOffset, Time.deltaTime * MOVE_SPEED);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

public class Movement
{
    public Vector3Int position;
    public bool bump;

    public Movement(Vector3Int position, bool bump)
    {
        this.position = position;
        this.bump = bump;
    }
}

[System.Serializable]
public class Damage
{
    public int value = 1;
    public Type type = Type.PHYSICAL;

    public enum Type
    {
        PHYSICAL,
        FIRE,
        GRASS,
        WATER,
        ICE,
        LIGHTNING,
        ROCK,
        WIND,
        RAINBOW
    }

    public static string[] typeNames = { "Physical", "Fire", "Grass", "Water", "Ice", "Lighting", "Rock", "Wind", "Rainbow" };

    public static string GetTypeName(Type type)
    {
        return typeNames[(int) type];
    }

    public Damage(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }
}