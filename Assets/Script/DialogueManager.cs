using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueUI UI;

    private DialogueData currentData;
    private int index;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData data)
    {
        currentData = data;
        index = 0;

        if (currentData.useVNStyle)
            UI.ShowVN();
        else
            UI.ShowFloating();

        DisplayLine();
    }

    public void NextLine()
    {
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
}

