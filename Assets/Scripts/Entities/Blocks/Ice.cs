using UnityEngine;
using UnityEngine.Events;

public class Ice : Entity
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
            Destroy(gameObject);
            destroyEvent.Invoke();
            return true;
        }
        else if(spell is Breeze)
        {
            int iterations = 0;
            while(Move(Vector3Int.RoundToInt(Vector3.Normalize(gridPosition - caster.gridPosition) + gridPosition)))
            {
                if(iterations > 10)
                {
                    return true;
                }
                iterations++;
            }
        }
        return false;
    }
}
