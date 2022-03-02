using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspectable : MonoBehaviour, IInspectable
{
    private Material[] originalMaterials;
    private Material[] inspectableMaterials;
    private MeshRenderer meshRenderer;
    private bool isInspectable;
    private Transform Transformation;
    [SerializeField] private string developerComment = "";

    public bool IsInspectable { get => isInspectable; set => isInspectable = value; }
    public string DeveloperComment { get => developerComment;}
    public Transform inspectableTransform { get => Transformation; }

    private void Awake()
    {
        gameObject.layer = 12; //12 = inspectable
        Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in trans)
        {
            t.gameObject.layer = 12;
        }
        Transformation = this.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        // +++ start preparing switching of materials
        meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            Debug.LogWarning("Inspectable component needs to be on on the same Object as a meshrenderer to take effect");
            return;
        }
        originalMaterials = meshRenderer.materials;
        inspectableMaterials = new Material[originalMaterials.Length];
        Material inspectableMaterial = Resources.Load<Material>("Material/InspectableMat");
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            inspectableMaterials[i] = inspectableMaterial;
        }
        // --- end preparing switching of materials
        if (DebugModeManager.Instance.DebugModeActive)
        {
            SwitchMaterials(true);
        }
        */
    }

    public void StartInspecting()
    {
        //do smth
    }

    public void StopInspecting()
    {
        //do smth
    }

    /*
    public void SwitchMaterials(bool inspectableMaterial)
    {
        if (inspectableMaterial)
        {
            meshRenderer.materials = inspectableMaterials;
        }
        else
        {
            meshRenderer.materials = originalMaterials;
        }
    }
    */
}
