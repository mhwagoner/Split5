using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GameManager
{
	public CreateGameManager gameRunner { get; set; }
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
	public float time;
	private float startTime;
	public Action onTimeExpire; // Used to start coroutines in effect scripts to slow until timeScale == 0;
	public Action onChangeTimescale;

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
    public IEnumerator RunGame()
	{
		while(true)
		{
			yield return TurnPlayer();
		}
	}

	public IEnumerator TurnPlayer()
	{
        yield return player.RunTurn();
    }

	public void TurnEnemy()
	{

	}

	public void TurnOther()
	{

	}

	public void PlayerDeath()
	{
		gameRunner.StopCoroutine(RunGame());
		gameRunner.StopCoroutine(Timer());
	}

	public void TimeExpire()
	{
        gameRunner.StopAllCoroutines();
        player.StopCoroutine(player.RunTurn());
		onTimeExpire?.Invoke();
    }

	public void InitiatePositions()
	{
        player.gridPosition = GameManager.Instance.grid.WorldToCell(player.transform.position);
    }

	public float GetElapsedTime()
	{
		return Time.time - startTime;
	}

	public IEnumerator Timer()
	{
		yield return new WaitForSeconds(25.0f);
        // play ticking sound (like the chrono trigger title screen)
        yield return new WaitForSeconds(1.0f);
		// tick
        yield return new WaitForSeconds(1.0f);
        // tick
        yield return new WaitForSeconds(1.0f);
        // tick
        yield return new WaitForSeconds(1.0f);
        // tick
        yield return new WaitForSeconds(1.0f);
		// gong
		TimeExpire();
    }
}
