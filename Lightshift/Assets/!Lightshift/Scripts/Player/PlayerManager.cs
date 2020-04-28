using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private List<PlayerData> _players = new List<PlayerData>();
    public static PlayerManager Instance { get; set; }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(Instance);
    }

    public void AddPlayer(PlayerData player)
    {
        if (!_players.Contains(player))
        {
            ChatBox.Instance.AddMessage($"<i>{player.username} entered the system.</i>");
            _players.Add(player);
        }

        GameUIManager.Instance.TryUpdatePlayerMenu();
    }

    public void RemovePlayer(PlayerData player)
    {
        ChatBox.Instance.AddMessage($"<i>{player.username} has left.</i>");

        if (_players.Contains(player))
            _players.Remove(player);
    }

    public List<PlayerData> GetAllPlayers()
    {
        return _players;
    }

    public PlayerData GetPlayer(string id) 
    {
        return _players.FirstOrDefault(p => p.userId == id);
    }
}
