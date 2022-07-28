using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrencyType { Money, Health }

[CreateAssetMenu(menuName = "Shop/Item")]
public class ShopItem : ScriptableObject
{
    public int Cost { get => cost; }
    public CurrencyType Currency { get => currency; }
    public GameObject[] Drops { get => itemDrops; }
    public Sprite Sprite { get => sprite; }
    public float SpriteScale { get => spriteScale; }

    [SerializeField] private int cost = 10;
    [SerializeField] private CurrencyType currency = CurrencyType.Money;
    [SerializeField] private GameObject[] itemDrops;
    [SerializeField] private Sprite sprite;
    [SerializeField] private float spriteScale = 1f;
}
