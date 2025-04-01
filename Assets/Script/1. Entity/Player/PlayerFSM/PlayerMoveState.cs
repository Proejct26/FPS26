using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// 플레이어의 이동(Move) 상태
/// </summary>
public class PlayerMoveState : IPlayerState
{
    private readonly PlayerControllerBase _controller;

    public PlayerMoveState(PlayerControllerBase ctrl) => _controller = ctrl;

    public void Enter()
    {
        _controller.animator.Play("walking");
    }

    /// <summary>
    /// 이동 및 중력 처리 후 상태 전환 조건 검사 (입력 없음/점프/낙하)
    /// </summary>
    public void Update()
    {
        _controller.HandleMovement();
        _controller.ApplyGravity();

        if (!_controller.HasMoveInput())
            _controller.StateMachine.ChangeState(new PlayerIdleState(_controller));
        if (!_controller.IsGrounded())
            _controller.StateMachine.ChangeState(new PlayerFallState(_controller));
        if (_controller.IsJumpInput() && _controller.IsGrounded())
            _controller.StateMachine.ChangeState(new PlayerJumpState(_controller));
    }
    public void Exit() { }

}