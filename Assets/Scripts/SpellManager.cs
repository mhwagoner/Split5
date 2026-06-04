using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class SpellManager : MonoBehaviour
{
    // Note to me - this class is a monobehavior so you can easily instantiate prefabs for visuals and other things
    public List<Spell> spells;

    public GameObject icePrefab;
    public Material rainbowMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add all spells to list (kinda sucks but who care)
        spells = new List<Spell>(new Spell[]
        {
            new Flame(),
            new Shock(),
            new Splash(),
            new Freeze(),
            new Breeze(),
            new Translate(),
            new Shine(),
            new Flip(),
            new Rock(),
            new Rainbow()
        });
        GameManager.Instance.spellManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Spell
{
    public Damage damage; // unusued for some spell types
    public List<Rune> runes; // RuneDraw is gonna use this, make a script to export runepoints to a formatted txt

    public Spell()
    {
        damage = new(Damage.Type.PHYSICAL, 1);
    }

    public virtual void Cast(Entity caster)
    {
        AttackCastForward(caster);
    }

    /// <summary>
    /// The basic generic cast type, used for spells without wacky rules like the base elemental spells
    /// </summary>
    public void AttackCastForward(Entity caster)
    {
        RaycastHit hit;
        if (TryCastForward(caster, out hit))
        { // HIT
            Entity entity;
            if (hit.collider.TryGetComponent<Entity>(out entity))
            {
                entity.OnSpellHit(this, caster);
            }
        }
        else
        { // MISS

        }
    }

    public bool TryCastForward(Entity caster, out RaycastHit hit)
    {
        return Physics.Raycast(caster.gridPosition + new Vector3(0.5f, 0.0f, 0.5f), Quaternion.Euler(new Vector3(0, caster.rotation * 90f, 0)) * Vector3.forward * 0.5f, out hit, 1f);
    }
}

public class Flame : Spell
{
    public Flame()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(0, 1, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 0, 1), } ),
            new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, 0), new RuneLine(-1, 0, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, -1, 1), } ),
            new Rune( new RuneLine[] { new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, 1, -1), new RuneLine(1, -1, 1, 0), new RuneLine(1, 0, 1, 1), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 0, -1), new RuneLine(0, -1, 1, -1), new RuneLine(1, -1, 0, 0), } )
        });
        damage = new(Damage.Type.FIRE, 1);
    }
}

public class Shock : Spell
{
    public Shock()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(0, 1, -1, 1), new RuneLine(-1, 1, 1, 1), new RuneLine(1, 1, 0, 0), } ),
            new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, 0), new RuneLine(-1, 0, 0, 0), new RuneLine(0, 0, -1, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, 1, 0), new RuneLine(1, 0, 1, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 1, -1), new RuneLine(1, -1, 0, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 1, -1, 1), new RuneLine(-1, 1, -1, 0), new RuneLine(-1, 0, 0, 0), new RuneLine(0, 0, 1, 0), new RuneLine(1, 0, 1, -1), new RuneLine(1, -1, 0, -1), } )
            } );
        damage = new(Damage.Type.LIGHTNING, 1);
    }
}

public class Splash : Spell
{
    public Splash()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 0, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 1, 0), } )
            });
        damage = new(Damage.Type.WATER, 1);
    }
}

public class Freeze : Spell
{
    public Freeze()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, -1), new RuneLine(1, -1, 0, 0), new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, 1, 1), } )
            });
        damage = new(Damage.Type.ICE, 1);
    }
}

public class Breeze : Spell
{
    public Breeze()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 1, 0, 1), new RuneLine(0, 1, 1, 1), new RuneLine(1, 1, -1, 1), } ),
            new Rune( new RuneLine[] { new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, 1, -1), new RuneLine(1, -1, -1, -1), } ),
            new Rune( new RuneLine[] { new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 1, 0), new RuneLine(1, 0, 0, 0), } ),
            new Rune( new RuneLine[] { new RuneLine(-1, 0, -1, 1), new RuneLine(-1, 1, 0, 1), new RuneLine(0, 1, 1, 1), new RuneLine(1, 1, 1, 0), new RuneLine(1, 0, 0, 0), new RuneLine(0, 0, -1, 0), } )
            });
        damage = new(Damage.Type.WIND, 1);
    }
}

