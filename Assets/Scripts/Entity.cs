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

    // REPLACE WITH ACTION QUEUE
    protected Queue<int> rotationQueue;
    protected Queue<Movement> movementQueue;

    public void Start()
    {
        //gridPosition = 
        rotationQueue = new();
        movementQueue = new();
        gridPosition = Vector3Int.FloorToInt(transform.position);
        positionOffset = transform.position - gridPosition;
    }

    public virtual IEnumerator TurnUpdate()
    {
        yield break;
    }

    public virtual void Move(Vector3Int newPosition, bool instant = false, bool teleport = false)
    {
        if(Physics.Raycast(gridPosition + new Vector3(0.5f, 0.5f, 0.5f), newPosition - gridPosition, 1f))
        {
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
