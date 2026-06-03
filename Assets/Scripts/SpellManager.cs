using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class SpellManager : MonoBehaviour
{
    // Note to me - this class is a monobehavior so you can easily instantiate prefabs for visuals and other things
    public List<Spell> spells;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spells.Add(new Flame());
        GameManager.Instance.spellManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Spell
{
    public Damage damage; // unusued for some spell types
    public List<Rune> runes; // RuneDraw is gonna use this, make a script to export runepoints to a formatted txt

    public Spell()
    {
        damage = new(Damage.Type.PHYSICAL, 1);
    }

    public virtual void Cast(Entity caster)
    {
        AttackCastForward(caster);
    }

    /// <summary>
    /// The basic generic cast type, used for spells without wacky rules like the base elemental spells
    /// </summary>
    public void AttackCastForward(Entity caster)
    {
        RaycastHit hit;
        if (Physics.Raycast(caster.gridPosition + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(0, caster.rotation * 90f, 0)) * Vector3.forward, out hit, 1f))
        { // HIT
            Entity entity;
            if (hit.collider.TryGetComponent<Entity>(out entity))
            {
                entity.OnSpellHit(this, damage);
            }
        }
        else
        { // MISS

        }
    }
}

public class Flame : Spell
{
    public Flame()
    {
        runes = new List<Rune>(new Rune[] { new Rune(new RuneLine[] { new RuneLine(-1, 0, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 1, 0), }) });
        damage = new(Damage.Type.FIRE, 1);
    }
}