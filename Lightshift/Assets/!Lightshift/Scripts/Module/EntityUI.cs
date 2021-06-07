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
    private bool isLocalPlayer;
    private void Start()
    {
        if (_ui == null && !isLocalPlayer)
        {
            _ui = Instantiate(PrefabManager.Instance.entityUIPrefab, transform).GetComponent<EntityUIControl>();

            var entity = GetComponent<Entity>();
            _ui.SetName(entity.displayName);
        }
    }

    public void InitLocalPlayer(bool isLocalPlayer)
    {
        this.isLocalPlayer = isLocalPlayer;
        if (_ui != null && isLocalPlayer)
            _ui.gameObject.SetActive(false);
    }

    public void SetShield(float min, float max)
    {
        if (isLocalPlayer)
            GameUIManager.Instance.ShipInterface.SetShield(min, max);
        else _ui?.SetShield(min, max);
    }

    public void SetHealth(float min, float max)
    {
        if (isLocalPlayer)
            GameUIManager.Instance.ShipInterface.SetHealth(min, max);
        else _ui?.SetHealth(min, max);
    }

    public void SetPower(float min, float max)
    {
        if (isLocalPlayer)
            GameUIManager.Instance.ShipInterface.SetPower(min, max);
    }
    public void SetName(string name)
    {
        if (!isLocalPlayer)
            _ui?.SetName(name);
        //else _ui?.SetName("");
    }

    public void SetTeam(bool isTeam) 
    {
        _ui?.SetTeam(isTeam);
    }
}