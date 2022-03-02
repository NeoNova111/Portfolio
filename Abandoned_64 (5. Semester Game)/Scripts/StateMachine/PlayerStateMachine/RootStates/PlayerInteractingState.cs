using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractingState : PlayerBaseState
{
    public PlayerInteractingState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory, IInteractable interactable) : base(currentContext, playerstateFactory)
    {
        context.InteractingWith = interactable;
        isRootState = true;
    }

    public override void EnterState()
    {
        context.CameraController.ThirdPersonCam.m_Transitions.m_InheritPosition = false;

        if (context.InteractingWith == null)
            return;

        context.InteractingWith.Interact();
        InitializeSubState();
        context.Animator.SetFloat("Velocity Z", 0);
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;

        if (context.UIManager.HUD.activeSelf)
        {
            context.UIManager.HUD.SetActive(false);
        }

        if (context.WasContextPressedThisFrame)
        {
            context.InteractingWith.Interact();
        }
    }

    public override void ExitState()
    {
        context.UIManager.HUD.SetActive(true);
        context.DoAccidentialAttack = true;

        if (context.InteractingWith != null)
        {
            if (context.InteractingWith.Interacting)
            {
                context.InteractingWith.EndInteract();
            }

            if(context.InteractingWith.ContextPrompt.type == "Talk") //todo: type
            {
                context.StartTalkingDelay();
            }
        }
    }

    public override void InitializeSubState()
    {
        if (context.CameraController && context.CameraController.LockedOn)
        {
            SetSubState(factory.LockOn());
        }
        else
        {
            SetSubState(factory.Grounded());
        }
    }

    public override bool CheckSwitchState()
    {
        if (context.InteractingWith == null || !context.InteractingWith.Interacting)
        {
            SwitchState(factory.Alive());
            return true;
        }
        return false;
    }
}
