using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PandaBossWalkState : EnemyBaseState
{

    public PandaBossWalkState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory, PandaBossStateMachine pandaContext)
     : base(currentContext, playerStateFactory)
    {
        bossContext = pandaContext;
    }

    private bool attack = false;
    private PandaBossStateMachine bossContext;
    private float randomAttackInterval = 2.2f;
    private float currentAttackInterval;

    public override void EnterState()
    {
        bossContext.ActivateBoss.Raise(); //only needed once but I'm lazy and under time pressure rn
        _ctx.DustCloud.Play();
        _ctx.navmeshagent.isStopped = false;
        currentAttackInterval = 0f;
        attack = false;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        currentAttackInterval += Time.deltaTime;
        if(currentAttackInterval >= randomAttackInterval)
        {
            attack = CheckIfRandomAttack();
        }
        _ctx.navmeshagent.destination = _ctx.PlayerTransform.position;
        _ctx.distanceToPlayer = Vector3.Distance(_ctx.Transform.position, _ctx.PlayerTransform.position);
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.distanceToPlayer <= _ctx.Attackrange || attack && bossContext.transitioned) //1 works fine
        {
            SwitchState(_factory.PandaBossAttacks(false));
        }
        if (_ctx.distanceToPlayer >= _ctx.seerange / 2)
        {
            SwitchState(_factory.Idle());
        }
    }

    public override void ExitState()
    {
        //_ctx.navmeshagent.destination = _ctx.enemy.transform.position;
        _ctx.navmeshagent.isStopped = true;
        _ctx.DustCloud.Stop();
    }

    public override void InitializeSubStates() { }

    private bool CheckIfRandomAttack()
    {
        currentAttackInterval = 0;
        bool rndAttack = Random.Range(0, 2 + bossContext.skippedShockwaves) != 0;
        if (!rndAttack) bossContext.skippedShockwaves++;
        else bossContext.skippedShockwaves = 0;
        return rndAttack;
    }
}
