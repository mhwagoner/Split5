using UnityEngine;
using UnityEngine.Events;

public class Water : Entity
{
    [SerializeField] private GameObject iceBlock;
    [SerializeField] private UnityEvent freezeEvent;

    public void Start()
    {
        base.Start();
    }

    public override bool OnSpellHit(Spell spell, Entity caster)
    {
        if(spell is Freeze)
        {
            var newBlock = Instantiate(iceBlock);
            newBlock.transform.position = transform.position;
            newBlock.GetComponent<Entity>().gridPosition = gridPosition;
            freezeEvent.Invoke();
            Destroy(gameObject);
        }
        return false;
    }
}
