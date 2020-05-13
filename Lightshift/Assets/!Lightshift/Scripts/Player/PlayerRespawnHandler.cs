using Lightshift;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerRespawnHandler : NetworkBehaviour
{
    [SyncVar]
    private float _timeRemaining = 5.3f;

    public TextMeshProUGUI _respawnLabel;
    public TextMeshProUGUI _killedByLabel;

    private void Start()
    {
        if (!hasAuthority)
            GetComponent<Canvas>().enabled = false;
    }
    public void Initialize(float respawnTime, string deathCause)
    {
        _timeRemaining = respawnTime;

        TargetRpcInit(deathCause);
    }

    [TargetRpc]
    public void TargetRpcInit(string deathCause)
    {
        _killedByLabel.text = $"Destroyed by {deathCause}";

        GameUIManager.Instance.HandleRespawnScreen(true);
    }

    public void Update()
    {
        if (isServer)
            _timeRemaining -= 1.0f * Time.deltaTime;

        if (!hasAuthority)
            return;

        if (_timeRemaining < 1 && !_canRespawn)
            SetRespawnable();

        if (_canRespawn)
        {
            if (Input.GetKeyDown(Settings.Instance.RespawnKey))
                Respawn();
        }
        else _respawnLabel.text = $"Respawning in {Mathf.RoundToInt(_timeRemaining)} Seconds";
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

        CmdRespawn();
    }

    [Command]
    public void CmdRespawn()
    {
        if (_timeRemaining <= 1)
        {
            var player = Server.GetPlayer(connectionToClient);

            var ship = Instantiate(NetworkManager.singleton.playerPrefab, player.lastSafePosition, transform.rotation);

            TargetRpcSpawn();

            NetworkServer.Spawn(ship, connectionToClient);

            NetworkServer.Destroy(gameObject);
        }
    }

    [TargetRpc]
    public void TargetRpcSpawn() 
    {
        GameUIManager.Instance.HandleRespawnScreen(false);
    }
}

