using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfiniteSworGlitchState : PlayerBaseState
{
    public PlayerInfiniteSworGlitchState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory)
    {
        isRootState = true;
    }

    bool interactEnded = false;
  //  [SerializeField] private GameObject ISGEffect;

    public override void EnterState()
    {
        context.ISGEffect.SetActive(true);
   
        context.InteractingWith.Interact();
        InitializeSubState();
        context.InfiniteSwordGlitching = true; //needs to be after initialize substate
        context.SwordHitbox.ActivateHitbox();

        context.SwordHitbox.AlwaysActive = true;
        context.UIManager.HUD.SetActive(false);
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;

        if (!context.SwordHitbox.ColliderBox.enabled)
        {
            context.SwordHitbox.ActivateHitbox();
        }

        if (context.WasContextPressedThisFrame && context.InContext && context.Interacing)
        {
            context.InteractingWith.Interact();
        }

        if (!context.Interacing && !interactEnded)
        {
            interactEnded = true;
            context.UIManager.HUD.SetActive(true);

            if (context.InteractingWith != null)
            {
                if (context.InteractingWith.Interacting)
                {
                    context.InteractingWith.EndInteract();
                }

                if (context.InteractingWith.ContextPrompt.type == "Talk") //todo: type
                {
                    context.StartTalkingDelay();
                }

                InitializeSubState();
            }
        }
    }

    public override void ExitState()
    {
        context.ISGEffect.SetActive(false);
        context.SwordHitbox.DeactivateHitbox();
        context.SwordHitbox.AlwaysActive = false;
        context.InfiniteSwordGlitching = false;
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

        if (context.WasAttackPressedThisFrame && !context.Interacing)
        {
            SwitchState(factory.Alive());
            return true;
        }

        if (context.InContext && !context.Interacing && context.WasContextPressedThisFrame && interactEnded)
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
