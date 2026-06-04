using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : Creature
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference turnAction;
    [SerializeField] private InputActionReference skipAction;
    private const float ROTATION_SPEED = 700f;
    private Spell queueSpell = null;

    private int queueRotation = 0;

    [Header("Sound Effects")]
    [SerializeField] public AudioClip clockTick;

    private void Start()
    {
        base.Start();

        GameManager.Instance.player = this;
        StartCoroutine(RunRotationQueue());
    }

    private void Update()
    {
        if(turnAction.ToInputAction().WasPressedThisFrame())
        {
            queueRotation = Mathf.RoundToInt(turnAction.ToInputAction().ReadValue<float>());
        }

        if(turn)
        {
            TurnUpdate();
        }
    }

    public override void TurnUpdate()
    {
        if(queueSpell != null)
        {
            queueSpell.Cast(this);
            queueSpell = null;
            EndTurn();
            return;
        }

        Input();
    }

    public override IEnumerator RunTurn()
    {
        StartTurn();

        while (turn)
        {
            TurnUpdate();

            yield return new WaitForEndOfFrame(); // dont wait frame if action taken
        }

        yield break;
    }

    private void Input()
    {
        // DEBUG CHANGE SPACE TO SKIP TURN
        if(skipAction.ToInputAction().WasPressedThisFrame())
        {
            EndTurn();
            return;
        }

        if (queueRotation != 0)
        {
            int turnInput = queueRotation;
            int newRotation = rotation + queueRotation;
            Rotate(ref newRotation);
            queueRotation = 0;
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
                return;
            }
        }
    }

    public override void Rotate(ref int newRotation)
    {
        base.Rotate(ref newRotation);
    }

    public void QueueCast(Spell spell)
    {
        queueSpell = spell;
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

    public IEnumerator RotationVisual(int rotation)
    {
        Quaternion targetRotation = Quaternion.LookRotation(Quaternion.Euler(new Vector3(0, rotation * 90f, 0)) * Vector3.forward);
        while (transform.rotation != targetRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * ROTATION_SPEED);
            yield return new WaitForEndOfFrame();
        }
    }

    public override void EndTurn()
    {
        base.EndTurn();
        GameManager.Instance.playerTurn = false;
    }

    public override void OnDeath()
    {
        GameManager.Instance.PlayerDeath();
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        print(damage.value);
        print(hp);
    }
}
