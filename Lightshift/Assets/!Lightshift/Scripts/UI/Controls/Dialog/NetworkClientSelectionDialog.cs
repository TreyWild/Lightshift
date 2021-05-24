using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NetworkClientSelectionDialog : BaseDialog
{
    public Action OnClickClient;
    public Action OnClickServer;
    public Action OnClickHost;

    [SerializeField] TextMeshProUGUI _button2Text;
    public void SetButton2Text(string text) => _button2Text.text = text;

    public void ClickClient()
    {
        OnClickClient?.Invoke();
        CloseMenu();
    }

    public void ClickServer()
    {
        OnClickServer?.Invoke();
        CloseMenu();
    }

    public void ClickHost()
    {
        OnClickHost?.Invoke();
        CloseMenu();
    }

    private void CloseMenu() 
    {
        Destroy(gameObject);
    }
}