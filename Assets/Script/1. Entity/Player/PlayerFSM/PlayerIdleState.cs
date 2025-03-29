/// <summary>
/// 플레이어의 정지(Idle) 상태
/// </summary>
public class PlayerIdleState : IPlayerState
{
    private readonly PlayerControllerBase _controller;

    public PlayerIdleState(PlayerControllerBase controller) => _controller = controller;

    public void Enter() { _controller.animator.Play("firing rifle"); }
    public void Update()
    {
        /// <summary>
        /// 중력을 적용하고, 이동/점프/낙하 입력에 따라 상태 전환을 처리
        /// </summary>
        if (_controller.HasMoveInput())
            _controller.StateMachine.ChangeState(new PlayerMoveState(_controller));

        if (!_controller.IsGrounded())
            _controller.StateMachine.ChangeState(new PlayerFallState(_controller));

        if (_controller.IsJumpInput())
            _controller.StateMachine.ChangeState(new PlayerJumpState(_controller));

        if (_controller.IsFiring())
            _controller.StateMachine.ChangeState(new PlayerAttackState(_controller));
    }
    public void Exit() { }
}