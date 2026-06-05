using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class Sign : Entity
{
    public bool translated = true;
    public string message = "The sign's author forgot to write a message.";
    public Sprite spellIcon = null;
    public Action<string, Sprite> onRead;

    public void Start()
    {
        //base.Start();
        onRead += GameManager.Instance.signMessageManager.ReadSignUI;
    }

    public override void TakeDamage(Damage damage)
    {
        GameManager.Instance.signMessageManager.gameObject.SetActive(true);
        onRead?.Invoke(message, spellIcon);
    }
}
