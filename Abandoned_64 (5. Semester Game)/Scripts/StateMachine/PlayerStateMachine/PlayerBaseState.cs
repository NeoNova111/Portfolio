public abstract class PlayerBaseState
{
    protected bool isRootState = false;
    protected PlayerStateMachine context;
    protected PlayerStateFactory factory;
    protected PlayerBaseState currentSuperState;
    protected PlayerBaseState currentSubState;

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        context = currentContext;
        factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract bool CheckSwitchState();

    public abstract void InitializeSubState();

    public void UpdateStates()
    {       
        if (currentSubState != null)
        {
            currentSubState.UpdateStates();
        }

        if (isRootState && context.RootChanged)
        {
            context.RootChanged = false;
            return;
        }


        UpdateState();
    }

    public void ExitStates()
    {
        if (currentSubState != null)
        {
            currentSubState.ExitStates();
        }
        ExitState();
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        ExitStates();
        newState.EnterState();

        if (isRootState)
        {
            context.RootState = newState;
        }
        else if (currentSuperState != null)
        {
            currentSuperState.PassOnSubState(newState);
        }
    }

    public void ForceSwitch(PlayerBaseState newstate)
    {
        SwitchState(newstate);
    }

    protected void NewRoot(PlayerBaseState newrootState)
    {
        context.RootState.ExitStates();
        context.RootState = newrootState;
        context.RootState.EnterState();
        context.RootChanged = true;
    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        if (currentSubState != null)
        {
            currentSubState.ExitStates();
        }

        if (newSubState == null)
        {
            currentSubState = null;
        }
        else
        {
            newSubState.EnterState();
            newSubState.SetSuperState(this);
            currentSubState = newSubState;
        }
    }

    public string PrintHirarchy(PlayerBaseState state)
    {
        if (state.currentSubState != null)
        {
            return state.ToString() + " > " + PrintHirarchy(state.currentSubState);
        }

        return state.ToString();
    }

    protected void PassOnSubState(PlayerBaseState newSubState)
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
