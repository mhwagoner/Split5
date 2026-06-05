using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FadeToGray : MonoBehaviour
{
    private ColorAdjustments color;

    private void Awake()
    {
        Shader.SetGlobalFloat("_Saturation", 1f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Volume>().profile.TryGet<ColorAdjustments>(out color);
        GameManager.Instance.onChangeTimescale += ChangeFade;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeFade()
    {
        Shader.SetGlobalFloat("_Saturation", Mathf.Min(Time.timeScale, 1f));
        color.saturation.value = (Mathf.Min(Time.timeScale, 1f) - 1f) * 100f;
        //color.contrast.value = (Mathf.Min(Time.timeScale, 1f) - 1f) * -100f;
    }
}
