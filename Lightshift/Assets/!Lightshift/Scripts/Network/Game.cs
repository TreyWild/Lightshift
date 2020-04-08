using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections;
using System;

public class Game : MonoBehaviour
{
    public static Game Instance { get; set; }
    public void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        InitializeMessageHandlers();
    }

    public void InitializeMessageHandlers() 
    {
        NetworkClient.RegisterHandler<AlertMessage>(OnAlert);
        NetworkClient.RegisterHandler<ChatMessage>(OnChatMessage);
    }

    private void OnAlert(NetworkConnection connection, AlertMessage message)
    {
        if (message.IsPopup)
        {
            DialogManager.ShowMessage(message.Message);
            return;
        }
        else ChatBox.Instance.AddMessage($"<color=red>SYSTEM: {message.Message}</color>");
    }

    private void OnChatMessage(NetworkConnection connection, ChatMessage chatObj)
    {
        ChatBox.Instance.AddMessage($"{chatObj.username}: {chatObj.message}");
    }
}
