using UnityEngine;
using UnityEngine.Events;

public class Plant : Entity
{
    private MeshRenderer meshRenderer;
    [SerializeField] private Material plantAlive;
    [SerializeField] private Material plantDead;
    private enum State
    {
        ALIVE,
        DEAD
    }
    [SerializeField] private State state;

    [SerializeField] private UnityEvent plantAliveEvent;
    [SerializeField] private UnityEvent plantDeadEvent;

    public void Start()
    {
        base.Start();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        switch(state)
        {
            case State.ALIVE:
                if(spell is Flame || spell is Shock || spell is Freeze)
                {
                    meshRenderer.material = plantDead;
                    state = State.DEAD;
                    plantDeadEvent.Invoke();
                    return true;
                }
                break;
            case State.DEAD:
                if (spell is Splash)
                {
                    meshRenderer.material = plantAlive;
                    state = State.ALIVE;
                    plantAliveEvent.Invoke();
                    return true;
                }
                break;
        }
        return false;
    }
}
