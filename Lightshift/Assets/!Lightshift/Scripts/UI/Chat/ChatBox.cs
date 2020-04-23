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

    void Awake() 
    {
        if (Instance != null)
            Destroy(Instance);
        else Instance = this;
    }

    private void Start()
    {
        ChatBoxTextbox.verticalScrollbar.value = 1;
        ChatBoxTextbox.text = "";
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
        ChatBoxTextbox.text = (ChatBoxTextbox.text + Environment.NewLine + message);
        ChatBoxTextbox.stringPosition = ChatBoxTextbox.text.Length;

        StartCoroutine(ScrollToValue(ChatBoxTextbox.verticalScrollbar.value));
    }

    IEnumerator ScrollToValue(float value)
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        while (true) 
        {
            ChatBoxTextbox.verticalScrollbar.value = value;
            yield return wait;
        }
    }
}
