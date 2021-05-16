using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeResetView : MonoBehaviour
{
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private GameObject _resourcePrefab;

    public Action OnReset;
    public Action OnExit;


    public void Init(List<ResourceObject> resources) 
    {
        if (resources == null)
        {
            DialogManager.ShowMessage("You have no upgrades for this item.", delegate () 
            {
                Destroy(gameObject);
            });
            return;
        }

        foreach (var resource in resources)
        {
            var obj = Instantiate(_resourcePrefab, _contentPanel);
            var script = obj.GetComponent<ResourceItemControl>();
            script.Init(resource.Type, resource.Amount);
        }
    }

    public void Reset()
    {
        DialogManager.ShowDialog("Are you sure you want to reset all your upgrades?", delegate (bool result) 
        {
            if (result)
            {
                OnReset?.Invoke();
            }
            Exit();
        });
    }
    public void Exit() 
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        _contentPanel = null;
        _resourcePrefab = null;
        OnReset = null;
        OnExit = null;
    }
}
