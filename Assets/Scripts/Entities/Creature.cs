using UnityEngine;

public class Creature : Entity
{
    private bool attackReadied = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        if (CompareTag("Enemy"))
        {
            GameManager.Instance.enemies.Add(this);
            MOVE_SPEED = 3.5f;
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
                Attack();
            }
        }
        else
        {
            attackReadied = false;
        }

        turn = false;
        EndTurn();
    }

    public virtual void Attack()
    {
        Move(GameManager.Instance.player.gridPosition);
    }

    public override void OnDeath()
    {
        GameManager.Instance.enemies.Remove(this);
        base.OnDeath();
    }
}
