using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Tilemaps;

public class CreateGameManager : MonoBehaviour
{
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
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(GameManager.Instance.TurnUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