public class Translate : Spell
{
    public Translate()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 1, 0, 1), new RuneLine(0, 1, 1, 1), new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, 0, -1), } )
            });
    }
}

public class Shine : Spell
{
    public Shine()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 0, -1, -1), new RuneLine(-1, -1, 0, -1), new RuneLine(0, -1, 1, -1), new RuneLine(1, -1, 0, 0), new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 0, -1), new RuneLine(0, -1, 1, -1), new RuneLine(1, -1, 1, 0), } )
            });
    }
}
public class Flip : Spell
{
    public Flip()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, -1, 0, -1), new RuneLine(0, -1, 1, -1), new RuneLine(1, -1, 1, 0), new RuneLine(1, 0, 1, 1), new RuneLine(1, 1, 0, 1), new RuneLine(0, 1, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, 1), } )
            });
    }
}

public class Rock : Spell
{
    public Rock()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, -1, 1, -1), new RuneLine(1, -1, 1, 1), new RuneLine(1, 1, -1, 1), new RuneLine(-1, 1, -1, -1), } )
            });
        damage = new(Damage.Type.ROCK, 1);
    }
}

public class Rainbow : Spell
{
    public Rainbow()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, -1, 1), new RuneLine(-1, 1, -1, -1), new RuneLine(-1, -1, 1, -1), new RuneLine(1, -1, 1, 1), new RuneLine(1, 1, -1, 1), } )
            });
        damage = new(Damage.Type.RAINBOW, 1);
    }

    public override void Cast(Entity caster)
    {
        RaycastHit hit;
        if (TryCastForward(caster, out hit))
        { // HIT
            Entity entity;
            if (hit.collider.TryGetComponent<Entity>(out entity))
            {
                
            }
            else
            {
                //GameObject block = Object.Instantiate(GameManager.Instance.spellManager.rainbowBlock);
                //block.transform.position = hit.collider.transform.position;
                //Object.Destroy(hit.collider.gameObject);
            }

            MeshRenderer meshRenderer;
            if (hit.collider.TryGetComponent<MeshRenderer>(out meshRenderer))
            {
                meshRenderer.material = GameManager.Instance.spellManager.rainbowMaterial;
            }
        }
        else
        { // MISS

        }
    }
}

// Heart Shape: new Rune( new RuneLine[] { new RuneLine(-1, 0, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 1, 0), new RuneLine(1, 0, 1, -1), new RuneLine(1, -1, 0, -1), new RuneLine(0, -1, -1, -1), new RuneLine(-1, -1, -1, 0), } )
// alt cast with lines separating: new Rune( new RuneLine[] { new RuneLine(-1, 1, -1, 0), new RuneLine(-1, 0, -1, -1), new RuneLine(-1, -1, 0, -1), new RuneLine(0, -1, 1, -1), new RuneLine(1, -1, 1, 0), new RuneLine(1, 0, 1, 1), new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, -1, 1), new RuneLine(-1, 1, 0, 0), new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, 1, -1), } )

public class SecretYume : Spell
{
    public SecretYume()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, 1, 0, 1), new RuneLine(0, 1, 1, 1), new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, 1, -1), new RuneLine(1, -1, 0, 0), new RuneLine(0, 0, -1, 1), } )
            });
    }

    public override void Cast(Entity caster)
    {
        // go to scene
    }
}

public class SecretLodge : Spell
{
    public SecretLodge()
    {
        runes = new List<Rune>(new Rune[] {
            new Rune( new RuneLine[] { new RuneLine(-1, -1, 0, -1), new RuneLine(0, -1, 1, -1), new RuneLine(1, -1, 0, 0), new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 1, 0), new RuneLine(1, 0, 1, 1), new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, -1, 1), new RuneLine(-1, 1, -1, 0), } )
            });
    }

    public override void Cast(Entity caster)
    {
        // go to scene
    }
}