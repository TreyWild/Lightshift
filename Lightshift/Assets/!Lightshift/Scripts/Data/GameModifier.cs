using SharedModels.Models.Game;
using System;
using UnityEngine;

[Serializable]
public class GameModifier
{
    [Header("Base Value")]
    public float Value;
    [Header("Modifier Type")]
    public Modifier Type;
}