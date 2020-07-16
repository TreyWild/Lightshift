using Michsky.UI.ModernUIPack;
using Mirror;
using SharedModels.Models.User;
using System;
using TMPro;
using UnityEngine;
using static LightshiftAuthenticator;

public class LoginManager : MonoBehaviour
{

    public static LoginManager Instance;

    public bool UseTestServers;

    public WindowManager windowManager;

    [Header("Login")]
    public TMP_InputField _loginEmail;
    public TMP_InputField _loginPassword;

    [Header("Register")]
    public TMP_InputField _registerEmail;
    public TMP_InputField _registerUsername;
    public TMP_InputField _registerPassword;
    public TMP_InputField _registerConfirmPassword;

    [Header("Recover")]
    public TMP_InputField _recoverEmail;

    [SerializeField] private GameObject _networkManagerPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }

        _networkManagerPrefab = Instantiate(_networkManagerPrefab);
        if (Application.isEditor || UseTestServers)
        {
            NetworkManager.singleton.networkAddress = "localhost";
            if (Application.isEditor)
                _networkManagerPrefab.AddComponent<NetworkManagerHUD>();
        }
        else NetworkManager.singleton.networkAddress = "167.99.149.84";
    }

    private void Start()
    {
        _loginEmail.text = PlayerPrefs.GetString("login.email", "");
        _loginPassword.text = PlayerPrefs.GetString("login.password", "");

        DB.Accounts.SaveObject(new Account { emailAddress = "play@velcer.net", Id = Guid.NewGuid().ToString(), password = "asddfgh", username = "Velcer"});
    }

    public void Login() 
    {
        var email = _loginEmail.text;
        var password = _loginEmail.text;

        if (email == "" || !email.Contains("@"))
        {
            DialogManager.ShowMessage("Please enter a valid email address.");
            return;
        }

        if (password == "")
        {
            DialogManager.ShowMessage("Please enter a valid password.");
            return;
        }

        // TO DO : LOGIN
        LightshiftNetworkManager.Login(email, password);
    }

    public void Register() 
    {
        var email = _registerEmail.text;
        var username = _registerUsername.text;
        var password = _registerPassword.text;
        var confirmPassword = _registerConfirmPassword.text;

        if (email == "" || !email.Contains("@")) 
        {
            DialogManager.ShowMessage("Please enter a valid email address.");
            return;
        }

        if (password != confirmPassword) 
        {
            DialogManager.ShowMessage("Please ensure that the passwords you entered match! This is to ensure you don't accidentally mistype your password.");
            return;
        }

        if (password.Length < 6)
        {
            DialogManager.ShowMessage("Your password must be at least 6 characters long.");
            return;
        }

        // TO DO : REGISTER

        ShowLoading();
    }

    public void Recover() 
    {
        var email = _registerEmail.text;

        if (email == "" || !email.Contains("@"))
        {
            DialogManager.ShowMessage("Please enter a valid email address.");
            return;
        }

        ShowLoading();
    }

    public void ShowLoading() 
    {
        windowManager.OpenPanel("Loading");
    }

    public void HandleResponse(AuthResponse response)
    {
        switch (response)
        {
            case AuthResponse.LoginFail:
                DialogManager.ShowDialog("Invalid login details.");
                windowManager.OpenPanel("Login");
                break;
            case AuthResponse.EmailTaken:
                DialogManager.ShowDialog("That email is already in use.");
                windowManager.OpenPanel("Register");
                break;
            case AuthResponse.UsernameTaken:
                DialogManager.ShowDialog("That username is taken!");
                windowManager.OpenPanel("Register");
                break;
            case AuthResponse.RecoverSuccess:
                DialogManager.ShowDialog("Account found! Please check your email.");
                windowManager.OpenPanel("Login");
                break;
            case AuthResponse.RecoverFail:
                DialogManager.ShowDialog("That email does not exist.");
                windowManager.OpenPanel("Recover");
                break;
            case AuthResponse.Banned:
                DialogManager.ShowDialog("You are banned.");
                windowManager.OpenPanel("Login");
                break;
        }
    }
}
