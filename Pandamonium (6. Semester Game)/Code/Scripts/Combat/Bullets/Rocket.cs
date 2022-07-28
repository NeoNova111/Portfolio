using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : BaseBullet
{
    [SerializeField] private float speedupTime = 2f;
    [SerializeField] private AnimationCurve speedUpCurve;
    private float time;
    private Vector3 direction;

    public override void InitializeBullet(Vector3 bulletVelocity, float range, float damage, GameObject deathEffect = null)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        direction = bulletVelocity;
        this.range = range;
        this.damage = damage;
        this.deathEffect = deathEffect;
        time = 0;
    }

    protected override void Update()
    {
        base.Update();
        if(time < speedupTime)
        {
            rb.velocity = direction * (speedUpCurve.Evaluate(time) / speedupTime);
            time = Mathf.Clamp(time + Time.deltaTime, 0, speedupTime);
        }
    }
}
