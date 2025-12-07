using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public bool useVNStyle;

    public bool isAIConversation;
    public string npcID;
    [TextArea] public string npcSystemPrompt;

    public DialogueLine[] lines;
}