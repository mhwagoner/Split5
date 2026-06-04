using UnityEngine;
using UnityEngine.Events;

public class Torch : Entity
{
    private MeshRenderer meshRenderer;
    [SerializeField] private Material torchLit;
    [SerializeField] private Material torchUnlit;
    [SerializeField] private Material torchFreeze;
    private enum State
    {
        UNLIT,
        LIT,
        FREEZE
    }
    [SerializeField] private State state;

    [SerializeField] private UnityEvent torchLitEvent;
    [SerializeField] private UnityEvent torchUnlitEvent;
    [SerializeField] private UnityEvent torchFreezeEvent;

    public void Start()
    {
        base.Start();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        switch(state)
        {
            case State.UNLIT:
                if(spell is Flame || spell is Shock)
                {
                    meshRenderer.material = torchLit;
                    state = State.LIT;
                    torchLitEvent.Invoke();
                    return true;
                }
                else if(spell is Freeze)
                {
                    meshRenderer.material = torchFreeze;
                    state = State.FREEZE;
                    torchFreezeEvent.Invoke();
                    return true;
                }
                break;
            case State.LIT:
                if (spell is Splash || spell is Breeze)
                {
                    meshRenderer.material = torchUnlit;
                    state = State.UNLIT;
                    torchUnlitEvent.Invoke();
                    return true;
                }
                else if (spell is Freeze)
                {
                    meshRenderer.material = torchFreeze;
                    state = State.FREEZE;
                    torchFreezeEvent.Invoke();
                    return true;
                }
                break;
            case State.FREEZE:
                if (spell is Flame)
                {
                    meshRenderer.material = torchUnlit;
                    state = State.UNLIT;
                    torchUnlitEvent.Invoke();
                    return true;
                }
                break;
        }
        return false;
    }
}
