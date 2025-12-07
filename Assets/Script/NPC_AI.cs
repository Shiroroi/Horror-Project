using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPC_AI : MonoBehaviour
{
    public string npcName = "NPC";
    [TextArea]
    public string personalityPrompt = "You are a friendly NPC. Answer concisely.";
    public float interactRange = 2f;

    private Transform player;
    private AIChatUI chatUI;
    private List<ChatMessage> history = new List<ChatMessage>();

    void Start()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) player = playerGO.transform;

        chatUI = FindAnyObjectByType<AIChatUI>();
    }

    void Update()
    {
        if (player == null) return;

        // Prevent interaction while chat is open
        if (AIChatUI.IsChatOpen) return;

        float dist = Vector2.Distance(player.position, transform.position);

        if (dist < interactRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartChat();
            }
        }
    }

    void StartChat()
    {
        history.Clear();
        history.Add(new ChatMessage("system", personalityPrompt));

        chatUI.Open(npcName, OnPlayerSend);
    }

    void OnPlayerSend(string playerText)
    {
        if (string.IsNullOrWhiteSpace(playerText)) return;

        history.Add(new ChatMessage("user", playerText));

        // New typing indicator
        chatUI.ShowTyping(true, npcName);

        string modelToUse = "deepseek-ai/DeepSeek-V3.2-Exp";
        var modelSettings = FindAnyObjectByType<ChatModelSettings>();
        if (modelSettings != null && !string.IsNullOrEmpty(modelSettings.modelName))
            modelToUse = modelSettings.modelName;

        ChutesAIClient.Instance.SendChatRequest(modelToUse, history, (reply) =>
        {
            chatUI.ShowTyping(false);

            history.Add(new ChatMessage("assistant", reply));

            chatUI.AddMessage(npcName, reply);
        });
    }
}