using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaBossAttackState : EnemyBaseState
{
    public PandaBossAttackState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory, bool desperate)
    : base(currentContext, playerStateFactory)
    {
        this.desperate = desperate;
    }

    bool desperate;

    public override void CheckSwitchStates()
    {
        if (_ctx.enemyanimation.GetCurrentAnimatorStateInfo(0).IsName("Idle")) //enemies briefly transition into idle after an attack, check that to see if done attacking
        {
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        if(_ctx.distanceToPlayer > _ctx.Attackrange || desperate)
        {
            _ctx.enemyanimation.SetTrigger("Stomp");
        }
        else
        {
            _ctx.enemyanimation.SetTrigger("Hit");
        }

    }

    public override void ExitState()
    {
        _ctx.DeactivateAttack();
    }

    public override void InitializeSubStates() { }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}