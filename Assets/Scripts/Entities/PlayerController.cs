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
    public const float ROTATION_SPEED = 700f;
    private Spell queueSpell = null;
    public Action onPlayerInput;

    private int queueRotation = 0;

    [Header("Sound Effects")]
    [SerializeField] public AudioClip clockTick;
    [SerializeField] public AudioClip clockTickFast;
    private int stepCounter = 0;
    [SerializeField] public AudioClip stepSFX1;
    [SerializeField] public AudioClip stepSFX2;
    [SerializeField] public AudioClip stepSFX3;
    [SerializeField] public AudioClip stepSFX4;
    [SerializeField] public AudioClip hitSFX;
    [SerializeField] public AudioClip fallSFX;
    [SerializeField] public AudioClip gongSFX;

    private void Start()
    {
        EntityOnStart();

        GameManager.Instance.player = this;
        StartCoroutine(RunRotationQueue());
        GameManager.Instance.runeDraw.onSpellCast += QueueCast;
        onTakeDamage += GameManager.Instance.UpdateHealthUI;
        onPlayerInput += GameManager.Instance.signMessageManager.HideSignUI;
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
            GameManager.Instance.UpdateTextlog("You cast " + queueSpell.name + "!");
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
            onPlayerInput?.Invoke();
            
            EndTurn();
            return;
        }

        if (queueRotation != 0)
        {
            onPlayerInput?.Invoke();

            int turnInput = queueRotation;
            int newRotation = rotation + queueRotation;
            Rotate(ref newRotation);
            queueRotation = 0;
        }

        if (moveAction.ToInputAction().WasPressedThisFrame())
        {
            Vector2Int movement = Vector2Int.CeilToInt(moveAction.ToInputAction().ReadValue<Vector2>());

            onPlayerInput?.Invoke();

            if (movement != Vector2Int.zero)
            {
                stepCounter = (stepCounter + 1) % 4;
                switch (stepCounter)
                {
                    case 0:
                        audioSource.PlayOneShot(stepSFX1, 1.2f);
                        break;
                    case 1:
                        audioSource.PlayOneShot(stepSFX2, 1.2f);
                        break;
                    case 2:
                        audioSource.PlayOneShot(stepSFX3, 1.2f);
                        break;
                    case 3:
                        audioSource.PlayOneShot(stepSFX4, 1.2f);
                        break;
                }

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

    public override void TakeDamageText(int damage, Damage.Type type, int hitType)
    {
        switch (hitType)
        {
            case 0:
                GameManager.Instance.UpdateTextlog("You take " + damage + " " + Damage.GetTypeName(type) + " damage!");
                break;
            case 1:
                GameManager.Instance.UpdateTextlog("It's super effective! " + name + " takes " + damage + " " + Damage.GetTypeName(type) + " damage!");
                break;
            case 2:
                GameManager.Instance.UpdateTextlog(name + " is immune to " + Damage.GetTypeName(type) + " damage!");
                break;
        }
    }

    public override void Attack(Damage damage, Entity target)
    {
        base.Attack(damage, target);
        audioSource.PlayOneShot(hitSFX);
    }
}

