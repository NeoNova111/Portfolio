using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkState : EnemyBaseState    
{

    public EnemyWalkState (EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory)
     : base(currentContext, playerStateFactory) { }




    
    public override void CheckSwitchStates() {
        if (_ctx.distanceToPlayer <= _ctx.Attackrange) //1 works fine
        {
            SwitchState(_factory.Attack());
        }
        if (_ctx.distanceToPlayer >= _ctx.seerange/2) 
        {
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState() {
        _ctx.DustCloud.Play();
        _ctx.navmeshagent.isStopped = false;
    }   

    public override void ExitState() {
        //_ctx.navmeshagent.destination = _ctx.enemy.transform.position;
        _ctx.navmeshagent.isStopped = true;
        _ctx.DustCloud.Stop();
    }

    public override void InitializeSubStates() { }

    public override void UpdateState() {
        CheckSwitchStates();
        _ctx.navmeshagent.destination = _ctx.PlayerTransform.position;

        _ctx.distanceToPlayer = Vector3.Distance(_ctx.enemy.transform.position, _ctx.PlayerTransform.position);

    } 
}
