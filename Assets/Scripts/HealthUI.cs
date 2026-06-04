using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private int number;
    [SerializeField] private Sprite[] healthSprites;
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        GameManager.Instance.healthImages[number] = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthSprite(int value)
    {
        image.sprite = healthSprites[value];
    }
}
