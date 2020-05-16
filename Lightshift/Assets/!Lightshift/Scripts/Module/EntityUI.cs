using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EntityUI : NetworkBehaviour
{
    private EntityUIControl _ui;
    private void Awake()
    {
        _ui = Instantiate(PrefabManager.Instance.entityUIPrefab, transform).GetComponent<EntityUIControl>();
    }

    public override void OnStartClient()
    {
        if (_ui != null && hasAuthority)
            _ui.gameObject.SetActive(false);
    }

    public void SetShield(float min, float max)
    {
        if (hasAuthority)
            GameUIManager.Instance.ShipInterface.SetShield(min, max);
        else _ui?.SetShield(min, max);
    }

    public void SetHealth(float min, float max)
    {
        if (hasAuthority)
            GameUIManager.Instance.ShipInterface.SetHealth(min, max);
        else _ui?.SetHealth(min, max);
    }

    public void SetPower(float min, float max)
    {
        if (hasAuthority)
            GameUIManager.Instance.ShipInterface.SetPower(min, max);
    }
    public void SetName(string name)
    {
        if (!hasAuthority)
            _ui?.SetName(name);
        //else _ui?.SetName("");
    }
}