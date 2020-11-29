using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemViewModuleControl : MonoBehaviour
{
    [SerializeField] private ItemGraphicDisplay _graphicDisplay;
    [SerializeField] private ItemGraphicDisplay _lore;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private ButtonManagerBasic _button;

    [SerializeField] StatView _statView;

    [SerializeField] ButtonManagerBasic _equipButton;
}
