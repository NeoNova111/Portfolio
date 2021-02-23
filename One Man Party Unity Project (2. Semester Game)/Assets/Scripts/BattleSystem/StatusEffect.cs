using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect")]
public class StatusEffect : ScriptableObject
{
    [TextArea(2, 5)]
    public string description;
    [Range(0f, 1f)]
    public float damagePercent;
    public int turnDuration;
    public Color color;
}
