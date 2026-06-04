using UnityEngine;
using UnityEngine.Events;

public class Burnable : Entity
{
    [SerializeField] private UnityEvent destroyEvent;

    public void Start()
    {
        base.Start();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(spell is Flame)
        {
            destroyEvent.Invoke();
            Destroy(gameObject);
        }
        return false;
    }
}
