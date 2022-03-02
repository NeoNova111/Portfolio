public class PlayerStateFactory
{
    PlayerStateMachine context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;
    }

    public PlayerBaseState Idle()
    {
        return new PlayerIdleState(context, this);
    }

    public PlayerBaseState Walk()
    {
        return new PlayerWalkState(context, this);
    }

    public PlayerBaseState Run()
    {
        return new PlayerRunState(context, this);
    }

    public PlayerBaseState Jump()
    {
        return new PlayerJumpState(context, this);
    }

    public PlayerBaseState Fall()
    {
        return new PlayerFallState(context, this);
    }

    public PlayerBaseState Grounded()
    {
        return new PlayerGroundedState(context, this);
    }

    public PlayerBaseState Attack()
    {
        return new PlayerAttackState(context, this);
    }

    public PlayerBaseState Dodge()
    {
        return new PlayerRollState(context, this);
    }

    public PlayerBaseState LockOn()
    {
        return new PlayerLockOnState(context, this);
    }

    public PlayerBaseState Interact(IInteractable interactable)
    {
        return new PlayerInteractingState(context, this, interactable);
    }

    public PlayerBaseState ISG()
    {
        return new PlayerInfiniteSworGlitchState(context, this);
    }

    public PlayerBaseState Alive()
    {
        return new PlayerAliveState(context, this);
    }

    public PlayerBaseState Dead()
    {
        return new PlayerDeadState(context, this);
    }

    public PlayerBaseState Healing()
    {
        return new PlayerHealingState(context, this);
    }

    public PlayerBaseState Hit()
    {
        return new PlayerHitState(context, this);
    }
}
