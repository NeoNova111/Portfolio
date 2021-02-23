using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Race")]
public class Race : ScriptableObject
{
    public new string name;
    public Sprite apperance;
    public Sprite turnIcon;
    [Range(0f,1f)]
    public float statMultiplier = 1;
}
