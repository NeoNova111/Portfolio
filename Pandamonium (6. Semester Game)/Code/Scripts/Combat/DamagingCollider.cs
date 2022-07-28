using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingCollider : MonoBehaviour
{
    public bool GroundedDamage { get => groundedDamage;  set => groundedDamage = value; }
    public float Damage { get => damage; set => damage = value; }

    [SerializeField] private bool groundedDamage = false;
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask thisDamages = ~0; //~0 sets bitmask to everything
    [SerializeField] private List<IDamagable> damaged;

    private void OnTriggerEnter(Collider other)
    {
        if (thisDamages.Contains(other.gameObject.layer))
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null && !damaged.Contains(damagable))
            {
                damaged.Add(damagable); //ensure damageable gets only damaged once for the lifetime of a collider
                PlayerController player = other.GetComponent<PlayerController>();
                if (player)
                {
                    if (groundedDamage && !player.IsGrounded()) return;
                }
                damagable.TakeDamage(damage);
            }
        }
    }

    private void OnEnable()
    {
        damaged = new List<IDamagable>();
    }
}
