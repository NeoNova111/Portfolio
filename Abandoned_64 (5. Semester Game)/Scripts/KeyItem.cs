using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class KeyItem : MonoBehaviour, ICollectable, IInteractable
{

    private bool collectable;
    private SphereCollider sc;
    private bool interacting;
    private ContextPrompt prompt;
    [SerializeField] private bool interactable = true;
    [SerializeField] private Transform target;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameEvent collectedEvent;

    public bool Collectable { get => collectable; }
    public bool Interactable { get => interactable; }
    public bool Interacting { get => interacting; }
    public Transform TargetTransform { get { return target != null ? target : transform; } }
    public ContextPrompt ContextPrompt { get => prompt; }

    public void Start()
    {
        sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        prompt = new ContextPrompt("Pick Up", 1);
    }

    public void Collect()
    {
        Destroy(gameObject);
        playerStats.keyCount++;
        collectedEvent.Raise();
        PlayerStateMachine.Instance.InteractingWith = null;
    }

    public void Interact()
    {
        Collect();
    }

    public void VisualizeTargetable()
    {
        
    }

    public void EndInteract()
    {
        
    }
}
