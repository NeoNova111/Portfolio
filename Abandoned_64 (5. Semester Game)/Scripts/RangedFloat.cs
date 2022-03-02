using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RangedFloat
{
    public RangedFloat(float min, float max)
    {
        minValue = min;
        maxValue = max;
    }

    public float minValue;
    public float maxValue;
}
