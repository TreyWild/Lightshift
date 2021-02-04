using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using PlayerIOClient;
using Mirror.Authenticators;
using static LightshiftAuthenticator;
using SharedModels;
public class Server : MonoBehaviour
{
    public static Server Instance { get; set; }

    private static List<Player> _players = new List<Player>();
    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        InitMessageHandlers();

        NetworkServer.SpawnObjects();
    }

    public void InitMessageHandlers() 
    {
        NetworkServer.RegisterHandler<ChatMessage>(OnChatMessageRecieved, true) ;
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

        player.SaveAccount();

        if (player.GetConnection() != null)
            NetworkServer.DestroyPlayerForConnection(player.GetConnection());
    }

    public static Player GetPlayer(NetworkConnection connection)
    {
        return _players.FirstOrDefault(p => p.GetConnection() == connection);
    }

    public static void AddPlayer(Player player) 
    {
        if (!_players.Contains(player))
            _players.Add(player);
    }
    //public static void InitPlayer(NetworkConnection connection, Account account)
    //{
    //    //Create Player
    //    var player = Instantiate(LightshiftNetworkManager.GetPrefab<Player>()).GetComponent<Player>();



    //    NetworkServer.AddPlayerForConnection(connection, player.gameObject);

    //    //// Init Player
    //    //player.InitPlayer(connection, account);

    //    AddPlayer(player);

    //    Debug.LogError($"Ready: {connection.isReady}");


    //    // Create Inventory
    //    //var inventory = Instantiate(LightshiftNetworkManager.GetPrefab<InventoryManager>());
    //    ////player.InventoryManager = inventory.GetComponent<InventoryManager>();
    //    //NetworkServer.Spawn(inventory, player.GetConnection());

    //    // Create Ship
    //    //var ship = Instantiate(LightshiftNetworkManager.GetPrefab<PlayerShip>());
    //    //player.ship = ship.GetComponent<PlayerShip>();
    //    //NetworkServer.Spawn(ship, player.GetConnection());
    //}

    public static void SendChatBroadcast(string message) 
    {
        NetworkServer.SendToAll(new ChatMessage
        {
            message = message,
            username = "* SYSTEM"
        });
    }
}
