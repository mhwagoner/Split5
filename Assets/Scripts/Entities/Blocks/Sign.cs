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
        //base.Start();
        onRead += GameManager.Instance.signMessageManager.ReadSignUI;
    }

    public override int TakeDamage(Damage damage)
    {
        if (translated) {
            onRead?.Invoke(message, spellIcon);
        } else {
            onRead?.Invoke("You cannot make out the words.", null);
        }
        return 0;
    }
}
