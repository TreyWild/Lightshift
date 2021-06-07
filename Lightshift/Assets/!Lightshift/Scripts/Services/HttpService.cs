using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets._Lightshift.Scripts.Web
{
    public class HttpService : MonoBehaviour
    {
        public static HttpService Instance {get;set;}

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance);
            Instance = this;
            DontDestroyOnLoad(this);
        }
        private const string WEB_SERVER_URL_MAIN = "http://67.211.213.146:5000";
        public const string WEB_SERVER_URL_TESTING = "http://localhost:5000";
        private static string AUTHKEY = "";
        public static void InitGameServerAuthentication(string authKey) 
        {
            AUTHKEY = authKey;
            RestClient.DefaultRequestHeaders["auth"] = authKey;
        }
        public static void Get<T>(string uri, object document, Action<T> callback, Action<Exception> error = null)
        {
            RestClient.Post($"{WEB_SERVER_URL_MAIN}/{uri}", JsonConvert.SerializeObject(document)).Then(response =>
            {
                var json = response.Text;
                callback?.Invoke(JsonConvert.DeserializeObject<T>(json));
            }, delegate (Exception e) 
            {
                Debug.LogError(e.Message);
                error?.Invoke(e);
            });
        }

        public static void Get2<T>(string uri, object document, Action<T> callback, Action<Exception> error = null)
        {
            Instance.StartCoroutine(getRequest(uri, document, callback, error));
        }

        private static IEnumerator getRequest<T>(string uri, object document, Action<T> callback, Action<Exception> error = null)
        {
            //Make request
            UnityWebRequest uwr = UnityWebRequest.Post(uri, JsonConvert.SerializeObject(document));
            uwr.SetRequestHeader("auth", AUTHKEY);

            yield return uwr.SendWebRequest();

            if (uwr.isHttpError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
                var json = uwr.downloadHandler.text;
                callback?.Invoke(JsonConvert.DeserializeObject<T>(json));             
            }
        }
    }
}
