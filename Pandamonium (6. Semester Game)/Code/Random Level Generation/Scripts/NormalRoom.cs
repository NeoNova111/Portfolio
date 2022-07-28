using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRoom : MonoBehaviour
{
    private BoxCollider coll;
    public BoxCollider Collider { get => coll; }
    private Vector3 seperationDirection;

    private List<Collider> colliders = new List<Collider>();
    public List<Collider> GetColliders() { return colliders; }

    void Start()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.Contains(other)) colliders.Add(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (colliders.Count <= 0) return;

        Vector3 vel = Vector3.zero;
        foreach (var otherCol in GetColliders())
        {
            vel += otherCol.transform.position - transform.position;
        }
        vel *= -1;
        vel.y = 0;
        transform.position += vel.normalized * Time.deltaTime * 10;
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }

    private void OnDisable()
    {
        colliders.Clear();
    }
}
