using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;
using static UnityEditor.FilePathAttribute;

public class WinScreenSpells : SpellManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add all spells to list (kinda sucks but who care)
        spells = new List<Spell>(new Spell[]
        {
            new GoToTitle()
        });
        GameManager.Instance.spellManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class GoToTitle : Spell
{
    public GoToTitle()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune(new RuneLine[] { new RuneLine(-1, 1, 1, 1), new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 1, -1), new RuneLine(1, -1, 0, 0), new RuneLine(0, 0, -1, 1), })
    });
    }

    public override void Cast(Entity caster)
    {
        SceneManager.LoadScene("MainMenu");
    }
}