using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : Entity
{
    private Spell queueSpell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        rotationQueue = new();
        StartCoroutine(RunRotationQueue());
        GameManager.Instance.runeDraw.onSpellCast += QueueCast;
        MOVE_SPEED = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        if (queueSpell != null)
        {
            queueSpell.Cast(this);
            queueSpell = null;
        }
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
        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * PlayerController.ROTATION_SPEED);
            yield return new WaitForEndOfFrame();
        }
    }
}
