using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PickerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _displayLabel;
    [SerializeField] private TextMeshProUGUI _controlLabel;

    private List<PickerItemObject> _items = new List<PickerItemObject>();
    private int _currentIndex;

    public string saveCode;
    public void Initialize(string description, string saveCode, List<PickerItemObject> items) 
    {
        _items = items;
        _displayLabel.text = description;
        this.saveCode = saveCode;

        string desiredIndex = PlayerPrefs.GetString(saveCode, "");
        if (desiredIndex == "")
            _currentIndex = 0;
        else 
            for (int i = 0; i < items.Count; i++)
                if (items[i].value.ToLower() == desiredIndex.ToLower())
                    _currentIndex = i;
        ShowCurrentIndex();
    }

    public void SetIndex(int index) 
    {
        _currentIndex = index;
        ShowCurrentIndex();
    }
    private void ShowCurrentIndex() 
    {
        if (_currentIndex >= _items.Count)
            _currentIndex = 0;
        else if (_currentIndex < 0)
            _currentIndex = _items.Count - 1;

        var item = _items[_currentIndex];
        _controlLabel.text = item.displayValue;
    }

    public void OnBack() 
    {
        _currentIndex--;
        ShowCurrentIndex();
    }

    public void OnForwards() 
    {
        _currentIndex++;
        ShowCurrentIndex();
    }

    public string GetCurrentValue() 
    {
        ShowCurrentIndex();
        return _items[_currentIndex].value;
    }
}

public class PickerItemObject
{
    public string displayValue;
    public string value;
}
