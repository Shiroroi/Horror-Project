using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ChutesAIClient : MonoBehaviour
{
    public static ChutesAIClient Instance;

    [Header("Chutes settings")]
    [Tooltip("Chutes API here")]
    public string apiKey = "Chutes API Key";

    [Tooltip("Chutes chat completions endpoint (default).")]
    public string endpoint = "https://llm.chutes.ai/v1/chat/completions";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SendChatRequest(string modelName, List<ChatMessage> history, Action<string> callback)
    {
        StartCoroutine(SendRequestCoroutine(modelName, history, callback));
    }

    IEnumerator SendRequestCoroutine(string modelName, List<ChatMessage> history, Action<string> callback)
    {
        var body = new ChatRequest
        {
            model = modelName,
            messages = history
        };

        string json = JsonUtility.ToJson(body);

        using (UnityWebRequest req = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool requestFailed = req.result != UnityWebRequest.Result.Success;
#else
            bool requestFailed = req.isNetworkError || req.isHttpError;
#endif

            if (requestFailed)
            {
                Debug.LogError("ChutesAI request failed: " + req.error + " | Response: " + req.downloadHandler.text);
                callback?.Invoke("ERROR: " + req.error);
                yield break;
            }

            try
            {
                var response = JsonUtility.FromJson<ChatResponse>(req.downloadHandler.text);

                if (response != null && response.choices != null && response.choices.Length > 0 && response.choices[0].message != null)
                {
                    string content = response.choices[0].message.content;
                    callback?.Invoke(content);
                }
                else
                {
                    Debug.LogWarning("ChutesAI returned unexpected JSON: " + req.downloadHandler.text);
                    callback?.Invoke("ERROR: Unexpected response format");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to parse ChutesAI response: " + ex.Message + "\nRaw: " + req.downloadHandler.text);
                callback?.Invoke("ERROR: parse failure");
            }
        }
    }

    [Serializable]
    class ChatRequest
    {
        public string model;
        public List<ChatMessage> messages;
    }

    [Serializable]
    class ChatResponse
    {
        public ChatChoice[] choices;
    }

    [Serializable]
    class ChatChoice
    {
        public ChatMessage message;
    }
}
