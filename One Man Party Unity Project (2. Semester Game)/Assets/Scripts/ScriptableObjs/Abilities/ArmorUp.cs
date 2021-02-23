using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Armor Up")]
public class ArmorUp : Ability
{
    public int armorAddValue;

    public override void UseAbility(Unit target, Unit user)
    {
        target.armor += armorAddValue;
    }
}
