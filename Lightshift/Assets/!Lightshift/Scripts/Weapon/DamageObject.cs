using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DamageObject 
{
    public bool IsDot;
    public bool IsStacking;
    public bool IsBurn;
    public float energyDamage;
    public float kineticDamage;
    public float corrosiveDamage;
    public float effectDuration;
    public float totalEffectDuration;
    public string weaponId;
    public int entityId;
    public float startTime;
}