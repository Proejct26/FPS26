using UnityEngine;

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

public class PlayerStateMachine
{
    private IPlayerState currentState;

    public void ChangeState(IPlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}