using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FearType { DEPTH, DARKNESS, UNKOWN, SCALE, TIGHTNESS}

[CreateAssetMenu(menuName = "Fear")]
public class Fear : ScriptableObject
{
    public FearType phobia;
    [Range(0, 1)]
    public float intensity;
}
