using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SignNPC : MonoBehaviour, IInteractable, ITargetable
{
    [SerializeField] private DialogueConversation dialogue;
    [SerializeField] private CinemachineVirtualCamera[] dialogueCameras;
    public CinemachineVirtualCamera[] DialogueCameras { get => dialogueCameras; }
    [SerializeField] private bool interactable = true;
    [SerializeField] private bool interacting = false;
    private ContextPrompt contextPrompt;

    //targetable interface
    [SerializeField] private bool targetable = true;
    [SerializeField] private Transform targetTransform;

    public bool Targetable { get => targetable; }
    public Transform TargetTransform { get { return targetTransform ? targetTransform : transform; } }

    public bool Interactable { get => interactable; }
    public bool Interacting { get => interacting; }
    public ContextPrompt ContextPrompt { get => contextPrompt; }

    private UIManager userInterfaceManager;
    private DialogueSystem dialogueSystem;

    protected void Start()
    {
        foreach(CinemachineVirtualCamera camera in dialogueCameras)
        {
            camera.gameObject.SetActive(false);
        }

        contextPrompt = new ContextPrompt("Talk", 1);
        userInterfaceManager = UIManager.Instance;
        dialogueSystem = DialogueSystem.Instance;
    }

    public void Interact()
    {
        if (!interactable || !dialogue)
            return;

        if (interacting)
        {
            dialogueSystem.SayCurentLine(this);
        }
        else
        {
            dialogueSystem.SetConvo(dialogue);
            dialogueSystem.SayCurentLine(this);
            interacting = true;
        }
    }

    //listen to dialogue ended game event then call this
    public void EndInteract()
    {
        interacting = false;
    }

    public void VisualizeTargetable()
    {
        Renderer rend = GetComponent<Renderer>();
        float radius = 0;
        Vector3 position = new Vector3(0, 0, 0);
        if (rend != null)
        {
            //Vector3 center = rend.bounds.center;
            radius = rend.bounds.extents.magnitude;
        }
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            foreach (var r in renderers)
            {
                if (r.bounds.extents.magnitude > radius)
                {
                    radius = r.bounds.extents.magnitude;
                    position = r.transform.localPosition;
                }
            }
        }
        if (radius == 0)
        {
            radius = 2;
        }
        GameObject instance = Instantiate(Resources.Load("Prefab/TargetableVisualizer", typeof(GameObject))) as GameObject;
        instance.transform.localScale = new Vector3(radius, radius, radius);
        instance.transform.parent = gameObject.transform;
        instance.transform.localPosition = position;
        Debug.Log("VisualizeTargetable() was called in Enemy.cs");
    }
}
