using Mirror;
using PlayerIOClient;
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
        Destroy(gameObject);
        SceneManager.LoadScene("_AUTHENTICATION_");
    }

    public static void Register(string email, string username, string password) 
    {
        var authenticator = FindObjectOfType<LightshiftAuthenticator>();
        authenticator.authType = LightshiftAuthenticator.AuthType.Register;
        authenticator.email = email;
        authenticator.username = username;
        authenticator.passwordHash = PasswordHasher.Hash(password);

        singleton.StartClient();
    }

    public static void Login(string email, string password)
    {
        var authenticator = FindObjectOfType<LightshiftAuthenticator>();
        authenticator.authType = LightshiftAuthenticator.AuthType.Login;
        authenticator.email = email;
        authenticator.passwordHash = PasswordHasher.Hash(password);

        singleton.StartClient();
    }

    public static void Recover(string email)
    {
        var authenticator = FindObjectOfType<LightshiftAuthenticator>();
        authenticator.authType = LightshiftAuthenticator.AuthType.Recover;
        authenticator.email = email;

        singleton.StartClient();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Server.RemovePlayer(Server.GetPlayer(conn));
    }
}