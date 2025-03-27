using UnityEngine;

/// <summary>
/// 상태 변화 행동 취하기 위한 인터페이스
/// </summary>
public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

public class PlayerStateMachine
{
    private IPlayerState currentState;

    /// <summary>
    /// 현재의 상태를 종료하고 새로운 상태를 실행하는 함수
    /// </summary>
    /// <param name="newState"></param>
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