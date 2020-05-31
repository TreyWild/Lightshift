using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using PlayerIOClient;
using Mirror.Authenticators;
using static LightshiftAuthenticator;

public class Server : MonoBehaviour
{
    public static DatabaseConnection Database;
    public static Server Instance { get; set; }

    private static List<Player> _players = new List<Player>();
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
        if (_players.Contains(player))
            _players.Remove(player);

        if (player.connection != null)
            NetworkServer.DestroyPlayerForConnection(player.connection);
    }

    public static Player GetPlayer(NetworkConnection connection)
    {
        return _players.FirstOrDefault(p => p.connection == connection);
    }

    private static void AddPlayer(Player player) 
    {
        if (!_players.Contains(player))
            _players.Add(player);
    }
    public static void InitPlayer(NetworkConnection connection, AuthRequestMessage msg, DatabaseObject playerObject)
    {
        //Create Player
        var player = Instantiate(LightshiftNetworkManager.GetPrefab<Player>()).GetComponent<Player>();

        // Init Player
        player.connection = connection;
        player.connectUserId = msg.userId;
        player.PlayerObject = playerObject;
        player.ConsumeAuthKey();
        player.InitPlayer();

        AddPlayer(player);

        NetworkServer.AddPlayerForConnection(connection, player.gameObject);
        
        // Create Inventory
        var inventory = Instantiate(LightshiftNetworkManager.GetPrefab<InventoryManager>());
        player.InventoryManager = inventory.GetComponent<InventoryManager>();
        NetworkServer.Spawn(inventory, player.connection);

        // Create Ship
        var ship = Instantiate(LightshiftNetworkManager.GetPrefab<PlayerShip>());
        player.ship = ship.GetComponent<PlayerShip>();
        NetworkServer.Spawn(ship, player.connection);
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
