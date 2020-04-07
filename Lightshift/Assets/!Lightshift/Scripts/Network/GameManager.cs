using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _isServer;
    private bool _isClient;
    private void Start()
    {
        //Tell server we're ready.
        if (ClientManager.Instance != null)
        {
            ClientManager.Instance.InitGame();
        }
        else _isClient = true;

        if (ServerManager.Instance != null)
            _isServer = true;


        if (_isServer)
            InitSolarSystem();
    }

    public void InitSolarSystem() 
    {
        NetworkServer.SpawnObjects();
    }
}
