using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(EntityUI))]
public class Shield : NetworkBehaviour 
{
    [SyncVar(hook = nameof(SetUIShieldMax))]
    public float maxShield;
    [SyncVar(hook = nameof(SetUIShield))]
    public float shield;
    [SyncVar]
    public float shieldRegen;
    [SyncVar]
    public float shieldSize;

    //private GameObject _visualShield;
    private EntityUI _ui;
    private Entity _entity;
    private void Awake()
    {
        _ui = GetComponent<EntityUI>();
        //_visualShield = Instantiate(PrefabManager.Instance.shieldPrefab, transform);
        //_visualShield.transform.localScale = new Vector3(shieldSize, shieldSize, 1);
        //var shieldImages = _visualShield.GetComponentsInChildren<SpriteRenderer>();
        //foreach (var shield in shieldImages)
        //    shield.sortingOrder = SortingOrders.SHIELD;

        //if (shield <= 0)
        //    _visualShield.SetActive(false);

        _entity = GetComponent<Entity>();
    }

    private void SetUIShield(float old, float newValue)
    {
        _ui.SetShield(newValue, maxShield);
    }

    private void SetUIShieldMax(float old, float newValue)
    {
        _ui.SetShield(shield, newValue);
    }

    public void SetShield(float value) 
    {
        if (!isServer)
            return;
        SetUIShield(shield, value);
        shield = value;
    }

    public void SetMaxShield(float value) 
    {
        if (!isServer)
            return;
        SetUIShieldMax(maxShield, value);
        maxShield = value;
    }

    public void Update()
    {
        if (isServer)
        {
            var shield = this.shield;
            if (shield >= maxShield)
            {
                shield = maxShield;
                return;
            }

            if (_entity.isInCheckpoint)
                shield += maxShield / 10 * Time.deltaTime;
            shield += shieldRegen * Time.deltaTime;

            if (shield >= maxShield)
                shield = maxShield;

            SetShield(shield);
        }

        //if (shield <= 0)
        //    _visualShield.SetActive(false);
        //else if (!_visualShield.activeInHierarchy) _visualShield.SetActive(true);
    }

    // TO DO : Consume power to use EXTRA shield
}