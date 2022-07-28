using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Item : MonoBehaviour, IPickupable
{
    public string itemDescription;

    [SerializeField] private Transform transformOverride;
    public Transform TargetTransform { get => transformOverride ? transformOverride : transform; }

    [SerializeField] private bool pickupable = false;
    public bool Pickupable { get => pickupable; }

    public virtual void PickUp()
    {
        UIManager ui = UIManager.Instance;
        if (ui)
        {
            ui.ToolTipPopUp(3f, itemDescription);
        }

        Destroy(gameObject);
    }
}
