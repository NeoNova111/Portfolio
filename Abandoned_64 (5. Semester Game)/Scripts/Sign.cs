using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sign : MonoBehaviour, IInteractable
{
    [SerializeField] private bool interactable = true;
    [SerializeField] private bool interacting = false;
    private ContextPrompt contextPrompt;

    public bool Interactable { get => interactable; }
    public bool Interacting { get => interacting; }
    public Transform TargetTransform { get => transform; }
    public ContextPrompt ContextPrompt { get => contextPrompt; }

    [SerializeField] [TextArea(2, 5)] private string signText;
    [SerializeField] [TextArea(2, 5)] private string debugSignText;
    [SerializeField] private GameObject virtualCamera;
    private UIManager userInterfaceManager;

    public UnityEvent OnInteract;
    public UnityEvent OnEndInteract;

    protected void Start()
    {
        contextPrompt = new ContextPrompt("Read", 1);
        userInterfaceManager = UIManager.Instance;
        VisualizeTargetable();
    }

    public void Interact()
    {
        if (!interactable)
            return;

        if (interacting)
        {
            StopReadingSign();
        }
        else
        {
            ReadSign();
            OnInteract.Invoke();
        }
    }

    protected void ReadSign()
    {
        userInterfaceManager.DispalySign(signText, debugSignText, true);

        if (virtualCamera)
        {
            virtualCamera.SetActive(true);
        }

        interacting = true;
    }

    protected void StopReadingSign()
    {
        OnEndInteract.Invoke();
        userInterfaceManager.DispalySign(false);

        if (virtualCamera)
        {
            virtualCamera.SetActive(false);
        }

        interacting = false;
        StartCoroutine(BlockInteracting());
    }
   
    public void EndInteract()
    {

        StopReadingSign();
      
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
        GameObject instance = Instantiate(Resources.Load("Prefab/InteractableVisualizer", typeof(GameObject))) as GameObject;
        instance.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        instance.transform.parent = gameObject.transform;
        instance.transform.localPosition = position + new Vector3(0,4,0);
        Debug.Log("VisualizeTargetable() was called in Sign.cs");
    }
    IEnumerator BlockInteracting()
    {
        interactable = false;
        yield return new WaitForSeconds(1.2f);
        interactable = true;

    }
}
