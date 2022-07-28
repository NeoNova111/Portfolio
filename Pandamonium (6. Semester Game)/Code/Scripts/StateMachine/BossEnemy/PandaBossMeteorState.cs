using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaBossMeteorState : EnemyBaseState
{
    public PandaBossMeteorState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory, PandaBossStateMachine pandaContext)
    : base(currentContext, playerStateFactory)
    {
        this.pandaContext = pandaContext;
    }

    private PandaBossStateMachine pandaContext;
    private Vector3 meteorTarget;
    private Vector3 meteorStart;
    private float currentMeteorTime = 0;
    private float currentDelayTime = 0;

    private bool impacted = false;

    public override void CheckSwitchStates()
    {
        if (_ctx.enemyanimation.GetCurrentAnimatorStateInfo(0).IsName("Idle")) //enemies briefly transition into idle after an attack, check that to see if done attacking
        {
            SwitchState(_factory.Idle());
        }
    }

    public override void EnterState()
    {
        pandaContext.Fireball.SetActive(true);
        meteorStart = pandaContext.transform.position;
        pandaContext.enemyanimation.SetTrigger("ReenterArena");
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        currentDelayTime = Mathf.Clamp(currentDelayTime + Time.deltaTime, 0, pandaContext.meteorDelay);
        if (currentDelayTime != pandaContext.meteorDelay) return;

        currentMeteorTime = Mathf.Clamp(currentMeteorTime + Time.deltaTime, 0, pandaContext.meteorDuration);
        float pointInJump = currentMeteorTime / pandaContext.meteorDuration;

        if(pointInJump < pandaContext.meteorTracking)
        {
            meteorTarget = PlayerController.Instance.Origin.position;
        }

        Vector3 hori = Vector3.Lerp(meteorStart, meteorTarget, 1 - pandaContext.meteorSpeed.Evaluate(pointInJump));
        Vector3 vert = Vector3.Lerp(meteorStart, meteorTarget, 1 - pandaContext.meteorTrajectory.Evaluate(pointInJump)) + (pandaContext.transform.position - pandaContext.Transform.position); //neccesary cause of weird offset with sprite, and this hack is faster than redoin animations
        pandaContext.transform.position = new Vector3(hori.x, vert.y, hori.z);

        if (pointInJump == 1 && !impacted)
        {
            pandaContext.enemyanimation.SetTrigger("GroundImpact");
            pandaContext.Impact();
            impacted = true;
            pandaContext.Fireball.SetActive(false);
        }
    }

    public override void ExitState()
    {
        pandaContext.navmeshagent.enabled = true;
        pandaContext.transitioned = true;
        pandaContext.transitioning = false;
        pandaContext.Invincible(false);
    }

    public override void InitializeSubStates() { }
}