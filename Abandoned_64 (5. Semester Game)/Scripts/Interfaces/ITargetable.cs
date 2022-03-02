using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    bool Targetable { get; }
    Transform TargetTransform { get; }

    void VisualizeTargetable();
}
