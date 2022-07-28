using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingExplosion : MonoBehaviour
{
    [SerializeField] private bool scaleWithScale = true;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float damage = 60f;
    [SerializeField] private LayerMask thisDamages = ~0; //~0 sets bitmask to everything
    [SerializeField] private AnimationCurve damageFalloff;

    void Start()
    {
        if (scaleWithScale)
        {
            radius *= transform.localScale.x;
            damage *= transform.localScale.x;
        }

        DealDamageInRadius(damage, radius);
    }

    public void DealDamageInRadius(float damage, float radius)
    {
        List<IDamagable> damnagables = new List<IDamagable>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, thisDamages);

        foreach (Collider collider in colliders)
        {
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null && damagable.Damagable)
            {
                bool duplicate = false; //prevents dealing damage multiple times to ame damagable if it has more than one collider
                foreach(var d in damnagables)
                {
                    if (d == damagable) duplicate = true;
                }

                if(!duplicate) damnagables.Add(damagable);
            }
        }

        foreach (IDamagable damagable in damnagables)
        {
            float distance = Vector3.Distance(transform.position, damagable.Transform.position);
            float damageByDistance = damage * damageFalloff.Evaluate(distance / radius);
            damagable.TakeDamage(damageByDistance);
        }
    }
}
