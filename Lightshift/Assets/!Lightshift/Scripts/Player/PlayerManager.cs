using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private List<Player> _players = new List<Player>();
    public static PlayerManager Instance { get; set; }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(Instance);
    }

    public void AddNewPlayer(Player player)
    {
        _players.Add(player);
        ChatBox.Instance.AddMessage($"<i>{player.Username} entered the system.</i>");

        GameUIManager.Instance.TryUpdatePlayerMenu();
    }

    public void AddPlayer(Player player) 
    {
        _players.Add(player);

        GameUIManager.Instance.TryUpdatePlayerMenu();
    }

    public void RemovePlayer(Player player)
    {
        if (player != null)
        {
            ChatBox.Instance.AddMessage($"<i>{player.Username} has left.</i>");
            _players.Remove(player);
        }
    }

    public List<Player> GetAllPlayers()
    {
        return _players;
    }
}
