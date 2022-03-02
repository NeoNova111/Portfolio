using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPullable
{
    bool Pullable { get; }
    void Pull();
}
