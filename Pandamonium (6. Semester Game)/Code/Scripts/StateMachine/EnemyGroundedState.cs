using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyBaseState
{
    public EnemyGroundedState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory)
   : base (currentContext, playerStateFactory){}


    public override void CheckSwitchStates() { }

    public override void EnterState() {
        Debug.Log("hello from the grounded state");
    }

    public override void ExitState() { }

    public override void InitializeSubStates() { }

    public override void UpdateState() { }

}
