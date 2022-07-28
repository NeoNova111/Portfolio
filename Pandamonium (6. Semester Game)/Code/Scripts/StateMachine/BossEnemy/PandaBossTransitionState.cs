using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaBossTransitionState : EnemyBaseState
{
    public PandaBossTransitionState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory, PandaBossStateMachine pandaContext)
     : base(currentContext, playerStateFactory)
    {
        this.pandaContext = pandaContext;
    }

    private PandaBossStateMachine pandaContext;
    private Vector3 jumpTarget;
    private Vector3 jumpStart;
    private float currentJumpTime = 0;

    public override void EnterState()
    {
        pandaContext.Invincible(true);
        pandaContext.navmeshagent.enabled = false;
        pandaContext.navmeshagent.speed *= pandaContext.speedMultiplier;
        AkSoundEngine.PostEvent("Bear_Buff", pandaContext.gameObject);
        pandaContext.enemyanimation.SetTrigger("Transition");
        pandaContext.enemyanimation.SetFloat("AnimationSpeed", pandaContext.speedMultiplier);
        pandaContext.transitioning = true;
        jumpStart = pandaContext.transform.position;
        Vector3 directioToPlayer = PlayerController.Instance ? PlayerController.Instance.transform.position - pandaContext.Transform.position : Vector3.left;
        directioToPlayer.y = 0;
        jumpTarget = pandaContext.transform.position + (Vector3.up + directioToPlayer.normalized) * pandaContext.meteorStartDistance;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (pandaContext.jumping)
        {
            currentJumpTime = Mathf.Clamp(currentJumpTime + Time.deltaTime, 0, pandaContext.jumpDuration);
            float pointInJump = pandaContext.jump.Evaluate(currentJumpTime / pandaContext.jumpDuration);
            pandaContext.transform.position = Vector3.Lerp(jumpStart, jumpTarget, pointInJump);
        }
    }

    public override void CheckSwitchStates()
    {
        if(pandaContext.transform.position == jumpTarget)
        {
            SwitchState(_factory.PandaBossMeteor(pandaContext));
        }
    }

    public override void ExitState()
    {
        pandaContext.jumping = false;
    }

    public override void InitializeSubStates() { }

}