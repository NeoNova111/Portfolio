using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/StatusAttack")]
public class StatusAttack : Attack
{
    public StatusEffect inflicts;
    [Range(0f, 1f)]
    public float inflictChance;
    
    public override void UseAbility(Unit target, Unit user)
    {
        base.UseAbility(target, user);
        if (target.statusEffect == null && Random.Range(0f, 1f) <= inflictChance + user.job.luck)
        {
            target.SetStatusEffect(inflicts);
        }
    }
}
