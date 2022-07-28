using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : Item
{
    [SerializeField, Range(0, 100)] private int moneyAmount;
    [SerializeField] private GameEvent moneypickedUp;

    public override void PickUp()
    {
        if (PlayerController.Instance) PlayerController.Instance.money.Money += moneyAmount;
        moneypickedUp.Raise();

        base.PickUp();
    }
}
