using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory) { }

    float TimePassed = 0; 

    public override void EnterState()
    {
        context.DoAccidentialAttack = false;
        context.Animator.SetTrigger("Attack");
    }

    public override void UpdateState()
    {
        TimePassed += Time.deltaTime;

        if (CheckSwitchState())
            return;

        //There should probably be some forward movement when attacking
        //So Oliver is going to include it for now until one of our programmers sees this and says "Oh no,let's not do it like that!"
        if (TimePassed<0.15f)
        context.transform.position += context.transform.forward*context.Movespeed*2*Time.deltaTime;

    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {
      
    }

    public override bool CheckSwitchState()
    {
        if(context.InContext && !context.Interacing && context.WasContextPressedThisFrame)
        {
            if(context.GetClosestInteractable(context.InteractRange) != null && context.GetClosestInteractable(context.InteractRange).ContextPrompt.type != "Talk" || context.CanTalk)
            {
                NewRoot(factory.ISG());
                return true;
            }
        }

        if (context.Animator.GetCurrentAnimatorStateInfo(0).IsName("AttackTransition1") || context.Animator.GetCurrentAnimatorStateInfo(0).IsName("AttackTransition2"))
        {
            if (context.IsAttackPressed)
            {
                SwitchState(factory.Attack());
                return true;
            }
            else if (context.Healing)
            {
                SwitchState(factory.Healing());
                return true;
            }
            else if (context.AccelerationProgress == 0)
            {
                SwitchState(factory.Idle());
                return true;
            }
            else if (context.IsMovementPressed && !context.IsRunning())
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