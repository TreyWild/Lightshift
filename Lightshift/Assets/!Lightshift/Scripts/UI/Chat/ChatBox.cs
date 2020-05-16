﻿using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    public static ChatBox Instance;

    public TMP_InputField ChatBoxTextbox;
    public GameObject ChatInputBox;
    public TMP_InputField ChatInput;
    private List<string> _chatMessages = new List<string>();
    void Awake() 
    {
        if (Instance != null)
            Destroy(Instance);
        else Instance = this;
    }

    private void Start()
    {
        ChatBoxTextbox.verticalScrollbar.value = 1;
        for (int i = 0; i < 20; i++) 
        {
            AddMessage(Environment.NewLine);
        }

        ChatBoxTextbox.verticalScrollbar.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float newValue)
    {
        if (newValue == 0)
            ChatBoxTextbox.verticalScrollbar.value = 1;
    }

    private void OnEnable()
    {
        ChatBoxTextbox.verticalScrollbar.value = 1;
    }

    void Update()
    {

        if (Input.GetKeyDown(Settings.Instance.ChatKey) || Input.GetKeyDown(Settings.Instance.ChatKeyAlt))
        {
            if (ChatInputBox.activeInHierarchy)
            {
                Settings.Instance.KeysLocked = false;

                var msg = ChatInput.text;
                msg.Replace("<", "");
                msg.Replace(">", "");

                if (msg != "")
                {
                    if (Game.Instance == null)
                        AddMessage(msg);
                    else
                        SendChatMessage(msg);
                }
                ChatInputBox.SetActive(false);
                ChatInput.text = "";
                Settings.Instance.KeysLocked = false;
            }
            else
            {
                Settings.Instance.KeysLocked = true;
                ChatInputBox.SetActive(true);
                ChatInput.ActivateInputField();
            }
        }
    }
    public void SendChatMessage(string message = "")
    {
        if (message == "")
            message = ChatInput.text;

        if (message == "")
            return;

        ChatInput.text = "";

        NetworkClient.Send(new ChatMessage {message = message });
    }

    public void AddMessage(string message)
    {
        _chatMessages.Add(message);
        if (_chatMessages.Count > 100)
            _chatMessages.Remove(_chatMessages[0]);
        ChatBoxTextbox.text = "";
        foreach (var m in _chatMessages)
            ChatBoxTextbox.text += $"{Environment.NewLine}{m}";
    }

}
