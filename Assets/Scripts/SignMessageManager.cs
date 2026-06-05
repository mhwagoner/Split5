using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignMessageManager : MonoBehaviour
{
    public Image signMessageSpellIcon;
    public TextMeshProUGUI signMessageText;

    public void ReadSignUI(string message, Sprite spellIcon)
    {
        if (message == "") {
            signMessageText.text = "The sign's author forgot to write a message.";
        } else {
            signMessageText.text = message;
        }

        if (spellIcon == null) {
            this.transform.Find("SpellPanel").gameObject.SetActive(false);
        } else {
            signMessageSpellIcon.sprite = spellIcon;
            this.transform.Find("SpellPanel").gameObject.SetActive(true);
        }
        this.transform.gameObject.SetActive(true);
    }

    public void HideSignUI()
    {
        this.transform.gameObject.SetActive(false);
    }

    void Awake()
    {
        GameManager.Instance.signMessageManager = this;
        this.transform.gameObject.SetActive(false);
    }
}
