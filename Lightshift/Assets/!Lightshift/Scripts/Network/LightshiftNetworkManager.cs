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
        gameObject.AddComponent<Server>();
        Debug.Log($"Server Started. Running on {networkAddress}.");
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

        gameObject.AddComponent<Game>();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Destroy(gameObject);
        SceneManager.LoadScene("_AUTHENTICATION_");
    }

    public static void Authenticate(string connectUserId, string authKey) 
    {
        var authenticator = FindObjectOfType<LightshiftAuthenticator>();
        authenticator.userId = connectUserId;
        authenticator.authKey = authKey;

        singleton.StartClient();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Server.RemovePlayer(Server.GetPlayer(conn));
    }
}