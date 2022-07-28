using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseBullet : MonoBehaviour
{
    [SerializeField] protected LayerMask collisionLayers;
    [SerializeField] protected TrailRenderer trailObject;
    [SerializeField] protected float trailLingeringTime = 0.1f;
    protected GameObject deathEffect;
    protected float range;
    protected float damage;
    protected Rigidbody rb;
    protected Vector3 startPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        startPos = transform.position;
    }

    protected virtual void Update()
    {
        if (Vector3.Distance(startPos, transform.position) > range) DestroyBullet();
    }

    public virtual void InitializeBullet(Vector3 bulletVelocity, float range, float damage, GameObject deathEffect = null)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = bulletVelocity;
        this.range = range;
        this.damage = damage;
        this.deathEffect = deathEffect;
    }

    public void DestroyBullet()
    {
        //if bullet has cool effect when "dying" instantiate it
        if(deathEffect) Instantiate(deathEffect, transform.position, transform.rotation);

        if (trailObject)
        {
            trailObject.transform.parent = null;
            Destroy(trailObject.gameObject, trailLingeringTime);
        }

        Destroy(gameObject);
    }

    protected void OnCollisionEnter(Collision other)
    {
        if((collisionLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            PlayerController playerInstance = PlayerController.Instance;
            IDamagable hitDamagable = other.gameObject.GetComponent<IDamagable>();
            if (hitDamagable != null && hitDamagable.Damagable) hitDamagable.TakeDamage(damage);

            DestroyBullet();
        }
    }
}
