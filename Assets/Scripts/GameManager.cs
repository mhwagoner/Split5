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
	public SpellManager spellManager { get; set; }
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
	public Coroutine gameCoroutine;
    public bool playerTurn = true;
	public bool runGameLogic = true;

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

	public GameManager()
	{
		enemies = new();
	}

    // Interesting note (to me): this doesn't run by itself, another object needs to give it a "push" to make the game run
    public void RunGame()
	{
		TurnPlayer();

        //yield return new WaitForEndOfFrame();

        TurnEnemy();

		gameRunner.StartCoroutine(gameRunner.RoundEnd());
	}

	public void TurnPlayer()
	{
        player.TurnUpdate();
    }

	public void TurnEnemy()
	{
		foreach (Entity enemy in enemies)
		{
			enemy.TurnUpdate();
		}
    }

	public IEnumerator TurnOther()
	{
		yield return null;
	}

	public void PlayerDeath()
	{
		gameRunner.StopCoroutine(Timer());
		runGameLogic = false;

    }

	public void TimeExpire()
	{
		onTimeExpire?.Invoke();
		runGameLogic = false;
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
		player.audioSource.PlayOneShot(player.clockTick);
        yield return new WaitForSeconds(1.0f);
        // tick
        player.audioSource.PlayOneShot(player.clockTick);
        yield return new WaitForSeconds(1.0f);
        // tick
        player.audioSource.PlayOneShot(player.clockTick);
        yield return new WaitForSeconds(1.0f);
        // tick
        player.audioSource.PlayOneShot(player.clockTick);
        yield return new WaitForSeconds(1.0f);
        // tick
        player.audioSource.PlayOneShot(player.clockTick);
        yield return new WaitForSeconds(1.0f);
        // gong
        player.audioSource.Stop();
        player.audioSource.PlayOneShot(player.clockTick);
        TimeExpire();
    }
}
