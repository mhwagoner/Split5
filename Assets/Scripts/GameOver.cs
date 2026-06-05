using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private TextMeshProUGUI[] text;
    private Image[] images;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponentsInChildren<TextMeshProUGUI>();
        images = GetComponentsInChildren<Image>();

        foreach(TextMeshProUGUI textmesh in text)
        {
            textmesh.alpha = 0.0f;
        }

        foreach (Image image in images)
        {
            image.color = new Color(0, 0, 0, 0.0f);
        }

        GameManager.Instance.onLose += StartFadeIn;
    }

    private void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(2.0f);

        while (images[0].color.a < 1.0f)
        {
            foreach (TextMeshProUGUI textmesh in text)
            {
                textmesh.alpha += 0.005f;
            }

            foreach (Image image in images)
            {
                image.color = new Color(0, 0, 0, image.color.a + 0.005f);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
