
using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseConnection
{
    private Connection _connection { get; set; }
    public Connection GetConnection() => _connection;
    public void SetConnection(Connection connection) 
    {
        _connection = connection;
        _connection.OnMessage += OnMessage;
        _connection.OnDisconnect += OnDisconnect;
    } 

    public Client GetClient() => PlayerIONetwork.Instance.Client;

    private List<MessageHandler> _messageHandlers = new List<MessageHandler>();

    public void Send(string type, params object[] args)
    {
        if (_connection != null && _connection.Connected)
            _connection.Send(type, args);
    }

    public void Send(Message message)
    {
        if (_connection != null && _connection.Connected)
            _connection.Send(message);
    }

    public void Disconnect() 
    {
        if (_connection != null && _connection.Connected)
            _connection.Disconnect();
    }

    public void AddMessageHandler(string type, Action<Message> callback = null) 
    {
        var messageHandler = _messageHandlers.FirstOrDefault(m => m.MessageType == type);
        if  (messageHandler == null) 
        {
            messageHandler = new MessageHandler { MessageType = type };
            if (callback != null)
            messageHandler.OnMessageRecieved += callback;
            _messageHandlers.Add(messageHandler);
        }
    }

    public void RemoveMessageHandler(string type, Action<Message> callback = null) 
    {
        var messageHandler = _messageHandlers.FirstOrDefault(m => m.MessageType == type);
        if (messageHandler != null)
        {
            messageHandler = new MessageHandler { MessageType = type };

            if (callback != null)
                messageHandler.OnMessageRecieved -= callback;

            _messageHandlers.Remove(messageHandler);
        }
    }

    public virtual void OnDisconnect(object sender, string message)
    {
        // yeet
    }

    public virtual void OnMessage(object sender, Message message)
    {
        var messageHandler = _messageHandlers.FirstOrDefault(h => h.MessageType == message.Type);
        if (messageHandler == null)
        {
            Debug.Log($"Unhandled Message: {message}");
            return;
        }

        messageHandler.OnMessage(message);
    }
}
