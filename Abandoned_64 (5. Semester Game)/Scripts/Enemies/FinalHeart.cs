using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalHeart : Enemy
{
    [SerializeField] private GameEvent specificHeartDeathEvent;
    [SerializeField] private int heartID;
    [SerializeField] private GameEvent someHeartDeathEvent;
    private HeartAttack heartAttack;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        someHeartDeathEvent.Raise();
        specificHeartDeathEvent.Raise();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
