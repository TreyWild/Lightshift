using Assets._Lightshift.Scripts;
using Assets._Lightshift.Scripts.Web;
using MasterServer;
using Michsky.UI.ModernUIPack;
using Mirror;
using Newtonsoft.Json;
using Proyecto26;
using SharedModels;
using SharedModels.WebRequestObjects;
using System;
using TMPro;
using UnityEngine;
using static LightshiftAuthenticator;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

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

    [Header("Confirm Account")]
    public TMP_InputField _confirmCode;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        _loginEmail.text = PlayerPrefs.GetString("login.email", "");
        _loginPassword.text = PlayerPrefs.GetString("login.password", "");

        //Debug.Log("sending");
        //Web.Accounts.CheckEmailAvailability("test", delegate (bool result)
        //{
        //    Debug.Log(result);
        //});

        //var password = $"Vel99Cer99!";
        //var hashed = PasswordHasher.Hash(password);

        //if (PasswordHasher.Verify(password, hashed))
        //{
        //    DialogManager.ShowMessage("Works");
        //    Debug.LogError("Test");
        //}
        //else DialogManager.ShowMessage("Failed");

        //Debug.LogError("Test2");
        //Web.Accounts.SaveObject(new Account { emailAddress = "play@velcer.net", Id = Guid.NewGuid().ToString(), password = "asddfgh", username = "Velcer"});
    }


    public void Login() 
    {
        var email = _loginEmail.text;
        var password = _loginPassword.text;

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
        //LightshiftNetworkManager.Login(email, password);

        HttpService.Get("account/login", new LoginRequest
        {
            Email = email, Password = password
        }, 
        delegate (AuthenticationResponse response) 
        {
            HandleResponse(response.ResponseType, response.Message);
        }, delegate (Exception e) 
        {
            DialogManager.ShowMessage("Uh oh! The servers couldn't be reached. Check your internet connection and if the issue persists please report the outage on our discord server. https://discord.gg/8S9A2f4");
            windowManager.OpenPanel("Login");
        });

        ShowLoading();
    }

    public void Register()
    {
        var email = _registerEmail.text;
        var username = _registerUsername.text;
        var password = _registerPassword.text;
        var confirmPassword = _registerConfirmPassword.text;

        _loginEmail.text = _registerEmail.text;

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

        if (username.Length < 2)
        {
            DialogManager.ShowMessage("Username must be at least 2 characters long!!");
            return;
        }
        // TO DO : REGISTER

        //LightshiftNetworkManager.Register(email, username, password);
        HttpService.Get("account/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            Username = username
        },
        delegate (AuthenticationResponse response)
        {
            HandleResponse(response.ResponseType, response.Message);
        }, delegate (Exception e)
        {
            DialogManager.ShowMessage("Uh oh! The servers couldn't be reached. Check your internet connection and if the issue persists please report the outage on our discord server. https://discord.gg/8S9A2f4");
            windowManager.OpenPanel("Register");
        });

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

        //LightshiftNetworkManager.Recover(email);
        ShowLoading();
    }

    public void ConfirmAccount()
    {
        var code = _confirmCode.text;

        if (code.Length != 5)
        {
            DialogManager.ShowMessage("Please enter the code provided in the Email you registered with.");
            return;
        }

        

        //LightshiftNetworkManager.ConfirmAccount(_loginEmail.text, code);
        ShowLoading();
    }

    public void RequestNewConfirmationCode()
    {
        //LightshiftNetworkManager.RequestNewConfirmationCode(_loginEmail.text);
        DialogManager.ShowMessage("A new email has been sent. Please allow upto 5 minutes for it to process and be sure to check your spam folder!");
        ShowLoading();
    }

    public void ShowLoading() 
    {
        windowManager.OpenPanel("Loading");
    }

    public void ShowNews() {

        windowManager.OpenPanel("News");

    }

    public void OpenDiscord() 
    {
        Application.OpenURL("https://discord.gg/8S9A2f4");
    }

    private void UpdateSavedCredentials(string email, string password) 
    {
        PlayerPrefs.SetString("login.email", email);
        PlayerPrefs.SetString("login.password", password);
        PlayerPrefs.Save();
    }

    public void HandleResponse(AuthenticationResponseType response, string message = "")
    {
        switch (response)
        {
            case AuthenticationResponseType.LoginSuccess:
                FindObjectOfType<LightshiftNetworkManager>().Authenticate(message);
                UpdateSavedCredentials(_loginEmail.text, _loginPassword.text);
                break;
            case AuthenticationResponseType.RegisterSuccess:
                FindObjectOfType<LightshiftNetworkManager>().Authenticate(message);
                UpdateSavedCredentials(_registerEmail.text, _registerPassword.text);
                break;
            case AuthenticationResponseType.ValidationRequired:
                windowManager.OpenPanel("ConfirmAccount");
                break;
            case AuthenticationResponseType.ValidationFail:
                DialogManager.ShowMessage("Invalid Code.");
                windowManager.OpenPanel("ConfirmAccount");
                break;
            case AuthenticationResponseType.LoginFail:
                DialogManager.ShowMessage("Invalid login details.");
                windowManager.OpenPanel("Login");
                break;
            case AuthenticationResponseType.AuthenticationFail:
                DialogManager.ShowMessage("Uh oh! We couldn't properly authenticate you on the game server. Please try again.");
                windowManager.OpenPanel("Login");
                break;
            case AuthenticationResponseType.EmailTaken:
                DialogManager.ShowMessage("That email is already in use.");
                windowManager.OpenPanel("Register");
                break;
            case AuthenticationResponseType.UsernameTaken:
                DialogManager.ShowMessage("That username is taken!");
                windowManager.OpenPanel("Register");
                break;
            case AuthenticationResponseType.RecoverSuccess:
                DialogManager.ShowMessage("Account found! Please check your email.");
                windowManager.OpenPanel("Login");
                break;
            case AuthenticationResponseType.RecoverFail:
                DialogManager.ShowMessage("That email does not exist.");
                windowManager.OpenPanel("Recover");
                break;
            case AuthenticationResponseType.Banned:
                DialogManager.ShowDialog("You are banned.", delegate (bool result)
                {
                    if (result)
                    DialogManager.ShowDialog("Are you sure you want to cancel?", delegate (bool result2)
                    {
                        if (result2) 
                        {
                            DialogManager.ShowMessage("You cannot cancel a ban, silly!");
                        }
                        else DialogManager.ShowMessage("Smart lad.");
                    });
                });
                windowManager.OpenPanel("Login");
                break;
        }
    }
}
