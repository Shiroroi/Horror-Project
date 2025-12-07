using UnityEngine;

public class ChatModelSettings : MonoBehaviour
{
    public static ChatModelSettings Instance;

    [Header("Available models:")]
    public string modelName = "deepseek-ai/DeepSeek-V3.2-Exp";
    // Example options:
    // deepseek-ai/DeepSeek-R1
    // deepseek-ai/DeepSeek-V3.2-Exp
    // meta-llama/Llama-3.1-Chat
    // qwen/Qwen2.5-7B-Instruct
    // mistral/Mistral-Large

    void Awake()
    {
        Instance = this;
    }
}