using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using PlayerIOClient;
using Mirror.Authenticators;

public class Server : MonoBehaviour
{
    public static DatabaseConnection Database;

    private List<Player> _players = new List<Player>();
    public static Server Instance { get; set; }
    public void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        InitMessageHandlers();

        Database = new DatabaseConnection();

        NetworkServer.SpawnObjects();
    }

    public void InitMessageHandlers() 
    {
        NetworkServer.RegisterHandler<ChatMessage>(OnChatMessageRecieved, true);
    }

    private void OnChatMessageRecieved(NetworkConnection connection, ChatMessage chatMessage)
    {
        var player = GetPlayer(connection);
        if (player != null) 
        {
            chatMessage.username = player.Username;
            NetworkServer.SendToAll(chatMessage);
        }
    }
    public static void RemovePlayer(Player player)
    {
        if (player != null)
            Instance._players.Remove(player);

        if (player.connection != null)
            NetworkServer.DestroyPlayerForConnection(player.connection);
    }

    public static Player GetPlayer(NetworkConnection connection)
    {
        return Instance._players.FirstOrDefault(p => p.connection == connection);
    }
    public static void AddPlayer(Player player) 
    {
        if (!Instance._players.Contains(player))
            Instance._players.Add(player);
    }

    public static void SendChatBroadcast(string message) 
    {
        NetworkServer.SendToAll(new ChatMessage
        {
            message = message,
            username = "* SYSTEM"
        });
    }
}
