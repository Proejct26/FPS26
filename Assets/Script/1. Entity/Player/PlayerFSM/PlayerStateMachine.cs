using UnityEngine;
using UnityEngine.InputSystem.XR;

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
    private IPlayerState _currentState;
    private PlayerControllerBase _controller;

    public PlayerStateMachine(PlayerControllerBase controller)
    {
        _controller = controller;
    }

    /// <summary>
    /// 현재의 상태를 종료하고 새로운 상태를 실행하는 함수
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IPlayerState newState)
    {
        if (_currentState?.GetType() == newState.GetType()) return;
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void Update()
    {
        UpdateStateTransition();
        _currentState?.Update();
    }
    /// <summary>
    /// 현재 상태, 입력값, 조건에 따라 다음 상태를 판단
    /// </summary>
    private void UpdateStateTransition()
    {
        if (_controller.IsJumpInput() && _controller.IsGrounded())
        {
            ChangeState(new PlayerJumpState(_controller));
        }
        else if (!_controller.IsGrounded())
        {
            ChangeState(new PlayerFallState(_controller));
        }
        else if (_controller.HasMoveInput())
        {
            ChangeState(new PlayerMoveState(_controller));
        }
        else
        {
            ChangeState(new PlayerIdleState(_controller));
        }
    }
}