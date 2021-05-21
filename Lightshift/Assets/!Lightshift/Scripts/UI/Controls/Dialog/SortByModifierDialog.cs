using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMPro;
using UnityEngine.UI;
using UnityEngine;
using SharedModels.Models.Game;
using Michsky.UI.ModernUIPack;
using Assets._Lightshift.Scripts.Utilities;

public class SortByModifierDialog : BaseDialog
{
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private GameObject _itemPrefab;

    public Action onBackClicked;
    public Action<Modifier> onConfirm;

    private void Start()
    {
        foreach (Modifier modifier in Enum.GetValues(typeof(Modifier)))
        {
            var button = Instantiate(_itemPrefab, _contentPanel).GetComponent<ButtonManagerBasic>();
            button.buttonText = modifier.ToString();

            var image = button.GetComponent<Image>();
            image.color = ColorHelper.FromModifier(modifier);

            button.buttonEvent.AddListener(delegate () 
            {
                onConfirm(modifier);
                GoBack();
            });
        }
    }
    public void GoBack()
    {
        onBackClicked?.Invoke();
        onBackClicked = null;
        Destroy(gameObject);
    }
}


