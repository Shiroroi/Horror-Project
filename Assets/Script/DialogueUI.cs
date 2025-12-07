using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("Floating")]
    public GameObject floatingPanel;
    public TMP_Text floatingText;

    [Header("VN")]
    public GameObject vnPanel;
    public TMP_Text vnName;
    public TMP_Text vnText;
    public Image vnPortrait;
    
    [Header("Player Input")]
    public TMP_InputField playerInput;
    public GameObject inputPanel;

    public void ShowInput(bool show)
    {
        inputPanel.SetActive(show);
        if (show)
            playerInput.text = "";
    }

    public void OnSendMessage()
    {
        DialogueManager.Instance.PlayerSentMessage(playerInput.text);
        playerInput.text = "";
    }


    public void ShowFloating()
    {
        floatingPanel.SetActive(true);
    }

    public void ShowVN()
    {
        vnPanel.SetActive(true);
    }

    public void HideAll()
    {
        floatingPanel.SetActive(false);
        vnPanel.SetActive(false);
    }

    public void UpdateFloating(string text)
    {
        floatingText.text = text;
    }

    public void UpdateVN(string name, string text, Sprite portrait)
    {
        vnName.text = name;
        vnText.text = text;
        vnPortrait.sprite = portrait;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DialogueManager.Instance.NextLine();
        }
    }
}