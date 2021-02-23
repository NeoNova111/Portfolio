using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Healing Ability/Heal")]
public class HealingAbility : Ability
{
    [Range(0f, 1f)]
    public float recoverPercentage;

    public override void UseAbility(Unit target, Unit user)
    {
        target.HealUnit((int)(target.maxHealth * recoverPercentage));
    }
}