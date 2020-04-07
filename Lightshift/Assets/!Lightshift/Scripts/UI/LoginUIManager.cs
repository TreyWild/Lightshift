using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoginUIManager : MonoBehaviour
{
    [Header("Background")]
    public bool RainbowColors = true;
    public float BackgroundSpeed = 0.01f;

    [Header("AuthForm")]
    public string LogoText = "Tutorial Project";
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TextMeshProUGUI _logoText;

    [Header("Login Form")]
    [SerializeField] private TMP_InputField _emailInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private GameObject _loginForm;
    [SerializeField] private Toggle _rememberMeCheckbox;

    [Header("Register Form")]
    [SerializeField] private TMP_InputField _registerEmailInput;
    [SerializeField] private TMP_InputField _registerPasswordInput;
    [SerializeField] private TMP_InputField _confirmPasswordInput;
    [SerializeField] private GameObject _registerForm;


    [Header("Recover Form")]
    [SerializeField] private TMP_InputField _recoverEmailInput;
    [SerializeField] private GameObject _recoverForm;

    [Header("Username Form")]
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private GameObject _usernameForm;

    public Action<string, string> onLogin;
    public Action<string, string> onRegister;
    public Action<string> onRecover;
    public Action<string> onUsernameSubmit;
    public enum ActiveFrom
    {
        None,
        Login,
        Register,
        Recover,
        Username,
    }

    public ActiveFrom currentForm;

    private Camera _camera;
    private void Awake()
    {
        _camera = Camera.main;
    }
    private void Start()
    {
        _logoText.text = LogoText;
        Back();

        if (PlayerPrefs.GetString("remember", "").ToLower() == "true")
        {
            _emailInput.text = PlayerPrefs.GetString("email", "");
            _passwordInput.text = PlayerPrefs.GetString("password", "");
            _rememberMeCheckbox.isOn = true;
        }

        if (_emailInput.text == "")
            _emailInput.ActivateInputField();
        else if (_passwordInput.text == "") _passwordInput.ActivateInputField();
    }

    public void SetLoginPrefs(string email, string password) 
    {
        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);
        PlayerPrefs.SetString("remember", _rememberMeCheckbox.isOn.ToString());
        PlayerPrefs.Save();
    }
    private void Update()
    {
        if (RainbowColors)
            _camera.backgroundColor = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * BackgroundSpeed, 1), 1, 1));
        #region Handle Textbox Focus
        if (Input.GetKeyDown("tab") || Input.GetKeyDown("down"))
        {
            switch (currentForm)
            {
                case ActiveFrom.Login:
                    if (_emailInput.isFocused)
                    {
                        _passwordInput.ActivateInputField();
                    }
                    else
                        _emailInput.ActivateInputField();
                    break;
                case ActiveFrom.Register:
                    if (_registerEmailInput.isFocused)
                    {
                        _registerPasswordInput.ActivateInputField();
                    }
                    else if (_registerPasswordInput.isFocused)
                    {
                        _confirmPasswordInput.ActivateInputField();
                    }
                    else _registerEmailInput.ActivateInputField();
                    break;
            }
        }
        else if (Input.GetKeyDown("return") || Input.GetKeyDown("enter"))
        {
            switch (currentForm)
            {
                case ActiveFrom.Login:
                    Login();
                    break;
                case ActiveFrom.Recover:
                    RecoverEmail();
                    break;
                case ActiveFrom.Register:
                    Register();
                    break;
                case ActiveFrom.Username:
                    SubmitUsername();
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Back();
        #endregion
    }

    public void Login()
    {
        if (_emailInput.text == "")
        {
            ShowError("Email field cannot be empty.");
            return;
        }

        if (_passwordInput.text == "")
        {
            ShowError("Password field cannot be empty.");
            return;
        }

        Loading = true;
        onLogin?.Invoke(_emailInput.text, _passwordInput.text);
    }

    public void Register()
    {
        _rememberMeCheckbox.isOn = true;
        if (_registerEmailInput.text == "" || !_registerEmailInput.text.Contains("@"))
        {
            ShowError("Please enter a valid Email Address");
            return;
        }
        if (_registerPasswordInput.text != _confirmPasswordInput.text)
        {
            ShowError("Passwords do not match !");
            _registerPasswordInput.text = "";
            _confirmPasswordInput.text = "";
            return;
        }

        if (_registerPasswordInput.text.Length < 6)
        {
            ShowError("Password too small! Please use 6 or more characters.");
            _registerPasswordInput.text = "";
            _confirmPasswordInput.text = "";
            return;
        }

        Loading = true;
        onRegister?.Invoke(_registerEmailInput.text, _registerPasswordInput.text);
    }

    public void RecoverEmail()
    {
        if (_recoverEmailInput.text == "" || !_recoverEmailInput.text.Contains("@"))
        {
            ShowError("Please enter a valid Email Address");
            return;
        }

        Loading = true;
        onRecover?.Invoke(_recoverEmailInput.text);
    }

    public void SubmitUsername()
    {
        if (_usernameInput.text == "")
        {
            ShowError("Invalid username!");
            return;
        }

        if (_usernameInput.text.Length < 3 || _usernameInput.text.Length > 20)
        {
            ShowError("Username must be between 3 and 20 characters in length.");
            return;
        }

        Loading = true;
        onUsernameSubmit?.Invoke(_usernameInput.text);
    }

    public void Back()
    {
        _loginForm.SetActive(true);
        _registerForm.SetActive(false);
        _recoverForm.SetActive(false);
        _usernameForm.SetActive(false);
        currentForm = ActiveFrom.Login;
        _emailInput.ActivateInputField();

        _logoText.text = currentForm.ToString();
    }

    public void ShowRegisterForm()
    {
        _loginForm.SetActive(false);
        _registerForm.SetActive(true);
        currentForm = ActiveFrom.Register;
        _registerEmailInput.ActivateInputField();
        Loading = false;

        _logoText.text = currentForm.ToString();
    }

    public void ShowRecoverForm()
    {
        _loginForm.SetActive(false);
        _recoverForm.SetActive(true);
        currentForm = ActiveFrom.Recover;
        _recoverEmailInput.ActivateInputField();
        Loading = false;

        _logoText.text = currentForm.ToString();
    }

    public void ShowUsernameForm()
    {
        _recoverForm.SetActive(false);
        _usernameForm.SetActive(true);
        _recoverForm.SetActive(false);
        _loginForm.SetActive(false);
        currentForm = ActiveFrom.Username;
        Loading = false;

        _logoText.text = currentForm.ToString();
    }

    public void ShowMessage(string messaage)
    {
        ShowDisplay(messaage, new Color(231, 106, 106));
    }

    public void ShowError(string error)
    {
        ShowDisplay(error, new Color(231, 106, 106));
    }

    private void ShowDisplay(string display, Color color)
    {
        _errorText.color = color;
        _errorText.CrossFadeAlpha(1, 0, false);
        _errorText.text = display;
        _errorText.CrossFadeAlpha(0, 5, false);
    }

    private bool _loading = false;
    public bool Loading
    {
        get
        {
            return _loading;
        }
        set
        {
            _loading = value;
            SetLoading(_loading);
        }
    }

    private void SetLoading(bool loading = true)
    {
        switch (currentForm)
        {
            case ActiveFrom.Login:
                _loginForm.SetActive(!loading);
                break;
            case ActiveFrom.Recover:
                _recoverForm.SetActive(!loading);
                break;
            case ActiveFrom.Register:
                _registerForm.SetActive(!loading);
                break;
            case ActiveFrom.Username:
                _usernameForm.SetActive(!loading);
                break;
        }

        ShowMessage("");
        if (loading)
        {
            _errorText.CrossFadeAlpha(1, 0, false);
            _errorText.text = "Loading...";
        }
    }
}
