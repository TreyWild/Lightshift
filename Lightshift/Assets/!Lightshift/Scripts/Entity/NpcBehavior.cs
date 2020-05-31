using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NpcBehavior : MonoBehaviour 
{
    [HideInInspector]
    public Npc behavior;

    private void Awake()
    {
        behavior = GetComponent<Npc>();
    }
}