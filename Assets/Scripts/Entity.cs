using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Vector3Int gridPosition;
    public bool turn = false;
    protected int rotation = 0; // most likely unused besides player
    protected Vector3 positionOffset; // offset from grid position for visuals
    protected bool canBeAttacked = true;
    [SerializeField] protected int maxHp = 1;
    public int hp { get; protected set; }
    [SerializeField] protected Damage baseAttack;

    // REPLACE WITH ACTION QUEUE
    protected Queue<int> rotationQueue;
    protected Queue<Movement> movementQueue;

    public void Start()
    {
        rotationQueue = new();
        movementQueue = new();
        gridPosition = Vector3Int.FloorToInt(transform.position);
        positionOffset = transform.position - gridPosition;
        hp = maxHp;
        baseAttack = new(Damage.Type.PHYSICAL, 1);
    }

    public virtual IEnumerator TurnUpdate()
    {
        yield break;
    }

    public virtual void Move(Vector3Int newPosition, bool instant = false, bool teleport = false)
    {
        RaycastHit hit;
        if(Physics.Raycast(gridPosition + new Vector3(0.5f, 0.5f, 0.5f), newPosition - gridPosition, out hit, 1f))
        {
            if(hit.transform.gameObject.CompareTag("Enemy"))
            {
                Attack(baseAttack, hit.transform.GetComponent<Entity>());
            }
            movementQueue.Enqueue(new Movement(newPosition, true));
        }
        else
        {
            gridPosition = newPosition;
            movementQueue.Enqueue(new Movement(newPosition, false));
        }
    }

    // might go unused outside of player
    public virtual void Rotate(ref int newRotation)
    {
        rotation = (newRotation + 4) % 4;

        rotationQueue.Enqueue(rotation);
    }

    public virtual void StartTurn()
    {
        turn = true;
        StartCoroutine(TurnUpdate());
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
        LIGHTNING
    }

    public Damage(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }
}