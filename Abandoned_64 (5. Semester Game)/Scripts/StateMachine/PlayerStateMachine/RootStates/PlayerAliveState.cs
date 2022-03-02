using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAliveState : PlayerBaseState
{
    public PlayerAliveState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory)
    {
        isRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.Animator.SetBool("IsDead", false);
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;
    }

    public override void ExitState()
    {

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
        if (context.PlayerStats.CurrentHealth == 0)
        {
            SwitchState(factory.Dead());
            return true;
        }

        if (context.InContext && !context.Interacing && context.WasContextPressedThisFrame)
        {
            IInteractable closest = context.GetClosestInteractable(context.InteractRange);
            if (closest != null)
            {
                if (closest.ContextPrompt.type != "Talk" || context.CanTalk)
                {
                    SwitchState(factory.Interact(closest));
                    return true;
                }
            }
        }

        return false;
    }
}
