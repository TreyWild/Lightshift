using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class SystemMap : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _coordinateLabel;
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private GameObject _mapItemPrefab;
    [SerializeField] private GameObject _mapOrbitPrefab;
    [SerializeField] private Slider _zoomSlider;

    private List<MapObjectData> _objects = new List<MapObjectData>();
    private List<SolarSystemMapData> _solarObjects = new List<SolarSystemMapData>();

    public float Zoom = 10;

    private PlayerShip _playerShip;

    private void Awake()
    {
        _zoomSlider.value = PlayerPrefs.GetFloat("systemMapZoom", 8.5f);
        Zoom = _zoomSlider.value;
        _zoomSlider.onValueChanged.AddListener(delegate (float value)
        {
            PlayerPrefs.SetFloat("systemMapZoom", value);
            Zoom = value;
            PlayerPrefs.Save();
        });

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
                    var rect = mapOrbit.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2((orbit * 2) / Zoom, (orbit * 2) / Zoom);
                    var image = mapOrbit.GetComponent<Image>();
                    image.color = solarSystem.mapColor;
                    solarMap.uiObjects.Add(rect);
                }

                _solarObjects.Add(solarMap);
            }
        }
    }

    private void OnEnable()
    {
        _playerShip = FindObjectsOfType<PlayerShip>().ToList().Where(s => s.hasAuthority).FirstOrDefault();

        var mapObjects = FindObjectsOfType<MapObject>().ToList();

        foreach (var obj in _objects.ToList())
        {
            obj.uiMapObject.Init(obj.mapObject);

            // Destroy removed objects
            if (!mapObjects.Contains(obj.mapObject))
            {
                Destroy(obj.uiMapObject);
                Destroy(obj.mapObject);

                _objects.Remove(obj);
            }
            else mapObjects.Remove(obj.mapObject);
        }

        foreach (var obj in mapObjects)
        {
            var item = Instantiate(_mapItemPrefab, _contentPanel);
            var uiObj = item.GetComponent<UIMapObject>();

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
        if (_playerShip == null)
            return;

        _coordinateLabel.text = $"{Mathf.Round(GetLocalPosition().x)}, {Mathf.Round(GetLocalPosition().y)}";

        foreach (var obj in _objects)
        {
            obj.uiMapObject.transform.position = new Vector2((obj.mapObject.transform.position.x - GetLocalPosition().x) / Zoom + (Screen.width / 2), (obj.mapObject.transform.position.y - GetLocalPosition().y) / Zoom + (Screen.height / 2));
            obj.uiMapObject.SetRotation(obj.mapObject.transform.eulerAngles.z);
            obj.uiMapObject.SetSize(new Vector2((obj.mapObject.IconSize.x) / ((Zoom)/ 4), (obj.mapObject.IconSize.y) / ((Zoom) / 4)));
            obj.uiMapObject.Init(obj.mapObject);
        }

        foreach (var obj in _solarObjects)
        {

            var orbits = obj.solarSystem.OrbitSizes;
            orbits.Sort();
            orbits.Reverse();

            for (int i = 0; i < orbits.Count; i++)
            {
                var ui = obj.uiObjects[i];

                ui.sizeDelta = new Vector2((orbits[i] * 2) / Zoom, (orbits[i] * 2) / Zoom);
                ui.transform.position = new Vector2((obj.solarSystem.transform.position.x - GetLocalPosition().x) / Zoom + (Screen.width / 2), (obj.solarSystem.transform.position.y - GetLocalPosition().y) / Zoom + (Screen.height/2));
            }
        }
    }

    private Vector2 GetLocalPosition()
    {
        if (_playerShip == null)
            return Vector2.zero;
        else return _playerShip.kinematic.position;
    }

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