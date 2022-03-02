using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLockOnState : PlayerBaseState
{
    public PlayerLockOnState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory)
    {
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.CameraController.ToggleLockOn(true);
        context.UIManager.ToggleTargetingUI(true);
        if(!context.CameraController.LockedOn)
        {
            ExitState();
            return;
        }

        context.Animator.SetFloat("LockOn", 1);
        context.CameraController.ThirdPersonCam.m_Transitions.m_InheritPosition = true;
        FreezeConstraints(true);
        context.Airborne = false;
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;

        //todo holding var
        if (context.WasLeftPressedThisframe)
        {
            context.CameraController.SwitchLockOnLeftNext();
        }
        else if (context.WasRightPressdThisFrame)
        {
            context.CameraController.SwitchLockOnRightNext();
        }

        Vector3 targetDir = context.CameraController.LockOnTarget.TargetTransform.position - context.transform.position;
        Vector3 planarToTarget = Vector3.ProjectOnPlane(targetDir, Vector3.up);

        context.transform.rotation = Quaternion.LookRotation(planarToTarget, Vector3.up);

        context.Animator.SetFloat("Velocity Z", context.CurrentMovement.y);
        context.Animator.SetFloat("Velocity X", context.CurrentMovement.x);
    }

    public override void ExitState()
    {
        if(context.WasLockOnThisFrame || !context.CameraController.LockedOn)
        {
            context.Animator.SetFloat("LockOn", 0);
            FreezeConstraints(false);
            context.UIManager.ToggleTargetingUI(false);
            context.CameraController.ToggleLockOn(false);
            context.Airborne = true;
        }
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
        else if (context.DoAccidentialAttack)
        {
            SetSubState(factory.Attack());
        }
        else if (!context.IsMovementPressed && !context.IsRunPressed)
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

        if (context.WasLockOnThisFrame || !context.CameraController.LockedOn)
        {
            if (context.IsGrounded())
            {
                SwitchState(factory.Grounded());
                return true;
            }
            else
            {
                SwitchState(factory.Fall());
                return true;
            }
        }
        return false;
    }

    //state specific methods
    public void FreezeConstraints(bool freeze)
    {
        context.Rb.constraints = RigidbodyConstraints.None;

        if (freeze)
        {
            context.Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            context.Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}
