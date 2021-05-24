using Assets._Lightshift.Scripts.Web;
using JetBrains.Annotations;
using MasterServer;
using Mirror;
using PlayerIOClient;
using SharedModels;
using SharedModels.WebRequestObjects;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class LightshiftAuthenticator : NetworkAuthenticator
{
    [Header("Authentication")]

    // set these in the inspector
    public string sessionAuthKey = "";

    public struct AuthRequestMessage : NetworkMessage
    {
        public string sessionAuthKey;
    }

    public struct AuthResponseMessage : NetworkMessage
    {
        public AuthenticationResponseType response;
    }
    #region Messages

    bool ServerAuthFailed;

    #endregion

    #region Server

    /// <summary>
    /// Called on server from StartServer to initialize the Authenticator
    /// <para>Server message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartServer()
    {
        // register a handler for the authentication request we expect from client
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    /// <summary>
    /// Called on server from StopServer to reset the Authenticator
    /// <para>Server message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStopServer()
    {
        // unregister the handler for the authentication request
        NetworkServer.UnregisterHandler<AuthRequestMessage>();
    }

    /// <summary>
    /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    public override void OnServerAuthenticate(NetworkConnection conn)
    {
        // do nothing...wait for AuthRequestMessage from client
    }

    /// <summary>
    /// Called on server when the client's AuthRequestMessage arrives
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    /// <param name="msg">The message payload</param>
    public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
    {
        // Debug.LogFormat(LogType.Log, "Authentication Request: {0} {1}", msg.authUsername, msg.authPassword);
        HttpService.Get("account/authenticate", new JsonString { Value = msg.sessionAuthKey },
        delegate (JsonString json)
        {
            if (json.Value != null)
            {
                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    response = AuthenticationResponseType.AuthenticationSuccess
                };

                conn.Send(authResponseMessage);

                conn.authenticationData = json;

                // Accept the successful authentication
                ServerAccept(conn);
            }
            else
            {
                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    response = AuthenticationResponseType.AuthenticationFail
                };

                conn.Send(authResponseMessage);

                // must set NetworkConnection isAuthenticated = false
                conn.isAuthenticated = false;

                // disconnect the client after 1 second so that response message gets delivered
                if (!ServerAuthFailed)
                {
                    // set this false so this coroutine can only be started once
                    ServerAuthFailed = true;

                    StartCoroutine(DelayedDisconnect(conn, 1));
                }
            }
        });
    }

    IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // Reject the unsuccessful authentication
        ServerReject(conn);
    }

    #endregion

    #region Client

    /// <summary>
    /// Called on client from StartClient to initialize the Authenticator
    /// <para>Client message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartClient()
    {
        // register a handler for the authentication response we expect from server
        NetworkClient.RegisterHandler<AuthResponseMessage>((Action<AuthResponseMessage>)OnAuthResponseMessage, false);
    }

    /// <summary>
    /// Called on client from StopClient to reset the Authenticator
    /// <para>Client message handlers should be unregistered in this method.</para>
    /// </summary>
    public override void OnStopClient()
    {
        // unregister the handler for the authentication response
        NetworkClient.UnregisterHandler<AuthResponseMessage>();
    }

    /// <summary>
    /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
    /// </summary>
    /// <param name="conn">Connection of the client.</param>
    public override void OnClientAuthenticate()
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage
        {
            sessionAuthKey = sessionAuthKey
        };

        NetworkClient.connection.Send(authRequestMessage);
    }

    [Obsolete("Call OnAuthResponseMessage without the NetworkConnection parameter. It always points to NetworkClient.connection anyway.")]
    public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg) => OnAuthResponseMessage(msg);

    /// <summary>
    /// Called on client when the server's AuthResponseMessage arrives
    /// </summary>
    /// <param name="msg">The message payload</param>
    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
        if (msg.response == AuthenticationResponseType.AuthenticationSuccess)
        {
            // Debug.LogFormat(LogType.Log, "Authentication Response: {0}", msg.message);

            // Authentication has been accepted
            ClientAccept();
        }
        else
        {
            Debug.LogError($"Authentication Response: {msg.response}");

            // Authentication has been rejected
            ClientReject();
            LoginManager.Instance.ShowGameServerAuthenticationFailure();
        }
    }

    #endregion
}


