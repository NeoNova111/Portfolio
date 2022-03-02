using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableSerpentStatue : MonoBehaviour, ITargetable
{
    [SerializeField] private bool targetable = true;
    [SerializeField] private Transform targetTransform;

    public bool Targetable { get => targetable; }
    public Transform TargetTransform { get => targetTransform ? targetTransform : transform; }

    private void Start()
    {
        VisualizeTargetable();
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
        if(renderers.Length > 0)
        {
            foreach (var r in renderers)
            {
                if(r.bounds.extents.magnitude > radius)
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
