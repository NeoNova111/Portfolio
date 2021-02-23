using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    ATTACK,
    BUFF,
    DEBUFF,
    DEFENSIVE
}

public enum DamageType
{
    NORMAL,
    MAGIC,
    TRUE
}

[System.Serializable]
public abstract class Ability : ScriptableObject
{
    public new string name;
    public AbilityType abilityType;
    public DamageType damageType;
    [TextArea(2, 5)]
    public string description;
    public int castTime;
    [Range(1,11)]
    public int cooldown = 1;

    public virtual void UseAbility(Unit target, Unit user) { }
}
