using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierScript : MonoBehaviour
{
    [SerializeField] Hand lowerHand;
    [SerializeField] Hand upperHand;

    public void PickUpLower(Transform toPickUp)
    {
        PickUp(lowerHand, toPickUp);
    }

    public void PickUpUpper(Transform toPickUp)
    {
        PickUp(upperHand, toPickUp);
    }

    void PickUp(Hand pickingUp, Transform toPickUp)
    {
        pickingUp.PickUp(toPickUp);
    }

    public void DiscardLower(Transform target)
    {
        Discard(lowerHand, target);
    }

    public void DiscardUpper(Transform target)
    {
        Discard(upperHand, target);
    }

    void Discard(Hand discarding, Transform toDiscard)
    {
        discarding.Discard(toDiscard);
    }

    public void HoldLower()
    {
        lowerHand.Hold();
    }

    public void HoldUpper()
    {
        upperHand.Hold();
    }
}
