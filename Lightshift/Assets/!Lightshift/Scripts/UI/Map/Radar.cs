using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Radar : MonoBehaviour
{
    [SerializeField] private Sprite playerIcon;
    [SerializeField] private GameObject _radarField;
    [SerializeField] private TextMeshProUGUI _coordinatesLabel;
    private Dictionary<Entity, GameObject> _mapIcons = new Dictionary<Entity, GameObject>();

    public float mapScale;

    private void AddToRadar(Entity entity)
    {
        var icon = _mapIcons[entity];
        if (icon != null)
        {
            Destroy(icon);
            _mapIcons[entity] = null;
        }

        icon = new GameObject(entity.displayName);
        icon.transform.parent = _radarField.transform;

        var image = icon.AddComponent<Image>();

        if (entity.hasAuthority)
            image.sprite = entity.mapIcon;
        else image.sprite = entity.mapIcon;
        _mapIcons[entity] = icon;
    }

    private void RemoveFromRadar(Entity entity)
    {
        var icon = _mapIcons[entity];
        if (icon != null)
        {
            Destroy(icon);
            _mapIcons[entity] = null;
        }
    }
    private void Update()
    {
        ConfirmMapAccuracy();

        foreach (var icon in _mapIcons) 
        {
            var entity = icon.Key;

            if (entity.hasAuthority)
                _coordinatesLabel.text = $"Y: {Mathf.Round(entity.transform.position.x)}, X: {Mathf.Round(entity.transform.position.y)}";


            var angle = Mathf.Atan2(entity.transform.position.y - transform.position.y, entity.transform.position.x - transform.position.x) - Mathf.PI * 0.5f;


            icon.Key.transform.position = new Vector3(Mathf.Cos(angle) / mapScale + _radarField.transform.position.x, Mathf.Sin(angle) / mapScale + _radarField.transform.position.y, 0f);
            icon.Key.transform.eulerAngles = new Vector3(0, 0, entity.transform.eulerAngles.z);
        }
    }

    private void ConfirmMapAccuracy() 
    {
        if (_mapIcons.Count != EntityManager.EntityCount)
        {
            foreach (var icon in _mapIcons)
                Destroy(icon.Value.gameObject);

            _mapIcons.Clear();

            foreach (var entity in EntityManager.GetAllEntities())
                AddToRadar(entity);
        }
    }
}
