using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory)
    {
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.Animator.SetBool("IsFalling", true);
        context.FallSpeed = 0;
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;

        float previousYVelocity = context.Rb.velocity.y;
        float newYVelocity = context.Rb.velocity.y + (Physics.gravity.y * (context.FallMultiplier - 1) * Time.deltaTime);
        float verletYVelocity = (previousYVelocity + newYVelocity) * 0.5f;

        if (Mathf.Abs(verletYVelocity) > context.MaxFallVelocity)
        {
            context.Rb.velocity = new Vector3(context.Rb.velocity.x, context.MaxFallVelocity * Mathf.Sign(context.Rb.velocity.y), context.Rb.velocity.z);
        }
        else
        {
            context.Rb.velocity = new Vector3(context.Rb.velocity.x, verletYVelocity, context.Rb.velocity.z);
        }
    }

    public override void ExitState()
    {
        context.PlayerLanded.Raise();
        context.Animator.SetBool("IsFalling", false);
    }

    public override void InitializeSubState()
    {
        if (context.GotHit)
        {
            SetSubState(factory.Hit());
        }

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
        if (context.WasLockOnThisFrame)
        {
            SwitchState(factory.LockOn());
            return true;
        }
        else if (context.IsGrounded()/* && Mathf.Abs(context.Rb.velocity.y) < 0.01*/)
        {
            SwitchState(factory.Grounded());
            return true;
        }
        return false;
    }
}