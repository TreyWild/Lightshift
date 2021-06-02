using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ModuleItemControl : MonoBehaviour
{
    public ModuleLocation ModuleLocation;

    private ItemButton _button;
    private List<ItemGraphicDisplay> _graphics = new List<ItemGraphicDisplay>();

    public UnityEvent<ModuleItemControl> OnClicked = new UnityEvent<ModuleItemControl>();

    private bool _disabled = false;
    private void Awake()
    {
        _graphics = GetComponentsInChildren<ItemGraphicDisplay>().ToList();

        var parentComponant = GetComponent<ItemGraphicDisplay>();
        if (parentComponant != null)
            _graphics.Add(parentComponant);

        _button = gameObject.AddComponent<ItemButton>();

        _button.OnClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        OnClicked?.Invoke(this);
    }

    public void Clear() 
    {
        foreach (var graphic in _graphics)
            graphic.Clear();
    }

    public void SetItem(ModuleItem item) 
    {
        foreach (var graphic in _graphics) 
            graphic.InitializeGraphic(item.Sprite);
    }
}
