using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GameManager
{
    public PlayerController player { get; set; }
	public RuneDraw runeDraw { get; set; }
    public List<Entity> enemies { get; set; }
    public List<Entity> otherEntities { get; set; }
    public enum Turn
	{
		PLAYER,
		ENEMY,
		OTHER
	}
    public Turn currentTurn { get; set; }
	public Grid grid { get; set; }
	public Dictionary<int, Tilemap> tilemaps { get; set; }

    public static GameManager theInstance { get; private set; }
    public static GameManager Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    // Interesting note (to me): this doesn't run by itself, another object needs to give it a "push" to make the game run
    public IEnumerator TurnUpdate()
	{
		while(true)
		{
			TurnPlayer();
			yield return new WaitForEndOfFrame();
		}
	}

	public void TurnPlayer()
	{
        player.StartTurn();
    }

	public void TurnEnemy()
	{

	}

	public void TurnOther()
	{

	}

	public void InitiatePositions()
	{
        player.gridPosition = GameManager.Instance.grid.WorldToCell(player.transform.position);
    }
}
