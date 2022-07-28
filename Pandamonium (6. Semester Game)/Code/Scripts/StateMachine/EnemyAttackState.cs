using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState (EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private float timemeasure;
    public override void CheckSwitchStates() {
        //if (timemeasure >= _ctx.enemyanimation.GetCurrentAnimatorClipInfo(0)[0].clip.length)
        //{
        //    SwitchState(_factory.Walk());
        //    timemeasure = 0;
        //}
        //else timemeasure += Time.deltaTime;
        if (_ctx.enemyanimation.GetCurrentAnimatorStateInfo(0).IsName("Idle")) //enemies briefly transition into idle after an attack, check that to see if done attacking
        {
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {

        _ctx.enemyanimation.SetTrigger("inReach");

        timemeasure = 0;
        //_ctx.AttackCollider.SetActive(true);
    }

    public override void ExitState() {
        _ctx.DeactivateAttack();
    }

    public override void InitializeSubStates() { }

    public override void UpdateState() {





        CheckSwitchStates();

        //        _ctx.currentState.ExitState();
        //SwitchState(_factory.Walk());
    }

    private IEnumerator WaitAnimationOverAndDoThings()  //these never work, Whyy??
    {
        yield return new WaitForSeconds(_ctx.enemyanimation.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        SwitchState(_factory.Walk());

    }

}
