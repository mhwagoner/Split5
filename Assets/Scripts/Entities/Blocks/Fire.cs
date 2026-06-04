using UnityEngine;
using UnityEngine.Events;

public class Fire : Entity
{
    [SerializeField] private UnityEvent destroyEvent;

    public void Start()
    {
        base.Start();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(spell is Splash)
        {
            destroyEvent.Invoke();
            Destroy(gameObject);
        }
        return false;
    }
}
