using Assets._Lightshift.Scripts.Utilities;
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

    public void SetTeam(bool isTeam) 
    {
        if (isTeam)
            nameTag.color = ColorHelper.GetTeamColor();
        else nameTag.color = ColorHelper.GetEnemyColor();
    }
}

