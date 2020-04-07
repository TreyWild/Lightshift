using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightshiftNetworkManager : NetworkManager 
{
    public override void OnStartServer()
    {
        Debug.Log($"Server Started. Running on {networkAddress}.");

        ServerChangeScene("_GAME_");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log($"Client with ID [{conn.connectionId}] connected.");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        Debug.Log($"Game Scene Loaded. Server Ready.");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientManager.Instance.Authenticate();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        SceneManager.LoadScene("_AUTHENTICATION_");

        Destroy(gameObject);
    }
}