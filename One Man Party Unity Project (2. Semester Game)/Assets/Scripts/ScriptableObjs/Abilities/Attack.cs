using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Attack")]
public class Attack : Ability
{
    public float damage;
    public bool aoe = false;

    public override void UseAbility(Unit target, Unit user)
    {
        if (damageType == DamageType.TRUE)
        {
            target.TakeDamage((int)damage);
        }
        else if(!aoe)
        {
            target.TakeDamage(CalcDamage(target, user));
        }
        else
        {
            foreach (Unit enemy in BattleSystem.instance.enemies)
            {
                enemy.TakeDamage(CalcDamage(enemy, user));
            }
        }
    }

    //Balance the shit out of this, somehow get attack into this, level?
    //include armor in calculation
    public int CalcDamage(Unit target, Unit user)
    {
        float uAttack = user.job.attack; /* * user.GetAttackMultiplier();*/
        //Debug.Log(user.job.attack);
        //Debug.Log(uAttack);

        float tDefense = target.job.defense * target.GetDefenseMultiplier();
        float tResistance = target.job.resistance * target.GetResistanceMultiplier();

        float rawDamage = damage * user.GetAttackMultiplier();
        if (damageType == DamageType.MAGIC)
        {
            rawDamage = rawDamage * (uAttack / (float)(uAttack + tResistance)); //temp damage formula
        }
        else if(damageType == DamageType.NORMAL)
        {
            rawDamage = rawDamage * (uAttack / (float)(uAttack + tDefense));
        }

        Debug.Log(rawDamage); //for testing

        if(target.armor > 0)
            rawDamage = target.TakeArmorDamage((int)rawDamage);

        if (rawDamage <= 0)
        {
            return 0;
        }

        return (int)rawDamage;
    }
}
