using UnityEngine;

public class Slime : Creature
{
    private enum Type
    {
        NEUTRAL,
        FIRE,
        ICE,
        ELECTRIC,
        WATER
    }
    private Type type = Type.NEUTRAL;
    private SpriteRenderer sprite;

    public void Start()
    {
        EntityOnStart();
        EnemyOnStart();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(type == Type.NEUTRAL)
        {
            if(spell is Flame)
            {
                type = Type.FIRE;
                sprite.color = Color.red;
                base.OnSpellHit(spell, caster);
                GameManager.Instance.UpdateTextlog("Slime became red!");
                weaknesses.Add(new Damage(Damage.Type.WATER, 4));
                weaknesses.Add(new Damage(Damage.Type.FIRE, 0));
                baseAttack = new Damage(Damage.Type.FIRE, baseAttack.value);
                return true;
            }
            else if (spell is Freeze)
            {
                type = Type.ICE;
                sprite.color = Color.lightBlue;
                base.OnSpellHit(spell, caster);
                GameManager.Instance.UpdateTextlog("Slime became light blue!");
                weaknesses.Add(new Damage(Damage.Type.FIRE, 4));
                weaknesses.Add(new Damage(Damage.Type.ICE, 0));
                baseAttack = new Damage(Damage.Type.ICE, baseAttack.value);
                return true;
            }
            else if (spell is Shock)
            {
                type = Type.ELECTRIC;
                sprite.color = Color.yellow;
                base.OnSpellHit(spell, caster);
                GameManager.Instance.UpdateTextlog("Slime became yellow!");
                weaknesses.Add(new Damage(Damage.Type.ROCK, 4));
                weaknesses.Add(new Damage(Damage.Type.LIGHTNING, 0));
                baseAttack = new Damage(Damage.Type.LIGHTNING, baseAttack.value);
                return true;
            }
            else if (spell is Splash)
            {
                type = Type.WATER;
                sprite.color = Color.blue;
                base.OnSpellHit(spell, caster);
                GameManager.Instance.UpdateTextlog("Slime became blue!");
                weaknesses.Add(new Damage(Damage.Type.LIGHTNING, 4));
                weaknesses.Add(new Damage(Damage.Type.WATER, 0));
                baseAttack = new Damage(Damage.Type.WATER, baseAttack.value);
                return true;
            }
        }
        return base.OnSpellHit(spell, caster);
    }
}
