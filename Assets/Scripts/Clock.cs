using UnityEngine;

public class Clock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Animator>().speed = 30.0f / GameManager.timerDuration;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
