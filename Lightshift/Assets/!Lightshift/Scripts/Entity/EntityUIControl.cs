using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityUIControl : MonoBehaviour
{
    public Transform containerTransform;

    public TextMeshProUGUI nameTag;
    public Slider healthBar;
    public Slider shieldBar;

    public Vector3 positionOffset;
    private void Start()
    {
        transform.position = new Vector3();
    }
    public void SetName(string n)
    {
        nameTag.text = n;
    }

    private void Update()
    { 
        transform.localPosition = positionOffset;
        transform.localRotation = Quaternion.Euler(new Vector3());
        transform.rotation = Quaternion.Euler(new Vector3());
    }

    public void SetShield(float currentShield = 0, float maxShield = 0) => shieldBar.value = (currentShield / maxShield) * 1.0f;


    public void SetHealth(float currentHealth = 0, float maxHealth = 0) => healthBar.value = (currentHealth / maxHealth) * 1.0f;
}

public class EntityUI : NetworkBehaviour
{
    private EntityUIControl _ui;
    private void Start()
    {
        var _ui = Instantiate(PrefabManager.Instance.entityUIPrefab, transform).GetComponent<EntityUIControl>();
    }

    public override void OnStartLocalPlayer()
    {
        if (_ui != null)
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
            _ui.SetName(name);
    }
}