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
        PlayWalkingIfNotAlready();
    }

    /// <summary>
    /// 이동 및 중력 처리 후 상태 전환 조건 검사 (입력 없음/점프/낙하)
    /// </summary>
    public void Update()
    {
        if (_controller is LocalPlayerController local)
        {
            local.HandleMovement();
            local.ApplyGravity();

            if (!local.HasMoveInput())
                _controller.StateMachine.ChangeState(new PlayerIdleState(_controller));
            else if (!local.IsGrounded())
                _controller.StateMachine.ChangeState(new PlayerFallState(_controller));
            else if (local.IsJumpInput())
                _controller.StateMachine.ChangeState(new PlayerJumpState(_controller));
        }
    }
    public void Exit() { }

    private void PlayWalkingIfNotAlready()
    {
        AnimatorStateInfo state = _controller.animator.GetCurrentAnimatorStateInfo(0);
        if (!state.IsName("walking"))
        {
            _controller.animator.Play("walking");
        }
    }
}