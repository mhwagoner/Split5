using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignMessageManager : MonoBehaviour
{
    public Image signMessageSpellIcon;
    public TextMeshProUGUI signMessageText;

    public void ReadSignUI(string message, Sprite spellIcon)
    {
        this.transform.gameObject.SetActive(true);
        signMessageText.text = message;
        signMessageSpellIcon.sprite = spellIcon;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.signMessageManager = this;
    }
}
