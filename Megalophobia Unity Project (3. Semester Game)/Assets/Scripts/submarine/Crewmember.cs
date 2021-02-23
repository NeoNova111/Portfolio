using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crewmember")]
public class Crewmember : ScriptableObject
{
    public string memberName;
    public Sprite[] icons;

    public FearType[] typeOfFear;
    [Range(0, 100)]
    public float sanity = 100;
    public float maxSanity = 100;
    [Range(0, 1)]
    public float currentFearIntensity = 0;
    [Range(0, 1)]
    public float potentialFearIntensity = 0;
}
