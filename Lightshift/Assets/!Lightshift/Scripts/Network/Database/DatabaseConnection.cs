using System;
using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;

public class DatabaseConnection
{
    private Client _client;

    public DatabaseConnection() 
    {
        var auth = new Dictionary<string, string>();
        auth["userId"] = "gameServer";
        auth["auth"] = PlayerIO.CalcAuth256("gameServer", "L1ghtSh1ft");

        PlayerIO.Authenticate("lightshift-cegvlcfmeqeewpktzbfka", "gameserver", auth, null, delegate (Client client) 
        {
            _client = client;
            Debug.Log("Database successfully connected.");
        }, delegate (PlayerIOError error) 
        {
            Debug.LogError(error);   
        });
    }

    public void GetPlayerObject(string userId, Action<DatabaseObject> callback) 
    {
        _client.BigDB.Load("PlayerObjects", userId, delegate (DatabaseObject o) 
        {
            callback(o);
        }, delegate (PlayerIOError error) 
        {
            Debug.LogError(error);
            callback(null);
        });
    }
}