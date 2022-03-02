using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnTransform = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.position = respawnTransform.position;
        }
    }
}
