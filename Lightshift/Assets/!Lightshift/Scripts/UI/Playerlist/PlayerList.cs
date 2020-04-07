using Lightshift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour {

    public GameObject playerItemPrefab;
    public GameObject contentPanel;

    private List<GameObject> _playerListObjects = new List<GameObject>();
    public void Start()
    {
        Settings.Instance.KeysLocked = true;

        ShowOnlinePlayers();
    }

    public void ShowOnlinePlayers() 
    {
        var players = PlayerManager.Instance.GetAllPlayers();

        foreach (var item in _playerListObjects)
        {
            Destroy(item);
        }

        _playerListObjects.Clear();

        foreach (Player player in players)
        {
            CreatePlayerItem(player);
        }
    }

    public void CreatePlayerItem(Player player)
    {
        var item = Instantiate(playerItemPrefab, contentPanel.transform);
        var script = item.GetComponent<PlayerMenuItem>();

        script.SetUsername(player.Username);

        _playerListObjects.Add(item);
    }

    public void Exit()
    {
        Settings.Instance.KeysLocked = false;
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.Instance.PlayerMenuKey))
            Exit();
    }
}
