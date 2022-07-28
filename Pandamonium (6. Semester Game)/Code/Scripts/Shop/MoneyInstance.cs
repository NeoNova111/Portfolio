using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Money")]
public class MoneyInstance : ScriptableObject
{
   [SerializeField] private int money;
   public int Money { get => money;set => money = value; }
}
