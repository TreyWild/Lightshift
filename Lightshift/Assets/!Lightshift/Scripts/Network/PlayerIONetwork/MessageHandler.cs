using PlayerIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageHandler
{
    public string MessageType;
    public event Action<Message> OnMessageRecieved;
    public virtual void OnMessage(Message message)
    {
        OnMessageRecieved?.Invoke(message);
    }
}