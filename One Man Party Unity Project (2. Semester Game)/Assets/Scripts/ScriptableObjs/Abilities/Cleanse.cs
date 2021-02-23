using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName= "Ability/Healing Ability/Cleanse")]
public class Cleanse : HealingAbility
{
    public override void UseAbility(Unit target, Unit user)
    {
        base.UseAbility(target, user);

        if (target.statusEffect != null)
        {
            target.ClearStatusEffect();
        }
    }
}
