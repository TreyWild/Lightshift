using SharedModels.Models.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatView : MonoBehaviour
{
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _itemPrefab;

    private List<StatViewControl> _controls = new List<StatViewControl>();
    public void AddStat(GameModifier modifier) 
    {
        var item = Instantiate(_itemPrefab, _contentTransform).GetComponent<StatViewControl>();
        item.Setup(modifier);

        _controls.Add(item);
    }

    public void Clear() 
    {
        foreach (var stat in _controls)
            Destroy(stat.gameObject);

        _controls.Clear();
    }

    public void RemoveStat(Modifier modifier) 
    {
        var stat = _controls.FirstOrDefault(m => m.GetModifier().Type == modifier);
        if (stat == null)
            return;

        _controls.Remove(stat);
        Destroy(stat.gameObject);
    }
}
