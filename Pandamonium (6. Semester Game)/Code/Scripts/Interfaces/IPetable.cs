using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPetable
{
    bool Petable { get; }
    bool Petting { get; set; }
    void Pet();
    Transform TargetTransform { get; }
}
