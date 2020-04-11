using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceConnection : BaseConnection
{
    public ServiceConnection()
    {
        AddMessageHandler("init", OnInit);
        AddMessageHandler("username", OnUsernameMessage);
        AddMessageHandler("alert", OnAlertMessage);

        ConnectToServiceRoom();
    }

    private void OnAlertMessage(Message message)
    {
        DialogManager.ShowMessage(message.GetString(0));
    }

    private void OnUsernameMessage(Message message)
    {
        switch (message.GetString(0))
        {
            // Set username
            case "set":
                PlayerIONetwork.Instance.loginUI.ShowUsernameForm();
                break;

            //Username Taken
            case "taken":
                PlayerIONetwork.Instance.loginUI.ShowError("Username is taken!");
                PlayerIONetwork.Instance.loginUI.ShowUsernameForm();
                break;
            //Username invalid
            case "invalid":
                PlayerIONetwork.Instance.loginUI.ShowError("Username is invalid!");
                break;

            //Username confirmed
            case "success":
                Send("init");
                break;
        }
    }

    private void OnInit(Message message)
    {
        //Show Loading Status for login menu if we're
        // still on the login scene.
        if (PlayerIONetwork.Instance.loginUI)
        {
            PlayerIONetwork.Instance.loginUI.Loading = true;
            PlayerIONetwork.Instance.loginUI.ShowMessage("Waiting for Server...");
        }
        PlayerIONetwork.Instance.JoinServer(message.GetString(0));
        
    }

    public override void OnMessage(object sender, Message message)
    {
        Debug.Log(message);
        base.OnMessage(sender, message);
    }

    private void ConnectToServiceRoom()
    {
        //Creating a Game Server specifically for this user and this user alone. It will handle all special data, e.g shop.
        GetClient().Multiplayer.CreateJoinRoom(GetClient().ConnectUserId, "Service", false, null, null, delegate (Connection connection)
        {
            SetConnection(connection);

            Send("init");

        }, delegate (PlayerIOError error)
        {
            DialogManager.ShowMessage("An error occured while connecting to the server.");
            PlayerIONetwork.Instance.loginUI.Loading = false;
        });
    }
}

