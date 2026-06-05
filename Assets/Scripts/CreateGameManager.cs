using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Tilemaps;

// RENAME TO GAME RUNNER OR SOMETHING
public class CreateGameManager : MonoBehaviour
{
    private bool newRound = false;
    [SerializeField] private bool runTimer = true;
    public Coroutine timerCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.Instance.grid = FindFirstObjectByType<Grid>();
        GameManager.Instance.tilemaps = new();
        foreach (Tilemap tilemap in GameManager.Instance.grid.GetComponentsInChildren<Tilemap>())
        {
            if(tilemap.gameObject.layer == PhysicsLayers.GetLayerOrdinal("Ground"))
            {
                GameManager.Instance.tilemaps.Add((int) tilemap.transform.position.y, tilemap);
            }
        }
        GameManager.Instance.gameRunner = this;
        GameManager.Instance.onTimeExpire += StartReduceTimescale;

        if(runTimer) timerCoroutine = StartCoroutine(GameManager.Instance.Timer());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.RunGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.runGameLogic)
        {
            if (GameManager.Instance.playerTurn)
            {
                GameManager.Instance.TurnPlayer();
            }
            else
            {
                GameManager.Instance.TurnEnemy();
                GameManager.Instance.playerTurn = true;
            }
        }
    }

    private void StartReduceTimescale()
    {
        StartCoroutine(ReduceTimescale());
    }

    private IEnumerator ReduceTimescale()
    {
        while (Time.timeScale > 0.0f)
        {
            Time.timeScale -= Mathf.Min(0.02f, Time.timeScale);
            print(Time.timeScale);
            GameManager.Instance.onChangeTimescale?.Invoke();
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator RoundEnd()
    {
        yield return new WaitForEndOfFrame();
        newRound = true;
    }
}
