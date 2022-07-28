using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPowerUp : Item
{
    public float healthGain = 0f;
    public int armor = 0;
    public TempBuff[] buffs;
    public PermaBuff[] permaBuffs;

    public override void PickUp()
    {
        ApplyPowerup();

        base.PickUp();
    }

    private void ApplyPowerup()
    {
        PlayerController player = PlayerController.Instance;
        if (!player) return;

        foreach (TempBuff buff in buffs)
        {
            player.stats.GetCorrispondingStatToBuffType(buff.buffType).SetBuff(buff);
        }

        foreach (PermaBuff buff in permaBuffs)
        {
            player.stats.AddPermanentBuff(buff);
        }

        if (healthGain > 0)
        {
            player.Heal(healthGain);
        }

        if (armor > 0)
        {
            player.stats.ArmorStacks += armor;
        }

    }
}
