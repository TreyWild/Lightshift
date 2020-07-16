using JetBrains.Annotations;
using Mirror;
using PlayerIOClient;
using SharedModels.Models.User;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class LightshiftAuthenticator : NetworkAuthenticator
{
    public enum AuthType
    {
        Login,
        Register,
        Recover,
        ConfirmAccount,
    }

    public enum AuthResponse 
    {
        LoginSuccess,
        LoginFail,
        UsernameTaken,
        EmailTaken,
        RegisterSuccess,
        Banned,
        RecoverSuccess,
        RecoverFail,
    }

    [Header("Custom Properties")]

    // set these in the inspector
    public string email = "";
    public string username = "";
    public string passwordHash = "";
    public string verificationToken;
    public AuthType authType;

    public class AuthRequestMessage : MessageBase
    {
        // use whatever credentials make sense for your game
        // for example, you might want to pass the accessToken if using oauth
        public string email = "";
        public string username = "";
        public string passwordHash = "";
        public string verificationToken;

        public AuthType authType;
    }

    public class AuthResponseMessage : MessageBase
    {
        public AuthResponse response;
    }

    public override void OnStartServer()
    {
        // register a handler for the authentication request we expect from client
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    public override void OnStartClient()
    {
        // register a handler for the authentication response we expect from server
        NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }

    public override void OnServerAuthenticate(NetworkConnection conn)
    {
        // do nothing...wait for AuthRequestMessage from client
    }

    public override void OnClientAuthenticate(NetworkConnection conn)
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage
        {
            authType = authType,
            email = email,
            passwordHash = passwordHash,
            username = username,
            verificationToken = verificationToken
        };

        conn.Send(authRequestMessage);
    }

    public void Disconnect(NetworkConnection connection, AuthResponse reason) 
    {
        AuthResponseMessage authResponseMessage = new AuthResponseMessage
        {
            response = reason
        };
        connection.Send(authResponseMessage);
        connection.isAuthenticated = false;
        StartCoroutine(DelayedDisconnect(connection, 1));
        return;
    }

    public void LoginSuccess(NetworkConnection connection, AuthResponse response, Account account)
    {
        // create and send msg to client so it knows to proceed
        AuthResponseMessage authResponseMessage = new AuthResponseMessage
        {
            response = response
        };

        OnServerAuthenticated.Invoke(connection);

        Server.InitPlayer(connection, account);
        return;
    }

    public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage message)
    {
        Debug.Log($"{message.authType}[ Email: {message.email}, PasswordHash: {message.passwordHash}, Username: {message.username}, Verification: {message.verificationToken} ]");

        switch (message.authType)
        {
            case AuthType.Recover:
                // TO DO : RECOVER ACCOUNT
                break;
            case AuthType.ConfirmAccount:
                // TO DO : CONFIRM ACCOUNT
                break;
            case AuthType.Login:
                {
                    DB.Accounts.GetAccountByEmail(message.email, delegate (Account account) 
                    {
                        bool validPassword = PasswordHasher.Verify(message.passwordHash, account.password);

                        if (account == null || !validPassword)
                        {
                            Disconnect(conn, AuthResponse.LoginFail);
                            return;
                        }

                        LoginSuccess(conn, AuthResponse.LoginSuccess, account);
                    });           
                }
                break;
            case AuthType.Register:
                {
                    DB.Accounts.CheckEmailAvailability(message.email, delegate (bool available) 
                    {
                        if (!available)
                        {
                            Disconnect(conn, AuthResponse.EmailTaken);
                            return;
                        }

                        DB.Accounts.CheckUsernameAvailability(message.username, delegate (bool available) 
                        {
                            if (!available)
                            {
                                Disconnect(conn, AuthResponse.UsernameTaken);
                                return;
                            }

                            var account = new Account 
                            {
                                emailAddress = message.email,
                                Id = Guid.NewGuid().ToString(),
                                password = message.passwordHash,
                                username = message.username,

                            };
                        });
                    });


                    DB.Accounts.GetAccountByEmail(message.email, delegate (Account account)
                    {
                        bool validPassword = PasswordHasher.Verify(message.passwordHash, account.password);

                        if (account == null || !validPassword)
                        {
                            Disconnect(conn, AuthResponse.LoginFail);
                            return;
                        }

                        LoginSuccess(conn, AuthResponse.LoginSuccess, account);
                    });
                    //if (!Database.Users.EmailAvailable(message.email))
                    //{
                    //    Disconnect(conn, AuthResponse.EmailTaken);
                    //    return;
                    //}
                    //else if (!Database.Users.UsernameAvailable(message.username))
                    //{
                    //    Disconnect(conn, AuthResponse.UsernameTaken);
                    //    return;
                    //}
                    //else
                    //{

                    //    //var user = new User
                    //    //{
                    //    //    passwordHash = PasswordHasher.Hash(message.passwordHash),
                    //    //    username = message.username,
                    //    //    emailAddress = message.email,
                    //    //    Id = Guid.NewGuid().ToString(),
                    //    //    IsVerified = false,
                    //    //};


                    //    //Database.Users.Save(user);

                    //    LoginSuccess(conn, AuthResponse.RegisterSuccess, user);
                    //}
                }
            break;
        }
    }

    public IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        conn.Disconnect();
    }

    public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
    {
        Debug.Log(msg.response);

        if (msg.response == AuthResponse.RegisterSuccess || msg.response == AuthResponse.LoginSuccess)
        {
            OnClientAuthenticated.Invoke(conn);
            return;
        }

        LoginManager.Instance.HandleResponse(msg.response);

        conn.isAuthenticated = false;
        // disconnect the client
        conn.Disconnect();
    }
}

