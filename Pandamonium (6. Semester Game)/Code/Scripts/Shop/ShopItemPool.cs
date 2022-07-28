using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/ItemPool")]
public class ShopItemPool : ScriptableObject
{
    [System.Serializable]
    public class PoolHolder
    {
        public ShopItem item;
        public int weight = 1;
    }

    public int totalWeight = 0;
    public PoolHolder[] itemPool;

    private void OnValidate()
    {
        totalWeight = 0;
        foreach(var item in itemPool)
        {
            totalWeight += item.weight;
        }
    }

    public ShopItem DrawItemFromPool()
    {
        int rnd = Random.Range(0, totalWeight);

        int currentWeight = 0;
        ShopItem drawn = null;
        foreach(var itemHolder in itemPool)
        {
            currentWeight += itemHolder.weight;
            if (rnd <= currentWeight)
            {
                drawn = itemHolder.item;
                break;
            }
        }

        return drawn;
    }
}
