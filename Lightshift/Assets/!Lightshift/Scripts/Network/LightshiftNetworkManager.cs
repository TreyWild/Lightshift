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
    public bool Invert;
    public static GameObject GetPrefab<T>() => singleton.spawnPrefabs.FirstOrDefault(o => o.gameObject.HasType<T>());
    public override void OnStartServer()
    {
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
            ClientScene.RegisterPrefab(prefab);
        }
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
        //Destroy(gameObject);
        //SceneManager.LoadScene("_AUTHENTICATION_");
        singleton.StopClient();
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //Delegator.WaitForEndOfFrame(delegate 
        //{
            base.OnServerAddPlayer(conn);
        //});
    }

    public void Authenticate(string key)
    {
        var authenticator = FindObjectOfType<LightshiftAuthenticator>();
        authenticator.sessionAuthKey = key;

        if (Application.isEditor || UseTestServers)
        {
            singleton.networkAddress = "localhost";
            if (Application.isEditor && !Invert)
            {
                HttpService.InitGameServerAuthentication("dev-access");
                StartHost();
                return;
            }
            else if (Invert && Application.isEditor)
            {
                StartClient();
                return;
            }
            else 
            {
                HttpService.InitGameServerAuthentication("dev-access");
                StartHost();
                return;
            }
        }
        else singleton.networkAddress = "167.99.149.84";

        StartClient();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Server.RemovePlayer(Server.GetPlayer(conn));
    }
}