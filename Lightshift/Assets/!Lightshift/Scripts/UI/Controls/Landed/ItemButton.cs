using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnClick = new UnityEvent();

    public Vector2 HighlightScale = new Vector2(1.1f, 1.1f);
    public Vector2 NormalScale = new Vector2(1f, 1f);
    public Vector2 PressedScale = new Vector2(.9f, .9f);

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = PressedScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = HighlightScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = NormalScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = HighlightScale;
    }
}
