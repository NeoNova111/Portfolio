using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LanternHeal : MonoBehaviour, IInteractable
{
    [SerializeField] private bool interactable = true;
    private bool interacting;
    private ContextPrompt prompt;
    private SphereCollider sc;
    [SerializeField] private Transform target;
    [SerializeField] private GameEvent ActivatingLantern;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject deleteObject1;
    [SerializeField] private GameObject deleteObject2;

    public bool Interactable { get => interactable; }
    public bool Interacting { get => interacting; }
    public Transform TargetTransform { get { return target != null ? target : transform; } }
    public ContextPrompt ContextPrompt { get => prompt; }
    private PlayerStateMachine player;


    // Start is called before the first frame update
    void Start()
    {
        sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        prompt = new ContextPrompt("Heal", 1);
        player = PlayerStateMachine.Instance;
    }

    public void EndInteract()
    {

    }

    public void Interact()
    {
        player.Healing = true;
        ActivatingLantern.Raise();
        interactable = false;
        deleteObject1.SetActive(false);
        deleteObject2.SetActive(false);
    }

    public void VisualizeTargetable()
    {
        
    }
}
