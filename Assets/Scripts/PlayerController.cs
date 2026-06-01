using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Creature
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference turnAction;
    private const float ROTATION_SPEED = 700f;
    private const float MOVE_SPEED = 10.0f;

    private void Start()
    {
        base.Start();

        GameManager.Instance.player = this;
        StartCoroutine(RunMovementQueue());
        StartCoroutine(RunRotationQueue());
    }

    private void Update()
    {
        
    }

    public override IEnumerator TurnUpdate()
    {
        base.TurnUpdate();

        Input();
        yield return new WaitForEndOfFrame();
    }

    private void Input()
    {
        if (turnAction.ToInputAction().WasPressedThisFrame())
        {
            int turnInput = Mathf.CeilToInt(turnAction.ToInputAction().ReadValue<float>());

            if (turnInput != 0)
            {
                int newRotation = rotation + turnInput;
                Rotate(ref newRotation);
            }
        }

        if (moveAction.ToInputAction().WasPressedThisFrame())
        {
            Vector2Int movement = Vector2Int.CeilToInt(moveAction.ToInputAction().ReadValue<Vector2>());

            if (movement != Vector2Int.zero)
            {
                if (movement.x != 0 && movement.y != 0)
                {
                    movement.x = 0; // vertical movement prioritized
                }

                // Calculate movement vector relative to rotation (NOT camera rotation, true rotation)
                Quaternion targetRotation = Quaternion.LookRotation(Quaternion.Euler(new Vector3(0, rotation * 90f, 0)) * Vector3.forward);
                Vector3 moveVector = targetRotation * Vector3.right * movement.x + targetRotation * Vector3.forward * movement.y;
                Move(Vector3Int.RoundToInt(gridPosition + moveVector));
                EndTurn();
            }
        }
    }

    public override void Rotate(ref int newRotation)
    {
        base.Rotate(ref newRotation);
    }

    public IEnumerator RunMovementQueue()
    {
        while (true) {
            if (movementQueue.Count > 0)
            {
                yield return MovementVisual(movementQueue.Dequeue());
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator RunRotationQueue()
    {
        while (true)
        {
            if (rotationQueue.Count > 0)
            {
                yield return RotationVisual(rotationQueue.Dequeue());
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

    public IEnumerator RotationVisual(int rotation)
    {
        Quaternion targetRotation = Quaternion.LookRotation(Quaternion.Euler(new Vector3(0, rotation * 90f, 0)) * Vector3.forward);
        while (transform.rotation != targetRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * ROTATION_SPEED);
            yield return new WaitForEndOfFrame();
        }
    }
}
