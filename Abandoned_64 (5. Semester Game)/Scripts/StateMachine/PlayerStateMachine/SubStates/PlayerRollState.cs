using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerBaseState
{
    public PlayerRollState(PlayerStateMachine currentContext, PlayerStateFactory playerstateFactory) : base(currentContext, playerstateFactory) { }

    private float turnSmoothVelocity;
    private Vector2 directionInput;
    private Vector3 moveDir;
    private Vector3 startPos;
    float predictedRollTime;
    float currentRollTime;
    bool rolled;

    public override void EnterState()
    {
        context.Invincible = true;
        context.Animator.SetTrigger("Roll");
        rolled = false;
        startPos = context.transform.position;

        if (context.CurrentMovement == Vector2.zero)
        {
            directionInput = new Vector2(context.CurrentMovement.x, context.CurrentMovement.y);
        }
        else
        {
            directionInput = new Vector2(context.CurrentMovement.x, context.CurrentMovement.y);
        }

        context.Animator.SetFloat("Velocity Z", 1);

        Vector2 movementVector = new Vector2(directionInput.x, directionInput.y).normalized;
        float targetAngle = Mathf.Atan2(movementVector.x, movementVector.y) * Mathf.Rad2Deg + context.CameraController.MainCamera.transform.eulerAngles.y;
        context.transform.rotation = Quaternion.Euler(0, targetAngle, 0);

        moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        predictedRollTime = context.RollDistance / (context.RollMultiplier * context.Movespeed);
        currentRollTime = 0;
    }

    public override void UpdateState()
    {
        if (CheckSwitchState())
            return;

        if (Vector3.Distance(startPos, context.transform.position) < context.RollDistance && currentRollTime < predictedRollTime)
        {
            context.transform.position += moveDir.normalized * context.Movespeed * context.RollMultiplier * Time.deltaTime;
            currentRollTime += Time.deltaTime;
        }
        else
        {
            //uncomment this if you want the player to be able to move even if the roll animation isn't fully finioshed yet, smoother but makes roll spammable
            context.transform.position += moveDir.normalized * context.CurrentMovement.normalized.magnitude * context.Movespeed * Time.deltaTime;
            rolled = true;
        } 
    }

    public override void ExitState()
    {
        context.Invincible = false;
    }

    public override void InitializeSubState()
    {

    }

    public override bool CheckSwitchState()
    {
        if(rolled && !context.Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") || currentRollTime >= predictedRollTime && !context.Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
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
            else if (context.IsRunning())
            {
                SwitchState(factory.Run());
                return true;
            }
        }
        return false;
    }
}
