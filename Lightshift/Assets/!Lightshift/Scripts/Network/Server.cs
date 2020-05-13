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
        if (player.connection != null)
            NetworkServer.DestroyPlayerForConnection(player.connection);
    }

    public static Player GetPlayer(NetworkConnection connection)
    {
        var networkIdentity =  connection.clientOwnedObjects.FirstOrDefault(o => o.GetType() == typeof(Player));
        var obj = networkIdentity.gameObject;
        var player = obj.GetComponent<Player>();
        return player;
    }
    public static void InitPlayer(NetworkConnection connection, AuthRequestMessage msg) 
    {
        var player = GetPlayer(connection);

        player.connectUserId = msg.userId;

        player.ConsumeAuthKey();

        player.InitPlayer();

        // Create Inventory
        var inventory = Instantiate(NetworkManager.singleton.spawnPrefabs[PrefabManager.INVENTORY_PREFAB_ID]);
        player.InventoryManager = inventory.GetComponent<InventoryManager>();
        NetworkServer.Spawn(inventory, player.connection);

        // Create Ship
        var ship = Instantiate(NetworkManager.singleton.spawnPrefabs[PrefabManager.PLAYER_SHIP_PREFAB_ID]);
        player.ship = inventory.GetComponent<PlayerShip>();
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
