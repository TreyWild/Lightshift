using Assets._Lightshift.Scripts;
using Assets._Lightshift.Scripts.Web;
using MasterServer;
using Mirror;
using PlayerIOClient;
using SharedModels;
using SharedModels.WebRequestObjects;
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
    public bool UseTestServers;
    public bool UseHostFormat = true;
    public bool IsServer = true;
    public static GameObject GetPrefab<T>() => singleton.spawnPrefabs.FirstOrDefault(o => o.gameObject.HasType<T>());
    public override void OnStartServer()
    {
        base.OnStartServer();
        spawnPrefabs = Resources.LoadAll<GameObject>("Prefabs/Networked").ToList();
        gameObject.AddComponent<Server>();
        Debug.Log($"Server Started. Running on {networkAddress}.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        var prefabs = Resources.LoadAll<GameObject>("Prefabs/Networked").ToList();

        foreach (var prefab in prefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.Log($"Client with ID [{conn.connectionId}] connected.");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        Debug.Log($"Game Scene Loaded. Server Ready.");
    }
    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    base.OnClientConnect(conn);
    //}

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        //Destroy(gameObject);
        //SceneManager.LoadScene("_AUTHENTICATION_");
        singleton.StopClient();
    }
    //public override void OnServerAddPlayer(NetworkConnection conn)
    //{
    //    Delegator.WaitForEndOfFrame(delegate
    //    {
    //        base.OnServerAddPlayer(conn);
    //    });
    //}

    public void Authenticate(string key)
    {
        var authenticator = FindObjectOfType<LightshiftAuthenticator>();
        authenticator.sessionAuthKey = key;

        if (UseTestServers)
        {
            singleton.networkAddress = "localhost";
        }
        else singleton.networkAddress = "167.99.149.84";

        if (IsServer)
        {
            HttpService.InitGameServerAuthentication("dev-access");

            if (UseHostFormat)
                StartHost();
            else
                StartServer();
        }
        else StartClient();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Server.RemovePlayer(Server.GetPlayer(conn));
    }
}