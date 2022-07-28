using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    [SerializeField] private ShopItemPool itemPool;
    [SerializeField] private Transform itemSpawn;
    //[SerializeField] private Transform itemSpawn;
    [SerializeField] private MoneyInstance purse;
    [SerializeField] private GameEvent itembought;

    private ShopItemHolder[] holders;

    private void Awake()
    {
        holders = GetComponentsInChildren<ShopItemHolder>();
    }

    private void Start()
    {
        PopulateItems();
    }

    public void PopulateItems()
    {
        foreach(var holder in holders)
        {
            holder.SetItem(itemPool.DrawItemFromPool());
        }
    }

    public void BuyItem(ShopItem item, GameObject obj)
    {
        if (item.Currency == CurrencyType.Health)
        {
            PlayerController.Instance.TakeDirectDamage(item.Cost);
        }
        if (item.Currency == CurrencyType.Money)
        {
            if (purse.Money < item.Cost) return;

            purse.Money -= item.Cost;
        }

        for (int i = 0; i < item.Drops.Length; i++)
        {
            Instantiate(item.Drops[i], itemSpawn.position, Quaternion.identity);
        }

        itembought.Raise();
        obj.SetActive(false);
    }
}
