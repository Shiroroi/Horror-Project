using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AIChatUI : MonoBehaviour
{
    public static bool IsChatOpen = false;

    public GameObject panel;
    public TMP_Text chatLog;
    public TMP_InputField inputField;

    private System.Action<string> sendCallback;
    private string npcName;

    public ScrollRect scrollRect;
    // Tag to identify the typing line
    private string typingTag = "<typing-line>";

    void Update()
    {
        if (!IsChatOpen) return;

        // ESC = close chat
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        // ENTER = send, SHIFT+ENTER = new line
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                InsertNewLine();
            }
            else
            {
                OnSendButton();
            }
        }
    }

    public void Open(string npcName, System.Action<string> onSend)
    {
        panel.SetActive(true);

        Time.timeScale = 0f;
        IsChatOpen = true;

        this.npcName = npcName;
        this.sendCallback = onSend;

        chatLog.text = "";
        inputField.text = "";

        inputField.Select();
        inputField.ActivateInputField();
    }

    public void Close()
    {
        panel.SetActive(false);

        Time.timeScale = 1f;
        IsChatOpen = false;
    }

    

    public void AddMessage(string sender, string message)
    {
        RemoveTypingLine();
        chatLog.text += $"<b>{sender}:</b> {message}\n\n";

        LayoutRebuilder.ForceRebuildLayoutImmediate(chatLog.rectTransform);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }




    // NEW TYPING SYSTEM
    public void ShowTyping(bool show, string npcName = "")
    {
        if (show)
        {
            RemoveTypingLine();
            chatLog.text += $"<i>{npcName} is typing…</i>{typingTag}\n";
        }
        else
        {
            RemoveTypingLine();
        }
    }

    private void RemoveTypingLine()
    {
        // Find the tagged typing line and remove the entire line
        int index = chatLog.text.IndexOf(typingTag);
        if (index != -1)
        {
            // Find the beginning of that line
            int lineStart = chatLog.text.LastIndexOf('\n', index);
            if (lineStart == -1) lineStart = 0;

            // Find end of line
            int lineEnd = chatLog.text.IndexOf('\n', index);
            if (lineEnd == -1) lineEnd = chatLog.text.Length - 1;

            chatLog.text = chatLog.text.Remove(lineStart, (lineEnd - lineStart) + 1);
        }
    }


    public void OnSendButton()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        string playerMessage = inputField.text;
        inputField.text = "";

        AddMessage("You", playerMessage);

        sendCallback?.Invoke(playerMessage);

        inputField.Select();
        inputField.ActivateInputField();
    }

    private void InsertNewLine()
    {
        int caret = inputField.stringPosition;

        string before = inputField.text.Substring(0, caret);
        string after = inputField.text.Substring(caret);

        inputField.text = before + "\n" + after;

        inputField.stringPosition = caret + 1;
        inputField.caretPosition = caret + 1;

        inputField.Select();
        inputField.ActivateInputField();
    }
}
