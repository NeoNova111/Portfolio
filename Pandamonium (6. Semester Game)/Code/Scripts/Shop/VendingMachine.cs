using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
    public bool Interactable { get => interactable; }
    public bool Interacting { get => interacting; }
    public string InteractText { get => interactText; }
    public Transform Transform { get => transformOverride ? transformOverride : transform; }


    [SerializeField] private bool interactable = true;
    [SerializeField] [TextArea(2, 5)] private string interactText;
    [SerializeField] private Transform transformOverride;
    private bool interacting = false;

    public void EndInteract()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
