using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueUI UI;

    private DialogueData currentData;
    private int index;

    // For AI mode
    private List<ChatMessage> memory = new List<ChatMessage>();

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData data)
    {
        currentData = data;

        if (currentData.isAIConversation)
        {
            StartAIConversation();
            return;
        }

        // NORMAL MODE
        index = 0;

        if (currentData.useVNStyle)
            UI.ShowVN();
        else
            UI.ShowFloating();

        UI.ShowInput(false);
        DisplayLine();
    }

    private void StartAIConversation()
    {
        memory = new List<ChatMessage>();

        // Optional system prompt/personality
        if (!string.IsNullOrEmpty(currentData.npcSystemPrompt))
        {
            memory.Add(new ChatMessage("system", currentData.npcSystemPrompt));
        }

        if (currentData.useVNStyle)
            UI.ShowVN();
        else
            UI.ShowFloating();

        UI.ShowInput(true);
    }

    // ------------------------------ NORMAL MODE ------------------------------
    public void NextLine()
    {
        if (currentData.isAIConversation) return;

        index++;

        if (index >= currentData.lines.Length)
        {
            UI.HideAll();
            return;
        }

        DisplayLine();
    }

    private void DisplayLine()
    {
        var line = currentData.lines[index];

        if (currentData.useVNStyle)
            UI.UpdateVN(line.speakerName, line.line, line.portrait);
        else
            UI.UpdateFloating(line.line);
    }

    // ------------------------------ AI MODE ------------------------------

    public void PlayerSentMessage(string text)
    {
        if (!currentData.isAIConversation) return;

        memory.Add(new ChatMessage("user", text));

        if (currentData.useVNStyle)
            UI.UpdateVN("You", text, null);
        else
            UI.UpdateFloating(text);

        StartCoroutine(GetAIResponse());
    }

    IEnumerator GetAIResponse()
    {
        string jsonBody = JsonUtility.ToJson(new ChutesRequest { messages = memory });

        using (UnityWebRequest req = new UnityWebRequest("https://api.chutes.ai/chat", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer YOUR_API_KEY");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                UI.UpdateFloating("Error: " + req.error);
                yield break;
            }

            var response = JsonUtility.FromJson<ChutesResponse>(req.downloadHandler.text);

            string aiText = response.reply;

            memory.Add(new ChatMessage("assistant", aiText));

            if (currentData.useVNStyle)
                UI.UpdateVN(currentData.npcID, aiText, null);
            else
                UI.UpdateFloating(aiText);
        }
    }
}

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string content;

    public ChatMessage(string r, string c)
    {
        role = r;
        content = c;
    }
}

[System.Serializable]
public class ChutesRequest
{
    public List<ChatMessage> messages;
}

[System.Serializable]
public class ChutesResponse
{
    public string reply;
}
