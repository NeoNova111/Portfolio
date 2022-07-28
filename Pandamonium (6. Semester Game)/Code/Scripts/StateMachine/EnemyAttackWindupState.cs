using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackWindupState : EnemyBaseState
{
    public EnemyAttackWindupState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    
    public override void CheckSwitchStates()
    {

    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubStates() { }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}