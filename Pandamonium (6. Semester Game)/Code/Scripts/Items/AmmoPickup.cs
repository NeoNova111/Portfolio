using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Item
{
    [SerializeField] private WeaponType ammoType;
    [SerializeField][Range(0, 1)] private float refillPercentage;

    public override void PickUp()
    {
        WeaponsManager manager = WeaponsManager.Instance;
        manager.RefillReserves(ammoType, refillPercentage);
        base.PickUp();
    }
}
