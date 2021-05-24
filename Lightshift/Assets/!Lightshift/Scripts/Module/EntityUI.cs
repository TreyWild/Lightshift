using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntityUI : MonoBehaviour
{
    private EntityUIControl _ui;
    private bool hasAuthority;
    private bool isServer;
    private void Awake()
    {
        if (_ui != null)
            _ui = Instantiate(PrefabManager.Instance.entityUIPrefab, transform).GetComponent<EntityUIControl>();
    }

    public void Init(bool hasAuthority = false, bool isServer = false, string displayName = "")
    {
        this.hasAuthority = hasAuthority;
        this.isServer = isServer;

        if (isServer)
            return;

        if (_ui != null && hasAuthority)
            Destroy(_ui.gameObject);
        else if (_ui == null && !hasAuthority)
            _ui = Instantiate(PrefabManager.Instance.entityUIPrefab, transform).GetComponent<EntityUIControl>();

        SetName(displayName);
    }

    public void SetShield(float min, float max)
    {
        if (isServer)
            return;

        if (hasAuthority)
            GameUIManager.Instance.ShipInterface.SetShield(min, max);
        else _ui?.SetShield(min, max);
    }

    public void SetHealth(float min, float max)
    {
        if (isServer)
            return;

        if (hasAuthority)
            GameUIManager.Instance.ShipInterface.SetHealth(min, max);
        else _ui?.SetHealth(min, max);
    }

    public void SetPower(float min, float max)
    {
        if (isServer)
            return;

        if (hasAuthority)
            GameUIManager.Instance.ShipInterface.SetPower(min, max);
    }
    public void SetName(string name)
    {
        if (isServer)
            return;

        if (!hasAuthority)
            _ui?.SetName(name);
        //else _ui?.SetName("");
    }

    public void SetTeam(bool isTeam) 
    {
        _ui?.SetTeam(isTeam);
    }
}