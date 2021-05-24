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
    public bool DevMode = true;
    public bool IsServerBuild = false;
    public static GameObject GetPrefab<T>() => singleton.spawnPrefabs.FirstOrDefault(o => o.gameObject.HasType<T>());
    public override void OnStartServer()
    {
        base.OnStartServer();
        spawnPrefabs = Resources.LoadAll<GameObject>("Prefabs/Networked").Where(s => s.GetComponent<Player>() == null).ToList();
        gameObject.AddComponent<Server>();
        Debug.Log($"Server Started. Running on {networkAddress}.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        var prefabs = Resources.LoadAll<GameObject>("Prefabs/Networked").Where(s => s.GetComponent<Player>() == null).ToList();

        foreach (var prefab in prefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }
    public void Authenticate(string key)
    {
        var authenticator = FindObjectOfType<LightshiftAuthenticator>();
        authenticator.sessionAuthKey = key;

        if (DevMode)
        {
            singleton.networkAddress = "localhost";
            var selector = DialogManager.ShowNetworkClientSelector();
            selector.OnClickClient += () => StartClient();
            selector.OnClickHost += () =>
            {
                HttpService.InitGameServerAuthentication("dev-access");

                StartHost();
            };
            selector.OnClickServer += () =>
            {
                HttpService.InitGameServerAuthentication("dev-access");

                StartServer();
            };
        }
        else if (!IsServerBuild)
        {
            singleton.networkAddress = "167.99.149.84";
            StartClient();
        }
        else StartServer();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Server.RemovePlayer(Server.GetPlayer(conn));
    }
}