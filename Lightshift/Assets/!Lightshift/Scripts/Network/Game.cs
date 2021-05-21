using UnityEngine;
using Mirror;

public class Game : MonoBehaviour
{
    public static Game Instance { get; set; }
    public void Awake()
    {
        Instance = this;
    }
}



