using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory)
    {
        isRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.PlayerDied.Raise();
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;

        //just some visual representation when dying
        context.Animator.SetBool("IsDead", true);
        context.transform.localScale = new Vector3(context.transform.localScale.x, context.transform.localScale.y, context.transform.localScale.z) * 0.99f;
        //context.transform.localScale -= Vector3.one * 0.01f;
    }

    public override void ExitState()
    {
        context.transform.localScale = Vector3.one;
    }

    public override void InitializeSubState()
    {
        SetSubState(null);
    }

    public override bool CheckSwitchState()
    {
        if(context.transform.localScale.x < 0.1)
        {
            context.Respawn();
            SwitchState(factory.Alive());
            return true;
        }
        return false;
    }
}
