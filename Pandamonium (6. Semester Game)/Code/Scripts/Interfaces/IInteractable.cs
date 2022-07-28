using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool Interactable { get; }
    bool Interacting { get; }
    string InteractText { get; }
    Transform Transform { get; }
    void Interact();
    void EndInteract();
}