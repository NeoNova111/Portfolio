using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGroundEventFire : MonoBehaviour
{
    [SerializeField] private MovingGroundEnemy enemy;
    [SerializeField]
    private Hitbox hitbox = null;

    [SerializeField] protected GameEvent attack;

    public void OnUp()
    {
        enemy.Underground = false;
        enemy.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void OnDown()
    {
        //enemy.UpdateMeshVisibility(false);
        enemy.Underground = true;
        enemy.GetComponent<CapsuleCollider>().enabled = false;
    }

    public void AnticipEnd()
    {
        enemy.State = MovingGroundEnemy.MovingEnemyState.Attacking;
    }

    public void AttackStart()
    {
        if (attack)
        {
            attack.Raise();
        }
    }

    public void AttackComplete()
    {
        enemy.State = MovingGroundEnemy.MovingEnemyState.Idle;
        Debug.LogWarning("ATTACK COMPLETE!");
    }

    public void ActivateHitbox()
    {
        hitbox.ActivateHitbox();
    }

    public void DeactivateHitbox()
    {
        hitbox.DeactivateHitbox();
    }
}
