using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory) { }

    public override void EnterState()
    {
        context.Animator.SetBool("IsStanding", true);

        if (!context.IsRunPressed)
        {
            context.ToggleSprint(false);
        }
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsStanding", false);
    }

    public override void InitializeSubState()
    {
       
    }

    public override bool CheckSwitchState()
    {
        if (context.Healing)
        {
            SwitchState(factory.Healing());
            return true;
        }

        if (context.IsAttackPressed && !context.Airborne && context.RootState.GetType() != typeof(PlayerInfiniteSworGlitchState)) //check if root is isg to prevent double trigger of atack when switching back from isg due to it'S uniqueness
        {
            SwitchState(factory.Attack());
            return true;
        }

        if (context.WasContextPressedThisFrame && !context.InContext && !context.Airborne)
        {
            SwitchState(factory.Dodge());
            return true;
        }

        if (context.IsMovementPressed && context.IsRunning())
        {
            SwitchState(factory.Run());
            return true;
        }
        else if (context.IsMovementPressed)
        {
            SwitchState(factory.Walk());
            return true;
        }
        return false;
    }
}
