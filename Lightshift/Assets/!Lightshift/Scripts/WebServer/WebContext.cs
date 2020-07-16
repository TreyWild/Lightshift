using Proyecto26;
using System;
using UnityEngine;

public class WebContext
{
    public string controller;
    public WebContext(string controller)
    {
        this.controller = controller;
    }

    public void SaveObject<T>(T document)
    {
        Debug.Log(JsonUtility.ToJson(document));
        
        RestClient.Post<T>($"{DB.serverPath}{controller}", document).Then(response => {
            Debug.Log(response.ToString());
        });
    }

    public void GetObject<T>(string id, Action<T> callback)
    {
        RestClient.Get<T>($"{DB.serverPath}{controller}").Then(response => {
            callback(response);
        });
    }
}
