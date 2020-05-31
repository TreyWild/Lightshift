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

        foreach (PlayerData player in players)
        {
            CreatePlayerItem(player);
        }
    }

    public void CreatePlayerItem(PlayerData player)
    {
        var item = Instantiate(playerItemPrefab, contentPanel.transform);
        var script = item.GetComponent<PlayerMenuItem>();

        script.SetUsername(player.username);

        _playerListObjects.Add(item);
    }

    public void Exit()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.PlayerMenuKey))
            Exit();
    }
}
