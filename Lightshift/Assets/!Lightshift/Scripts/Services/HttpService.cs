using Newtonsoft.Json;
using Proyecto26;
using SharedModels.WebRequestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets._Lightshift.Scripts.Web
{
    public static class HttpService
    {
        public const string WEB_SERVER_URL = "http://localhost:50413";
        public static void InitGameServerAuthentication(string authKey) 
        {
            RestClient.DefaultRequestHeaders["auth"] = authKey;
        }
        public static void Get<T>(string uri, object document, Action<T> callback, Action<Exception> error = null)
        {
            RestClient.Post($"{WEB_SERVER_URL}/{uri}", JsonConvert.SerializeObject(document)).Then(response =>
            {
                var json = response.Text;
                callback?.Invoke(JsonConvert.DeserializeObject<T>(json));
            }, delegate (Exception e) 
            {
                Debug.LogError(e.Message);
                error?.Invoke(e);
            });
        }
    }
}
