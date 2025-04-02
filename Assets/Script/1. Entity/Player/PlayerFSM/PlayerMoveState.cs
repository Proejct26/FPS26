using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// 플레이어의 이동(Move) 상태
/// </summary>
public class PlayerMoveState : IPlayerState
{
    private readonly PlayerControllerBase _controller;

    public PlayerMoveState(PlayerControllerBase ctrl) => _controller = ctrl;

    public void Enter() {}

    /// <summary>
    /// 이동 및 중력 처리 후 상태 전환 조건 검사 (입력 없음/점프/낙하)
    /// </summary>
    public void Update()
    {
        UpdateAnimation();

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

    private void UpdateAnimation()
    {
        Vector3 move = _controller.GetMoveInput();

        Vector3 forward = _controller.transform.forward;
        Vector3 right = _controller.transform.right;
        Vector3 moveDir = move.normalized;

        float forwardAmount = Vector3.Dot(forward, moveDir);
        float rightAmount = Vector3.Dot(right, moveDir);

        string animToPlay = "";

        if (Mathf.Abs(forwardAmount) >= Mathf.Abs(rightAmount))
        {
            animToPlay = forwardAmount > 0 ? "walking" : "walking backwards";
        }
        else
        {
            animToPlay = rightAmount > 0 ? "strafe(2)" : "strafe";
        }

        // 중복 재생 방지
        if (!_controller.animator.GetCurrentAnimatorStateInfo(0).IsName(animToPlay))
        {
            _controller.animator.Play(animToPlay);
        }
    }
}