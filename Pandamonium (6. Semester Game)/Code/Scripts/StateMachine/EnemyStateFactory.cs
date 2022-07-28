public class EnemyStateFactory 
{
    EnemyHierarchicalStateMachine _context;
    public EnemyStateFactory (EnemyHierarchicalStateMachine currentContext)
    {
        _context = currentContext;
    }

    public EnemyBaseState Idle() {
        return new EnemyIdleState(_context, this);
    }

    public EnemyBaseState Walk() {
        return new EnemyWalkState(_context, this);
    }

    public EnemyBaseState Attack(){
        return new EnemyAttackState(_context, this);
    }

    public EnemyBaseState Jump(){
        return new EnemyJumpState(_context, this);
    }

    public EnemyBaseState Grounded(){
        return new EnemyGroundedState(_context, this);
    }

    public EnemyBaseState AttackWindup()
    {
        return new EnemyAttackWindupState(_context, this);
    }

    //PandaBoss
    public EnemyBaseState PandaBossAttacks(bool desperate)
    {
        return new PandaBossAttackState(_context, this, desperate);
    }

    public EnemyBaseState PandaBossTransition(PandaBossStateMachine pandaStatemachine)
    {
        return new PandaBossTransitionState(_context, this, pandaStatemachine);
    }

    public EnemyBaseState PandaBossMeteor(PandaBossStateMachine pandaStatemachine)
    {
        return new PandaBossMeteorState(_context, this, pandaStatemachine);
    }

    public EnemyBaseState PandaBossWalk(PandaBossStateMachine pandaStatemachine)
    {
        return new PandaBossWalkState(_context, this, pandaStatemachine);
    }
}
