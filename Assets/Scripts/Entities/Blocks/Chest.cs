using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Chest : Entity
{
    public void Start()
    {

    }

    public override void TakeDamage(Damage damage)
    {
        SceneManager.LoadScene("WinScreen");
    }
}
