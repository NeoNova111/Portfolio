using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private BaseWeapon weaponToPickup;
    [SerializeField] [TextArea(2, 5)] private string interactText = "[E] to equip ";
    public string InteractText { get => interactText + weaponToPickup.name; }

    [SerializeField] private bool interactable = true;
    public bool Interactable { get => interactable; }

    [SerializeField] private bool interacting = false;
    public bool Interacting { get => interacting; }

    [SerializeField] private Transform transformOverwrite;
    public Transform Transform { get => transformOverwrite ? transformOverwrite : transform; }

    [SerializeField] private GameObject heldGraphics;
    [SerializeField] private GameObject groundGraphics;
    public Sprite WeaponSprite { get => groundGraphics.GetComponent<SpriteRenderer>().sprite; }

    private Rigidbody rb;
    private Collider coll;

    public void EndInteract()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        WeaponsManager.Instance.AddToWeaponInventory(weaponToPickup);
        enabled = false;
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();        
    }

    private void OnEnable()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        coll.enabled = true;
        groundGraphics.SetActive(true);
        heldGraphics.SetActive(false);
    }

    private void OnDisable()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        coll.enabled = false;
        groundGraphics.SetActive(false);
        heldGraphics.SetActive(true);
    }
}
