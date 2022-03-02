using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerBaseState
{
    public PlayerHitState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory) { }

    string animationStateName;

    public override void EnterState()
    {
        animationStateName = "GetHit" + Random.Range(1, 3);
        context.Animator.Play(animationStateName);

        if(context.blink)
            context.blink.HasBeenHit = true;

        context.Invincible = true;
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;
    }

    public override void ExitState()
    {
        context.Invincible = false;
        context.GotHit = false;
    }

    public override void InitializeSubState()
    {

    }

    public override bool CheckSwitchState()
    {
        if (!context.Animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
        {
            if (context.IsAttackPressed && context.RootState.GetType() != typeof(PlayerInfiniteSworGlitchState)) //check if root is isg to prevent double trigger of atack when switching back from isg due to it'S uniqueness
            {
                SwitchState(factory.Attack());
                return true;
            }
            else if (context.AccelerationProgress == 0)
            {
                SwitchState(factory.Idle());
                return true;
            }
            else if (context.IsMovementPressed && !context.IsRunPressed)
            {
                SwitchState(factory.Walk());
                return true;
            }
            else
            {
                SwitchState(factory.Run());
                return true;
            }
        }
        return false;
    }
}
