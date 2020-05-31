using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DamageTextHandler : MonoBehaviour
{
    [SerializeField] GameObject _damageTextPrafab;
    [SerializeField] GameObject _dropTextPrefab;
    [SerializeField] GameObject _canvas;

    private static DamageTextHandler _instance;

    private List<GameObject> _dropTextList = new List<GameObject>();
    private List<GameObject> _textList = new List<GameObject>();
    private List<DamageTextObject> _objects = new List<DamageTextObject>();

    private float _lifeTime = .5f;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        for (int i = 0; i < _objects.Count; i++) 
        {
            _objects[i]._lifeTime -= Time.deltaTime;
            if (_objects[i]._lifeTime <= 0)
            {
                _objects[i].damageText.gameObject.SetActive(false);

                PlayDropEffect(_objects[i].currentDamage, _objects[i]._targetTransform.position);
                _objects.Remove(_objects[i]);
            }
            else 
            {
                _objects[i].damageText.transform.position = _objects[i]._targetTransform.position;
            }
        }
    }

    public static void AddDamage(Entity entity, float damage) 
    {
        if (entity == null)
            return;

        var obj = _instance._objects.FirstOrDefault(e => e.entityId == entity.Id);
        if (obj == null)
        {
            obj = new DamageTextObject
            {
                entityId = entity.Id,
                currentDamage = damage,
                damageText = _instance.GetText(),
                _lifeTime = _instance._lifeTime
            };

            obj.damageText.text = $"{damage}";
            obj.damageText.transform.position = entity.transform.position;
            obj._targetTransform = entity.transform;

            _instance._objects.Add(obj);
        }
        else 
        {
            obj._lifeTime = 1;
            obj.damageText.text = $"{obj.currentDamage + damage}";
            obj.currentDamage += damage;
        }
    }

    private TextMeshProUGUI GetText() 
    {
        var obj = _textList.FirstOrDefault(d => !d.activeInHierarchy);
        if (obj == null)
            obj = Instantiate(_damageTextPrafab, _canvas.transform);
        else obj.SetActive(true);

        if (!_textList.Contains(obj))
            _textList.Add(obj);

        return obj.GetComponent<TextMeshProUGUI>();
    }

    private void PlayDropEffect(float damage, Vector2 position) 
    {
        var obj = GetUsableDropText();
        obj.transform.position = position;

        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = damage.ToString();
        obj.gameObject.SetActive(true);
    }
    private GameObject GetUsableDropText() 
    {
        var obj = _dropTextList.FirstOrDefault(d => !d.activeInHierarchy);
        if (obj == null)
            obj = Instantiate(_dropTextPrefab, _canvas.transform);

        if (!_dropTextList.Contains(obj))
            _dropTextList.Add(obj);
        return obj;
    }
}

public class DamageTextObject 
{
    public TextMeshProUGUI damageText;
    public Transform _targetTransform;
    public short entityId;
    public float currentDamage;
    public float _lifeTime;
}
