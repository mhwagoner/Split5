using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFullscreen()
    {
        Screen.fullScreen = true;
    }

    public void SetWindowed()
    {
        Screen.fullScreen = false;
    }

    public void MuteAudio()
    {
        AudioListener.volume = 0f;
    }

    public void UnmuteAudio()
    {
        AudioListener.volume = 1f;
    }

    public void LongerTimer()
    {
        //GameManager.Instance.gameTimer = 60.0f;
    }

    public void ShorterTimer()
    {
        //GameManager.Instance.gameTimer = 30.0f;
    }
}
