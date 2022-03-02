using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool Interactable { get; }
    bool Interacting { get; }
    Transform TargetTransform { get; }
    ContextPrompt ContextPrompt { get; }
    void Interact();
    void VisualizeTargetable();
    void EndInteract();
}
