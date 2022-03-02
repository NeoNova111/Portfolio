using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory) { }

    private float turnSmoothVelocity;

    public override void EnterState()
    {
        context.Animator.SetBool("IsRunning", true);
        context.Animator.SetFloat("Velocity Z", 1);
        context.Animator.SetFloat("Velocity X", 0);
        context.Animator.SetFloat("Sprinting", 1);
    }

    public override void UpdateState()
    {
        if (context.IsMovementPressed)
        {
            context.AccelerationProgress = Mathf.Clamp(context.AccelerationProgress + Time.deltaTime / context.MovementAccelTime, 0, 1);
        }
        else
        {
            context.AccelerationProgress = Mathf.Clamp(context.AccelerationProgress - Time.deltaTime / context.MovementDeccelTime, 0, 1);
        }

        if (CheckSwitchState())
            return;

        if (context.IsMovementPressed)
        {
            Vector2 movementVector = new Vector2(context.CurrentMovement.x, context.CurrentMovement.y);
            float targetAngle = Mathf.Atan2(movementVector.x, movementVector.y) * Mathf.Rad2Deg + context.CameraController.MainCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(context.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, context.TurnSmoothTime);
            context.transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            context.transform.position += moveDir.normalized * context.Movespeed * Time.deltaTime * context.AccelerationProgress * context.RunMultiplier;
        }
        else
        {
            Vector3 moveDir = context.transform.forward;
            context.transform.position += moveDir.normalized * context.Movespeed * Time.deltaTime * context.AccelerationProgress * context.RunMultiplier;
        }
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsRunning", false);
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

        if (context.AccelerationProgress == 0)
        {
            SwitchState(factory.Idle());
            return true;
        }
        else if (!context.IsRunning())
        {
            SwitchState(factory.Walk());
            return true;
        }
        return false;
    }
}
