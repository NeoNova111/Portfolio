using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Buff")]
public class Buff : Ability
{
    public bool aoe = false; //generalize aoe when time
    [Range(0.5f, 2f)]
    public float attackMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float defenseMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float resistanceMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float speedMultiplier = 1f;

    public override void UseAbility(Unit target, Unit user)
    {
        if (aoe && abilityType == AbilityType.DEBUFF)
        {
            foreach(Unit enemy in BattleSystem.instance.enemies)
            {
                ApplyBuff(enemy);
            }
        }
        else
        {
            ApplyBuff(target);
        }
    }

    void ApplyBuff(Unit target)
    {
        target.attackMultiplier *= this.attackMultiplier;
        target.defenseMultiplier *= this.defenseMultiplier;
        target.resistanceMultiplier *= this.resistanceMultiplier;
        target.speedMultiplier *= this.speedMultiplier;
    }
}
