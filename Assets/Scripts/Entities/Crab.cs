using UnityEngine;

public class Crab : Creature
{
    private bool flipped = false;
    private SpriteRenderer sprite;
    [SerializeField] private Sprite crab;
    [SerializeField] private Sprite flippedCrab;

    private void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(!flipped)
        {
            if(spell is Flip)
            {
                flipped = true;
                base.OnSpellHit(spell, caster);
                sprite.sprite = flippedCrab;
                weaknesses.RemoveAt(0);
                weaknesses.Add(new Damage(Damage.Type.FIRE, 4));
                GameManager.Instance.UpdateTextlog("The crab was flipped over!");
                return true;
            }
            else
            {
                GameManager.Instance.UpdateTextlog("The spell had no effect on Crab!");
            }
        }
        return base.OnSpellHit(spell, caster);
    }
}
