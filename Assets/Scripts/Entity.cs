using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Vector3Int gridPosition;
    public bool turn = false;
    public int rotation = 0; // most likely unused besides player
    protected Vector3 positionOffset; // offset from grid position for visuals
    [SerializeField] protected bool canBeAttacked = true;
    [SerializeField] protected int maxHp = 1;
    public int hp { get; protected set; }
    [SerializeField] protected Damage baseAttack;

    // REPLACE WITH ACTION QUEUE
    protected Queue<int> rotationQueue;
    protected Queue<Movement> movementQueue;
    protected const float MOVE_SPEED = 10.0f;

    public void Start()
    {
        rotationQueue = new();
        movementQueue = new();
        gridPosition = Vector3Int.FloorToInt(transform.position);
        positionOffset = transform.position - gridPosition;
        hp = maxHp;
        baseAttack = new(Damage.Type.PHYSICAL, 1);
    }

    public virtual void TurnUpdate()
    {
        //yield break;
    }

    public virtual void Move(Vector3Int newPosition, bool instant = false, bool teleport = false)
    {
        if (!teleport)
        {
            RaycastHit hit;
            if (Physics.Raycast(gridPosition + new Vector3(0.5f, 0.0f, 0.5f), newPosition - gridPosition, out hit, 0.5f))
            {
                if (hit.transform.gameObject.CompareTag("Enemy"))
                {
                    Attack(baseAttack, hit.transform.GetComponent<Entity>());
                }
                movementQueue.Enqueue(new Movement(newPosition, true));
            }
            else
            {
                if (Physics.Raycast(newPosition + new Vector3(0.5f, 0.0f, 0.5f), Vector3.down, 1f))
                {
                    gridPosition = newPosition;
                    movementQueue.Enqueue(new Movement(newPosition, false));
                }
                else
                {
                    movementQueue.Enqueue(new Movement(newPosition, true));
                }
            }
        }
        else
        {
            gridPosition = newPosition;
            transform.position = newPosition + positionOffset;
        }
    }

    // might go unused outside of player
    public virtual void Rotate(ref int newRotation)
    {
        rotation = (newRotation + 4) % 4;

        rotationQueue.Enqueue(rotation);
    }

    public IEnumerator RunTurn()
    {
        StartTurn();

        while (turn)
        {
            TurnUpdate();
            yield return new WaitForEndOfFrame();
        }
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
        target.TakeDamage(damage);
    }

    public virtual void TakeDamage(Damage damage)
    {
        hp -= damage.value;

        if(hp <= 0)
        {
            OnDeath();
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

public class Damage
{
    public int value;
    public Type type;

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

    public Damage(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }
}