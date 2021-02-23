using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FloatReference
{
    public bool useConstant = true;
    public float constantValue;
    public FloatVariable Variable;

    public float Value
    {
        get { return useConstant ? constantValue : Variable.Value;  }
    }
}
