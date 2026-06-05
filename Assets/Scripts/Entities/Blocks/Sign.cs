using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class Sign : Entity
{
    public bool translated = true;
    public string message;
    public Sprite spellIcon;
    public Action<string, Sprite> onRead;

    public void Start()
    {
        base.Start();
        onRead += GameManager.Instance.signMessageManager.ReadSignUI;
    }

    public override void TakeDamage(Damage damage)
    {
        onRead?.Invoke(message, spellIcon);
    }
}
