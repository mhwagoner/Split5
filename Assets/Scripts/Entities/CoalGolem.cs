using UnityEngine;

public class CoalGolem : Creature
{
    private bool smoke = false;
    private SpriteRenderer sprite;
    [SerializeField] private Sprite golem;
    [SerializeField] private Sprite golemSmoke;

    private void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(!smoke)
        {
            if(spell is Flame)
            {
                smoke = true;
                base.OnSpellHit(spell, caster);
                sprite.sprite = golemSmoke;
                weaknesses.RemoveAt(0);
                weaknesses.Add(new Damage(Damage.Type.WIND, 5));
                GameManager.Instance.UpdateTextlog("The Golem turned to smoke!");
                return true;
            }
        }
        return base.OnSpellHit(spell, caster);
    }
}
