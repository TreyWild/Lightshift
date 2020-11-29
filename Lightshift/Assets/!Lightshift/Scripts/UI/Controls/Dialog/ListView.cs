using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListView : MonoBehaviour
{
    [Header("Titlebar")]
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _image;

    [Header("List")]
    [SerializeField] private GameObject _contentPanel;
    public void Close()
    {
        Destroy(gameObject);
    }

    public void SetTitle(string title) 
    {
        _title.text = title;
    }

    public GameObject InstantiateItem(GameObject item) 
    {
        return Instantiate(item, _contentPanel.transform);
    }

    public void RemoveItem(GameObject item) 
    {
        Destroy(item);
    }
}
