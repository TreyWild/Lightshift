using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using PlayerIOClient;

public class ServerManager : MonoBehaviour
{
    public DatabaseConnection Database;
    private List<Player> _players = new List<Player>();
    public Player GetPlayer(NetworkConnection connection) 
    {
        return _players.FirstOrDefault(p => p.connection == connection);
    }
    public static ServerManager Instance { get; set; }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        InitMessageHandlers();

        Database = new DatabaseConnection();

        NetworkManager.singleton.StartServer();
    }

    public void InitMessageHandlers() 
    {
        NetworkServer.RegisterHandler<AuthMessage>(OnClientAuth);
        NetworkServer.RegisterHandler<ChatMessage>(OnChatMessageRecieved);
        NetworkServer.RegisterHandler<InitMessage>(OnInitGame);
    }

    private void OnInitGame(NetworkConnection client, InitMessage message)
    {
        var player = GetPlayer(client);
        if (player == null)
        {
            client.Disconnect();
            return;
        }

        client.Send(new InitMessage 
        {
            Username = player.Username,
        });

        var playershipObj = Instantiate(NetworkManager.singleton.playerPrefab);
        NetworkServer.AddPlayerForConnection(client, playershipObj);
        //Set the player as ready.
        NetworkServer.SetClientReady(client);
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

    private void OnClientAuth(NetworkConnection client, AuthMessage message)
    {

        Database.GetPlayerObject(message.connectUserId, delegate (DatabaseObject o)
        {
            if (o == null)
            {
                client.Disconnect();
                return;
            }

            var authKey = o.GetString("authKey");
            if (authKey != message.authKey)
            {
                client.Disconnect();
                return;
            }

            client.isAuthenticated = true;

            _players.Add(new Player
            {
                //username = message.username,
                connection = client,
                connectUserId = message.connectUserId,
                PlayerObject = o,
                Username = "TEST",
            });

            client.Send(new LoadSceneMessage { SceneName = "_GAME_" });
        });
    }

    public void OnPlayerDisconnect(NetworkConnection connection) 
    {
        var player = GetPlayer(connection);
        if (player != null)
            _players.Remove(player);

        NetworkServer.DestroyPlayerForConnection(connection);
    }
}
