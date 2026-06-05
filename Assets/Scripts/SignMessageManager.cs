using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignMessageManager : MonoBehaviour
{
    public Image signMessageSpellIcon;
    public TextMeshProUGUI signMessageText;

    public void ReadSignUI(string message, Sprite spellIcon)
    {
        signMessageText.text = message;
        signMessageSpellIcon.sprite = spellIcon;
        this.transform.Find("TextPanel").gameObject.SetActive(true);
        this.transform.Find("SpellPanel").gameObject.SetActive(true);
    }

    void Start()
    {
        GameManager.Instance.signMessageManager = this;
        this.transform.Find("TextPanel").gameObject.SetActive(false);
        this.transform.Find("SpellPanel").gameObject.SetActive(false);
    }
}
