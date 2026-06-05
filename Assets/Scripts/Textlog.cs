using TMPro;
using UnityEngine;

public class Textlog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.textlogMesh = GetComponent<TextMeshProUGUI>();
        GameManager.Instance.textlogMesh.text = GameManager.Instance.textlog;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
