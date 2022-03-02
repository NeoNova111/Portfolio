using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private float speed = 10;
    [SerializeField] private float lifetime = 10;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(((1 << other.gameObject.layer) & collisionLayerMask) != 0 && !other.GetComponent<ScenePartLoader>()) //to ignore load triggers that span the whole level (they werent made prefabs so this solution is fine since performance has yet to become an issue)
        {
            Destroy(gameObject);
        }
    }
}
