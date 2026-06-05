using System.Collections;
using UnityEngine;

public class Creature : Entity
{
    private bool attackReadied = false;
    public bool canAct = true;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EntityOnStart();

        EnemyOnStart();
    }

    public void EnemyOnStart()
    {
        if (CompareTag("Enemy"))
        {
            GameManager.Instance.enemies.Add(this);
            MOVE_SPEED = 3.5f;
            BUMP_SPEED = MOVE_SPEED;
            TryGetComponent<SpriteRenderer>(out spriteRenderer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TurnUpdate()
    {
        if(Vector3.Distance(GameManager.Instance.player.gridPosition, gridPosition) <= 1)
        {
            if(!attackReadied)
            {
                attackReadied = true;
            }
            else
            {
                if (canAct)
                {
                    AttackPlayer();
                }
            }
        }
        else
        {
            attackReadied = false;
        }

        turn = false;
        EndTurn();
    }

    public virtual void AttackPlayer()
    {
        GameManager.Instance.UpdateTextlog(name + " strikes at you!");
        Attack(baseAttack, GameManager.Instance.player);
        movementQueue.Enqueue(new Movement(GameManager.Instance.player.gridPosition, true));
    }

    public override void OnDeath()
    {
        GameManager.Instance.enemies.Remove(this);
        base.OnDeath();
    }

    public override int TakeDamage(Damage damage)
    {
        int value = base.TakeDamage(damage);
        if (value > 0)
        {
            StartCoroutine(DamageTint());
        }
        return value;
    }

    private IEnumerator DamageTint()
    {
        if(spriteRenderer != null)
        {
            Color savedColor = spriteRenderer.color;
            spriteRenderer.color = Color.softRed;
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.color = savedColor;
        }
    }
}
