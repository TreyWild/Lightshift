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

    private List<Player> _players;

    private void OnDestroy() 
    {
        _players = null;
        Instance = null;
    }
    public void Awake()
    {
        Instance = this;
        _players = new List<Player>();
    }
    public static void RemovePlayer(Player player)
    {
        if (Instance._players.Contains(player))
            Instance._players.Remove(player);

        player.SaveAccount();
    }

    public static Player GetPlayer(NetworkConnection connection)
    {
        return Instance._players.FirstOrDefault(p => p.GetConnection() == connection);
    }

    public static Player GetPlayer(string id)
    {
        return Instance._players.FirstOrDefault(p => p.Id.ToLower() == id.ToLower());
    }

    public static void AddPlayer(Player player) 
    {
        if (!Instance._players.Contains(player))
            Instance._players.Add(player);
    }

    public static List<Player> GetAllPlayers() 
    {
        return Instance._players;
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

}
