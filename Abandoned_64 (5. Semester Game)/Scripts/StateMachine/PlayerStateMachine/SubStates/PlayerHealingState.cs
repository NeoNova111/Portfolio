using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealingState : PlayerBaseState
{
    public PlayerHealingState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory) { }

    float animationLength;
    float startHealth;
    float elapsedTime;

    public override void EnterState()
    {
        if (!context.Animator.GetCurrentAnimatorStateInfo(0).IsName("LanternActivate"))
        {
            context.Animator.SetTrigger("ActivateLantern");
        }
        animationLength = context.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2;
        Debug.Log(animationLength);
        startHealth = context.PlayerStats.CurrentHealth;
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;

        context.PlayerStats.CurrentHealth = Mathf.Lerp(startHealth, context.PlayerStats.maxHealth, elapsedTime / animationLength);
        elapsedTime += Time.deltaTime;
    }

    public override void ExitState()
    {
        if (context.PlayerStats.CurrentHealth >= (float)context.PlayerStats.maxHealth)
        {
            context.Healing = false;
        }

        context.SaveManager.SavePlayerData();
    }

    public override void InitializeSubState()
    {

    }

    public override bool CheckSwitchState()
    {
        if (!context.Animator.GetCurrentAnimatorStateInfo(0).IsName("LanternActivate") && context.PlayerStats.CurrentHealth == (float)context.PlayerStats.maxHealth)
        {
            if (context.IsAttackPressed || context.DoAccidentialAttack)
            {
                SwitchState(factory.Attack());
                return true;
            }
            else if (context.AccelerationProgress == 0)
            {
                SwitchState(factory.Idle());
                return true;
            }
            else if (context.IsMovementPressed && !context.IsRunPressed)
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
