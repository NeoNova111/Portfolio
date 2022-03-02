using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartEnemy : Enemy
{
    [SerializeField] private GameEvent specificHeartDeathEvent;
    [SerializeField] private int heartID;
    [SerializeField] private GameEvent someHeartDeathEvent;
    private HeartAttack heartAttack;
    private BossHealthBarManager bossHealthBarManager;

    protected override void Start()
    {
        base.Start();
        heartAttack = GetComponentInChildren<HeartAttack>();
        bossHealthBarManager = BossHealthBarManager.instance;
        bossHealthBarManager.setReferenceToHeart(this, heartID);
    }

    protected override void Die()
    {
        base.Die();

        specificHeartDeathEvent.Raise();
        someHeartDeathEvent.Raise();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        heartAttack.stun();
        bossHealthBarManager.updateHealthBar(heartID, currentHealth / startHealth);
    }
}
