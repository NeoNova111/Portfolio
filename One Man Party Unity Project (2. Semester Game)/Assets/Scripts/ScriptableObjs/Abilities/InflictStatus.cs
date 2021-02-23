using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Inflict Status")]
public class InflictStatus : Ability
{
    public StatusEffect inflicts;
    [Range(0f, 1f)]
    public float inflictChance;

    public override void UseAbility(Unit target, Unit user)
    {
        if (target.statusEffect == null && Random.Range(0f, 1f) <= inflictChance + user.job.luck)
        {
            target.SetStatusEffect(inflicts);
        }
    }
}