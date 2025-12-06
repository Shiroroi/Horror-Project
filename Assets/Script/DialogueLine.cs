using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea] public string line;
    public Sprite portrait;  // Only used in VN style
}
