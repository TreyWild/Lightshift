﻿using PlayerIOClient;
using PlayerIOConnect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerIONetwork : MonoBehaviour{

    [Tooltip("PlayerIO Game ID")]
    public string gameId = "";

    [Tooltip("Connects to local PlayerIO Test Server")]
    public bool useTestServer = true;
    public LoginUIManager loginUI;

    private Client _client;
    public Client Client
    {
        get { return _client; }
        set
        {
            if (useTestServer)
                value.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1", 8184);
            _client = value;
        }
    }

    public ServiceConnection ServiceConnection;

    public static PlayerIONetwork Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance.loginUI = loginUI;
            InitLoginUI();

            //If the instance is not null, we assume then that there was a disconnection.
            DialogManager.ShowMessage("Uh oh! We lost connection to the server! Check your internet connection and try again.");

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitLoginUI();
    }
    private void InitLoginUI()
    {
        loginUI.onLogin += (string email, string password) => Authenticate(email, password);
        loginUI.onRegister += (string email, string password) => Register(email, password);
        loginUI.onRecover += (email) => ForgotPassword(email);
        loginUI.onUsernameSubmit += (username) => ServiceConnection?.Send("username", "set", username);
    }

    public  void JoinServer(string authKey)
    {
        LightshiftNetworkManager.Authenticate(_client.ConnectUserId, authKey);

        ServiceConnection.Disconnect();
    }

    public void Authenticate(string email, string password)
    {
        loginUI.ShowMessage("Verifying Account...");
        PlayerIO.QuickConnect.SimpleConnect(gameId, email, password, null, delegate (Client client)
        {
            Client = client;
            loginUI.SetLoginPrefs(email, password);

            //Initialize new service connection
            ServiceConnection = new ServiceConnection();

            loginUI.ShowMessage("Connecting to Game Server...");

        }, delegate (PlayerIOError error)
        {
            Debug.Log(error);
            loginUI.ShowError(error.Message);
            loginUI.Loading = false;
        });
    }

    public void Register(string email, string password)
    {
        loginUI.ShowMessage("Creating Account...");
        PlayerIO.QuickConnect.SimpleRegister(gameId, Guid.NewGuid().ToString(), password, email, null, null, null, null, null, delegate (Client client)
        {
            Client = client;
            loginUI.SetLoginPrefs(email, password);

            loginUI.ShowMessage("Connecting to Game Server...");

            //Initialize new service connection
            ServiceConnection = new ServiceConnection();

        }, delegate (PlayerIORegistrationError error)
        {
            Debug.Log(error);
            loginUI.ShowError(error.Message);
            loginUI.Loading = false;
        });
    }

    public void ForgotPassword(string email)
    {
        loginUI.ShowMessage("Surfing the waves...");
        PlayerIO.QuickConnect.SimpleRecoverPassword(gameId, email, delegate
        {
            loginUI.Loading = false;
            loginUI.Back();
            loginUI.ShowMessage("Please check your Email address.");
        }, delegate
        {
            loginUI.Loading = false;
            loginUI.Back();
            loginUI.ShowMessage("Invalid email.");
        });
    }




}
