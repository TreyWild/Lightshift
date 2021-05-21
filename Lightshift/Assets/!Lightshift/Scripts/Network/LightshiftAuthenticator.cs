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

    public override void OnClientAuthenticate()
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage
        {
            sessionAuthKey = sessionAuthKey
        };

        NetworkClient.connection.Send(authRequestMessage);
    }

    public void Disconnect(NetworkConnection connection, AuthenticationResponseType reason) 
    {
        Debug.LogError($"Auth Failed: [{reason}]");
        AuthResponseMessage authResponseMessage = new AuthResponseMessage
        {
            response = reason
        };
        connection.Send(authResponseMessage);
        connection.isAuthenticated = false;
        StartCoroutine(DelayedDisconnect(connection, 1));
        return;
    }

    public void LoginSuccess(NetworkConnection connection)
    {
        // create and send msg to client so it knows to proceed
        AuthResponseMessage authResponseMessage = new AuthResponseMessage
        {
            response = AuthenticationResponseType.AuthenticationSuccess
        };

        ServerAccept(connection);

        connection.Send(authResponseMessage);

        Debug.Log($"{((JsonString)connection.authenticationData).Value} was authenticated.");

        return;
    }

    public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage message)
    {
        HttpService.Get("account/authenticate", new JsonString {Value = message.sessionAuthKey },
        delegate (JsonString json)
        {
            if (json.Value == null)
            {
                Disconnect(conn, AuthenticationResponseType.AuthenticationFail);
            }
            else 
            {
                conn.authenticationData = json;
                Debug.Log($"{conn.connectionId}");
                LoginSuccess(conn);
            }
        });
    }

    public IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        conn.Disconnect();
    }

    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
        Debug.Log(msg.response);

        if (msg.response == AuthenticationResponseType.AuthenticationSuccess)
        {
            ClientAccept();
            return;
        }

        LoginManager.Instance.HandleResponse(msg.response);

        ClientReject();
    }
}

