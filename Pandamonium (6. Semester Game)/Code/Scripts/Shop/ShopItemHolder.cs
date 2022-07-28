using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemHolder : MonoBehaviour, IInteractable
{
    public ShopItem HeldItem { get => item; }
    public bool Interactable { get => interactable; set => interactable = value; }
    public bool Interacting { get => interacting; }
    public string InteractText { get => interactText + item.name + " for " + item.Cost + " " + item.Currency.ToString(); }
    public Transform Transform { get => transformOverride ? transformOverride : transform; }

    [SerializeField] private ShopItem item;
    [SerializeField] [TextArea(2, 5)] private string interactText;
    [SerializeField] private Transform transformOverride;
    [SerializeField] private ShopInventory inv;

    private bool interactable = false;
    private bool interacting = false;

    private SpriteRenderer spriteRenderer;

    private void Update()
    {
        interactable = BeingLookedAt();
    }

    public void SetItem(ShopItem item)
    {
        if(!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!item) return;

        this.item = item;
        spriteRenderer.sprite = item.Sprite;
        transform.localScale = Vector3.one * item.SpriteScale;
    }

    private bool BeingLookedAt()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    public void Interact()
    {
        inv.BuyItem(item, gameObject);
    }

    public void EndInteract()
    {

    }
}
