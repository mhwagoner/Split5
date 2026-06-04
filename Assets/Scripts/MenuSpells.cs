using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.FilePathAttribute;

public class MenuSpells : SpellManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add all spells to list (kinda sucks but who care)
        spells = new List<Spell>(new Spell[]
        {
            new MenuTurnLeft(),
            new MenuTurnRight(),
            new MenuStartGame()
        });
        GameManager.Instance.spellManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MenuTurnLeft : Spell
{
    public MenuTurnLeft()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, 0), new RuneLine(-1, 0, -1, -1), new RuneLine(-1, -1, -1, 1), } )
            });
    }

    public override void Cast(Entity caster)
    {
        //int rot = caster.rotation - 1;
        //caster.Rotate(ref rot);
        caster.Move(caster.gridPosition + Vector3Int.left * 10);
    }
}

public class MenuTurnRight : Spell
{
    public MenuTurnRight()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(1, 1, 1, -1), new RuneLine(1, -1, 1, 0), new RuneLine(1, 0, 1, 1), } )
            });
    }

    public override void Cast(Entity caster)
    {
        // int rot = caster.rotation + 1;
        //caster.Rotate(ref rot);
        caster.Move(caster.gridPosition + Vector3Int.right * 10);
    }
}

public class MenuStartGame : Spell
{
    public MenuStartGame()
    {
        runes = new List<Rune>(new Rune[] {
            // S
            new Rune( new RuneLine[] { new RuneLine(1, 1, 0, 1), new RuneLine(0, 1, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, -1), new RuneLine(1, -1, 0, -1), new RuneLine(0, -1, -1, -1), } ),
            // C
            new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, 0), new RuneLine(-1, 0, -1, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, 1, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 0, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 1, -1, 1), new RuneLine(-1, 1, 0, 0), } ),
            // R
            new Rune( new RuneLine[] { new RuneLine(0, -1, 0, 0), new RuneLine(0, 0, 1, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(-1, -1, -1, 1), new RuneLine(-1, 1, 0, 0), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 0, 0, 1), new RuneLine(0, 1, 1, 1), } ),
            new Rune( new RuneLine[] { new RuneLine(1, -1, 1, 1), new RuneLine(1, 1, 1, 0), } ),
            // A
            new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, 0), new RuneLine(-1, 0, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 1, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 0, 1), new RuneLine(0, 1, 1, 1), new RuneLine(1, 1, 1, 0), } ),
            // W
            new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, 1, -1), new RuneLine(1, -1, 1, 1), } ),
            // L
            new Rune( new RuneLine[] { new RuneLine(0, 1, 0, 0), new RuneLine(0, 0, 0, -1), new RuneLine(0, -1, 1, -1), } )
            });
    }

    public override void Cast(Entity caster)
    {
        SceneManager.LoadScene("SampleScene");
    }
}