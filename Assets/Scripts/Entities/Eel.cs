using UnityEngine;

public class Eel : Creature
{
    private bool frozen = false;
    private SpriteRenderer sprite;

    private void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(!frozen)
        {
            if(spell is Freeze)
            {
                sprite.color = Color.darkBlue;
                frozen = true;
                base.OnSpellHit(spell, caster);
                weaknesses.Add(new Damage(Damage.Type.FIRE, 20));
                weaknesses.Add(new Damage(Damage.Type.ICE, 0));
                canAct = false;
                GameManager.Instance.UpdateTextlog("Eel became frozen!");
                return true;
            }
        }
        return base.OnSpellHit(spell, caster);
    }
}
