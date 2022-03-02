using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Collider hitbox;
    [SerializeField] private float damageValue = 10;
    [SerializeField] private bool constantDmageTicks = false;
    [Tooltip("if always active, how often does damage get dealt in second intervals")][SerializeField] private float damageTickInterval;
    private float currentIntervalTime;
    public GameObject WeaponTrail;
    public Collider ColliderBox { get => hitbox; }
    public bool AlwaysActive { get => constantDmageTicks; set => constantDmageTicks = value; }

    // Start is called before the first frame update
    void Start()
    {
        if(!hitbox && GetComponent<Collider>())
            hitbox = GetComponent<Collider>();

        currentIntervalTime = 0;
    }

    public void ActivateHitbox()
    {
        hitbox.enabled = true;
        if (WeaponTrail) WeaponTrail.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        hitbox.enabled = false;
        if (WeaponTrail) WeaponTrail.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if(damagable != null && damagable.Damagable)
        {
            damagable.TakeDamage(damageValue);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!constantDmageTicks)
            return;

        currentIntervalTime = Mathf.Clamp(currentIntervalTime - Time.deltaTime, 0, damageTickInterval);
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null && damagable.Damagable && currentIntervalTime == 0)
        {
            damagable.TakeDamage(damageValue);
            currentIntervalTime = damageTickInterval;
        }
    }
}
