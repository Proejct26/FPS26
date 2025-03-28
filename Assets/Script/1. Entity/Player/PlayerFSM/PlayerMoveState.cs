using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// 플레이어의 이동(Move) 상태
/// </summary>
public class PlayerMoveState : IPlayerState
{
    private PlayerController _controller;

    public PlayerMoveState(PlayerController ctrl) => _controller = ctrl;

    public void Enter()
    {
        PlayWalkingIfNotAlready();
    }

    /// <summary>
    /// 이동 및 중력 처리 후 상태 전환 조건 검사 (입력 없음/점프/낙하)
    /// </summary>
    public void Update()
    {
        _controller.HandleMovement();
        _controller.ApplyGravity();

        if (_controller.HasMoveInput())
        {
            PlayWalkingIfNotAlready();
        }


        if (!_controller.HasMoveInput())
            _controller.StateMachine.ChangeState(new PlayerIdleState(_controller));

        if (!_controller.IsGrounded())
            _controller.StateMachine.ChangeState(new PlayerFallState(_controller));

        if (_controller.IsJumpInput())
            _controller.StateMachine.ChangeState(new PlayerJumpState(_controller));
    }
    public void Exit() { }

    private void PlayWalkingIfNotAlready()
    {
        AnimatorStateInfo state = _controller.Animator.GetCurrentAnimatorStateInfo(0);
        if (!state.IsName("walking"))
        {
            _controller.Animator.Play("walking");
        }
    }
}