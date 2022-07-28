using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    bool Pickupable { get; }
    Transform TargetTransform { get; }
    void PickUp();
}