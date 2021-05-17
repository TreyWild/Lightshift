using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SystemMap : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _coordinateLabel;
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private GameObject _mapItemPrefab;
    [SerializeField] private GameObject _mapOrbitPrefab;
    [SerializeField] private Transform _playerIcon;
    private List<MapObjectData> _objects = new List<MapObjectData>();
    private List<SolarSystemMapData> _solarObjects = new List<SolarSystemMapData>();

    public float Zoom = 10;

    private PlayerShip _ship;

    

    private void Awake()
    {
        var player = FindObjectOfType<Player>();
        _ship = player.ship;

        var solarSystems = FindObjectsOfType<SolarSystem>();
        if (solarSystems != null)
        {
            foreach (var solarSystem in solarSystems)
            {
                var orbits = solarSystem.OrbitSizes;
                orbits.Sort();
                orbits.Reverse();

                var solarMap = new SolarSystemMapData();
                solarMap.solarSystem = solarSystem;
                solarMap.uiObjects = new List<RectTransform>();

                foreach (var orbit in orbits)
                {
                    var mapOrbit = Instantiate(_mapOrbitPrefab, _contentPanel);
                    mapOrbit.transform.position = solarSystem.transform.position / Zoom;
                    var rect = mapOrbit.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2((orbit * 2) / Zoom, (orbit * 2) / Zoom);
                    mapOrbit.transform.position = new Vector2((solarSystem.transform.position.x) / Zoom + (Screen.width / 2), (solarSystem.transform.position.y) / Zoom + (Screen.height / 2));

                    solarMap.uiObjects.Add(rect);
                }

                _solarObjects.Add(solarMap);
            }
        }
        var mapObjects = FindObjectsOfType<MapObject>();
        if (mapObjects != null)
            foreach (var obj in mapObjects)
            {
                var item = Instantiate(_mapItemPrefab, _contentPanel);
                var uiObj = item.GetComponent<UIMapObject>();

                uiObj.transform.position = new Vector2((obj.transform.position.x) / Zoom + (Screen.width / 2), (obj.transform.position.y) / Zoom + (Screen.height / 2));
                uiObj.Init(obj);

                _objects.Add(new MapObjectData
                {
                    mapObject = obj,
                    uiMapObject = uiObj,
                });
            }
    }
    private void FixedUpdate()
    {
        _playerIcon.transform.eulerAngles = new Vector3(0, 0, _ship.kinematic.rotation);

        _coordinateLabel.text = $"{Mathf.Round(_ship.kinematic.Transform.position.x)}, {Mathf.Round(_ship.kinematic.Transform.position.y)}";

        foreach (var obj in _objects)
            obj.uiMapObject.transform.position = new Vector2((obj.mapObject.transform.position.x - GetLocalPosition().x) / Zoom + (Screen.width / 2), (obj.mapObject.transform.position.y - GetLocalPosition().y) / Zoom + (Screen.height / 2));

        foreach (var obj in _solarObjects)
        {

            var orbits = obj.solarSystem.OrbitSizes;
            orbits.Sort();
            orbits.Reverse();

            for (int i = 0; i < orbits.Count; i++)
            {
                var ui = obj.uiObjects[i];

                ui.sizeDelta = new Vector2((orbits[i] * 2) / Zoom, (orbits[i] * 2) / Zoom);
                ui.transform.position = new Vector2((obj.solarSystem.transform.position.x - GetLocalPosition().x) / Zoom + (Screen.width / 2), (obj.solarSystem.transform.position.y - GetLocalPosition().y) / Zoom + (Screen.height / 2));
            }
        }
    }

    private Vector2 GetLocalPosition() => _ship.kinematic.position;
    public void Exit()
    {
        GameUIManager.Instance.ToggleSystemMap();
    }
}

public class MapObjectData 
{
    public MapObject mapObject;

    public UIMapObject uiMapObject;
}

public class SolarSystemMapData
{
    public SolarSystem solarSystem;

    public List<RectTransform> uiObjects;
}