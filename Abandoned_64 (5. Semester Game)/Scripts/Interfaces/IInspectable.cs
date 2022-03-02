using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInspectable
{
    bool IsInspectable { get; set; }
    string DeveloperComment { get; }

    void StartInspecting();
    void StopInspecting();
    //void SwitchMaterials(bool inspectableMaterial);
}
