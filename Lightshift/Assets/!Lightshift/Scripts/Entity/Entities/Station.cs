using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : Entity
{

    private List<Ship> _stationedShips = new List<Ship>();
    
    public StationType stationType;

    public void SetStation() 
    {
        SetStationGraphic(2);
    }

    private void SetStationGraphic(int stationGraphicId) 
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingOrder = SortingOrders.STATION;
        var sprite = PrefabManager.Instance?.Stations[stationGraphicId];

        if (sprite != null)
            renderer.sprite = sprite;
        else Debug.LogError("Sprite not assigned to station!");
    }
}
