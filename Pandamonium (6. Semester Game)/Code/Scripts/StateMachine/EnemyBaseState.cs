using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EnemyBaseState
{
    protected EnemyHierarchicalStateMachine _ctx;
    protected EnemyStateFactory _factory;
    public EnemyBaseState(EnemyHierarchicalStateMachine currentContext, EnemyStateFactory enemyStateFactory)
    {
        _ctx = currentContext;
        _factory = enemyStateFactory;
    }
    public abstract void EnterState();

    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubStates();

    void UpdateStates() { }
    public void SwitchState(EnemyBaseState newstate) {
        ExitState();

        newstate.EnterState();
        _ctx.currentState = newstate;
    }
    protected void SetSuperState() { }
    protected void SetSubState() { }
}
