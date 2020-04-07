using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Lightshift;

public class RespawnUI : MonoBehaviour
{
    public TextMeshProUGUI _respawnLabel;
    public TextMeshProUGUI _killedByLabel;

    public Action onRespawn;

    private float _respawnTime;
    private string _deathCause;
    public void Initialize(float respawnTime, string deathCause)
    {
        _respawnTime = respawnTime;
        _deathCause = deathCause;

        _killedByLabel.text = $"Destroyed by {deathCause}";
    }

    public void Update()
    {
        _respawnTime -= 1.0f * Time.deltaTime;

        if (_respawnTime < 1 && !_canRespawn)
            SetRespawnable();

        if (_canRespawn)
        {
            if (Input.GetKeyDown(Settings.Instance.RespawnKey))
                Respawn();
        }
        else _respawnLabel.text = $"Respawning in {Mathf.RoundToInt(_respawnTime)} Seconds";
    }

    private bool _canRespawn = false;
    private void SetRespawnable() 
    {
        _canRespawn = true;
        _respawnLabel.text = $"Press {Settings.Instance.RespawnKey} to Respawn";
    }

    private void Respawn()
    {
        _killedByLabel.text = "Respawning";
        _respawnLabel.text = $"Waiting for Server";
        onRespawn?.Invoke();
    }
}
