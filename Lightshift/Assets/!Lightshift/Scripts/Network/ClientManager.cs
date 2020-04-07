using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections;
using System;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance { get; set; }
    public void Awake()
    {
        if (Instance == null)
            Instance = this;    
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        StartCoroutine(TryJoinServer());

        InitializeMessageHandlers();
    }

    public void InitializeMessageHandlers() 
    {
        NetworkClient.RegisterHandler<AlertMessage>(OnAlert);
        NetworkClient.RegisterHandler<ChatMessage>(OnChatMessage);
        NetworkClient.RegisterHandler<InitMessage>(OnInitGame);
        NetworkClient.RegisterHandler<LoadSceneMessage>(OnLoadScene);
    }

    private void OnLoadScene(NetworkConnection connection, LoadSceneMessage message)
    {
        //SceneManager.LoadSceneAsync("_GAME_");
    }

    private void OnAlert(NetworkConnection connection, AlertMessage message)
    {
        if (message.IsPopup)
        {
            DialogManager.ShowMessage(message.Message);
            return;
        }
        else ChatBox.Instance.AddMessage($"<color=red>SYSTEM: {message.Message}</color>");
    }

    private void OnInitGame(NetworkConnection connection, InitMessage message)
    {
        //throw new NotImplementedException();
    }

    private void OnChatMessage(NetworkConnection connection, ChatMessage chatObj)
    {
        ChatBox.Instance.AddMessage($"{chatObj.username}: {chatObj.message}");
    }


    public void InitGame()
    {
        NetworkClient.Send(new InitMessage());
    }

    private IEnumerator TryJoinServer()
    {
        NetworkManager.singleton.StartClient();

        yield return new WaitForSeconds(1f);

        if (!NetworkManager.singleton.isNetworkActive)
            StartCoroutine(TryJoinServer());
    }

    #region Authentication

    private string _userId;
    private string _authKey;
    public void SetAuth(string userId, string authKey)
    {
        _userId = userId;
        _authKey = authKey;
    }


    public void Authenticate()
    {
        NetworkClient.Send(new AuthMessage { connectUserId = _userId, authKey = _authKey });
    }
    #endregion 
}
