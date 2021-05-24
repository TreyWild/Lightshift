using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIMapObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _loreLabel;
    [SerializeField] private Image _iconHolder;


    private void Start()
    {
        _iconHolderTransform = _iconHolder.transform;
        _iconHolderRectTransform = _iconHolder.GetComponent<RectTransform>();
    }
    public void Init(MapObject mapObject) 
    {
        _nameLabel.text = mapObject.Name;
        _nameLabel.color = mapObject.nameColor;

        _loreLabel.text = mapObject.Lore;
        _iconHolder.sprite = mapObject.Icon;
        _iconHolder.color = mapObject.iconColor;

        if (mapObject.hasLayoutPriority)
            transform.SetAsLastSibling();
    }

   
    public void SetSize(Vector2 size) 
    {
        _iconHolderRectTransform.sizeDelta = size;
    }
    private RectTransform _iconHolderRectTransform;
    private Transform _iconHolderTransform;
    public void SetRotation(float rotation) => _iconHolderTransform.eulerAngles = new Vector3(0, 0, rotation);
}

