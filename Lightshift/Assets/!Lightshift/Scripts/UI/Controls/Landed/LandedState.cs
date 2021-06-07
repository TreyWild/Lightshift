using Lightshift;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandedState : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    private void Awake()
    {
        player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);
    }

    void Update()
    {
        if (Input.GetKeyDown(Settings.DockKey))
        {
            if (player != null && player.IsLanded)
                player.TakeOff();
        }
    }

    public void LeaveStation()
    {
        player.TakeOff();
    }
}
