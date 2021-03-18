using Assets._Lightshift.Scripts.Network;
using Lightshift;
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
        {
            Destroy(Instance);
        }
        
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
        ChatBoxTextbox = null;
        ChatInputBox = null;
        ChatInput = null;
        _chatMessages = null;
    }

    private void Start()
    {
        ChatBoxTextbox.text = "";
        ChatBoxTextbox.verticalScrollbar.SetDirection(Scrollbar.Direction.TopToBottom, true);
        FixScrollbar();
    }

    private void OnValueChanged(float newValue)
    {
        FixScrollbar();
    }

    private void OnEnable()
    {
        FixScrollbar();
    }

    private void FixScrollbar()
    {
        StartCoroutine(FixScrollBar());
    }

    IEnumerator FixScrollBar() 
    {
        yield return new WaitForEndOfFrame();
        ChatBoxTextbox.verticalScrollbar.value = 1;
    }

    void Update()
    {

        if (Input.GetKeyDown(Settings.ChatKey) || Input.GetKeyDown(Settings.ChatKeyAlt))
        {
            if (ChatInputBox.activeInHierarchy)
            {
                Settings.KeysLocked = false;

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
                Settings.KeysLocked = false;
            }
            else
            {
                Settings.KeysLocked = true;
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

        Communication.SendChatMessage(message: message);
    }

    public void AddMessage(string message)
    {
        _chatMessages.Add(message);
        if (_chatMessages.Count > 100)
            _chatMessages.Remove(_chatMessages[0]);

        string text = "";
        foreach (var m in _chatMessages)
            if (text == "")
                text += $"{m}";
            else text += $"{Environment.NewLine}{m}";

        ChatBoxTextbox.text = (text);
        FixScrollbar();
    }

}
