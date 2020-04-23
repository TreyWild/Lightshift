using UnityEngine;
using System.Collections;
using TMPro;
using System.Runtime.Versioning;

public class InventoryToolTip : MonoBehaviour
{
    [SerializeField] private GameObject _toolTipContent;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _lore;
    [SerializeField] private TextMeshProUGUI _type;
    [SerializeField] private TextMeshProUGUI _toolTipItemPrefab;
    public Vector3 offset;
    public void DisplayTooltip(Item item)
    {
        _title.color = item.color;
        _title.text = item.displayName;
        _lore.color = _title.color;
        _lore.text = item.lore;
        _type.text = item.type.ToString();

        if (item.data.acceleration != 0)
            AddModLine("Acceleration", item.data.acceleration);
        if (item.data.agility != 0)
            AddModLine("Agility", item.data.agility);
        if (item.data.brakeForce != 0)
            AddModLine("Brake Force", item.data.brakeForce);
        if (item.data.maxHealth != 0)
            AddModLine("Health", item.data.maxHealth);
        if (item.data.healthRegen != 0)
            AddModLine("Health Regen", item.data.healthRegen);
        if (item.data.maxPower != 0)
            AddModLine("Power", item.data.maxPower);
        if (item.data.powerRegen != 0)
            AddModLine("Power Regen", item.data.powerRegen);
        if (item.data.maxShield != 0)
            AddModLine("Shield", item.data.maxShield);
        if (item.data.shieldRegen != 0)
            AddModLine("Shield Regen", item.data.shieldRegen);
        if (item.data.maxSpeed != 0)
            AddModLine("Speed", item.data.maxSpeed);
        if (item.data.weight != 0)
            AddModLine("Weight", item.data.weight);
        if (item.data.lightLancePowerCost != 0)
            AddModLine("LightLance PowerCost", item.data.lightLancePowerCost);
        if (item.data.lightLancePullForce != 0)
            AddModLine("LightLance Force", item.data.lightLancePullForce);
        if (item.data.lightLanceRange != 0)
            AddModLine("LightLance Range", item.data.lightLanceRange);
        if (item.data.overDriveBoostMultiplier != 0)
            AddModLine("Overdrive Boost", item.data.overDriveBoostMultiplier);
        if (item.data.overDrivePowerCost != 0)
            AddModLine("Overdrive Powercost", item.data.overDrivePowerCost);
    }

    private void AddModLine(string line, float value) 
    {
        var item = Instantiate(_toolTipItemPrefab, _toolTipContent.transform).GetComponent<TextMeshProUGUI>();

        item.text = $"{value} {line}";
    }

    void Update()
    {
        _toolTipContent.transform.position = new Vector2(offset.x + Input.mousePosition.x, offset.y + Input.mousePosition.y);
    }
}
