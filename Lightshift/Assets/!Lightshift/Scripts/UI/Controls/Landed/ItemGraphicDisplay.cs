
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class ItemGraphicDisplay : MonoBehaviour
{
    [SerializeField] private Image _shadow;
    [SerializeField] private Image _forground;
    private Sprite _defaultShadow;
    private void Awake()
    {
        if (_shadow == null)
            _shadow = GetComponent<Image>();
        _defaultShadow = _shadow.sprite;
        if (_forground == null)
            _forground = GetComponentsInChildren<Image>()[1];

        Clear();
    }

    public void InitializeGraphic(Sprite sprite) 
    {
        _forground.enabled = true;

        if (_shadow != null)
            _shadow.sprite = sprite;
        if (_forground != null)
            _forground.sprite = sprite;
    }

    public void Clear() 
    {
        _forground.enabled = false;
        _shadow.sprite = _defaultShadow;
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }

    public void SetColor(Color color) => _forground.color = color;
}
