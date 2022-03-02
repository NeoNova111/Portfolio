using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory)
    {
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.Rb.velocity = new Vector3(context.Rb.velocity.x, context.JumpVelocity, context.Rb.velocity.z);
        context.Animator.SetBool("IsJumping", true);
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsJumping", false);
    }

    public override void InitializeSubState()
    {
        if (context.Interacing)
        {
            SetSubState(null);
            return;
        }

        if (context.ForceWalk)
        {
            SetSubState(factory.Walk());
        }
        else if (context.Healing)
        {
            SetSubState(factory.Healing());
        }
        else if (context.AccelerationProgress == 0)
        {
            SetSubState(factory.Idle());
        }
        else if (context.IsMovementPressed && !context.IsRunPressed)
        {
            SetSubState(factory.Walk());
        }
        else
        {
            SetSubState(factory.Run());
        }
    }

    public override bool CheckSwitchState()
    {
        if (context.GotHit)
        {
            SetSubState(factory.Hit());
        }

        if (context.WasLockOnThisFrame)
        {
            SwitchState(factory.LockOn());
            return true;
        }
        else if (context.IsGrounded() /*&& Mathf.Abs(context.Rb.velocity.y) < 0.01*/)
        {
            SwitchState(factory.Grounded());
            return true;
        }
        else if(context.Rb.velocity.y < 0 || !context.IsJumpPressed)
        {
            SwitchState(factory.Fall());
            return true;
        }
        return false;
    }
}
