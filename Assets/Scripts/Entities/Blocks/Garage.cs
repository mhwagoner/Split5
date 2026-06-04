using UnityEngine;
using UnityEngine.Events;

public class Garage : Entity
{
    [SerializeField] private UnityEvent openEvent;

    public void Start()
    {
        base.Start();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(spell is Shock)
        {
            Move(gridPosition + Vector3Int.up, false, true);
            openEvent.Invoke();
        }
        return false;
    }
}
