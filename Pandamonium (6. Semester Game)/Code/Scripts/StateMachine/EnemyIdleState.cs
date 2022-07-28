using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    RaycastHit hit;
    Transform lookatPlayer;
    public EnemyIdleState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory)
     : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates() {
        if (_ctx.Passive) return; //if the enemy is passive it will indefinetly stay in its idle, so we don't check

        if (_ctx.distanceToPlayer <= _ctx.seerange)
        {
            Vector3 rayDirection = _ctx.PlayerTransform.position - _ctx.TinyGuySpawn.position;
            if (Physics.Raycast(_ctx.TinyGuySpawn.position, rayDirection, out hit, _ctx.seerange, _ctx.ViewObstuctionLayers))
            {
                Debug.DrawRay(_ctx.TinyGuySpawn.position, rayDirection * _ctx.seerange, Color.yellow);

                if (hit.transform.gameObject.GetComponent<PlayerController>())
                {
                    _ctx.enemyanimation.SetBool("SeesPlayer", true);
         
                    if (_ctx.GetComponent<PandaBossStateMachine>()) //hacky solution to check if the enemy is a boss, so it can behave differently (since statemachine ended up not being implemented hierrarchically after all)
                    {
                        SwitchState(_factory.PandaBossWalk(_ctx.GetComponent<PandaBossStateMachine>()));
                    }
                    else
                    {
                        SwitchState(_factory.Walk());
                    }
                }
                else
                {
                    _ctx.enemyanimation.SetBool("SeesPlayer", false);
                }
            }
        }
    }

    public override void EnterState() {

    }

    public override void ExitState() { }

    public override void InitializeSubStates() { }

    public override void UpdateState() {
        if(_ctx.PlayerTransform)
            _ctx.distanceToPlayer = Vector3.Distance(_ctx.enemy.transform.position, _ctx.PlayerTransform.position);

        CheckSwitchStates();
    }
}
