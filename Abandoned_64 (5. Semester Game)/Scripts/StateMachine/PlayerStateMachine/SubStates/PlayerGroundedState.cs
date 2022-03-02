using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory)
    {
    }

    float currentCoyoteTime;

    public override void EnterState()
    {
        InitializeSubState();
        context.FallSpeed = context.Rb.velocity.y;
        currentCoyoteTime = context.CoyoteTime;
        context.Airborne = false;
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;
    }

    public override void ExitState()
    {
        context.Airborne = true;
    }

    public override void InitializeSubState()
    {
        if (context.Interacing)
        {
            SetSubState(null);
            return;
        }

        if(context.ForceWalk)
        {
            SetSubState(factory.Walk());
        }
        else if (context.Healing)
        {
            SetSubState(factory.Healing());
        }
        else if (context.DoAccidentialAttack)
        {
            SetSubState(factory.Attack());
        }
        else if (context.AccelerationProgress == 0)
        {
            SetSubState(factory.Idle());
        }
        else if(context.IsMovementPressed && !context.IsRunPressed)
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

        if (context.Rb.velocity.y < 0 && !context.IsGrounded())
        {
            currentCoyoteTime = Mathf.Clamp(currentCoyoteTime - Time.deltaTime, 0, context.CoyoteTime);
            if(currentCoyoteTime == 0)
            {
                SwitchState(factory.Fall());
                return true;
            }
        }

        if (context.WithinJumpBufferTime && !context.Interacing && !context.GotHit && !context.Healing)
        {
            SwitchState(factory.Jump());
            return true;
        }
        return false;
    }
}
