/// <summary>
/// 플레이어의 점프(Jump) 상태
/// 점프 입력 시 진입하며, 수직 속도가 0 이하가 되면 낙하 상태로 전환
/// </summary>
public class PlayerJumpState : IPlayerState
{
    private readonly PlayerControllerBase _controller;

    public PlayerJumpState(PlayerControllerBase ctrl) => _controller = ctrl;

    /// <summary>
    /// 점프 시작 시 수직 속도 초기화
    /// </summary>
    public void Enter()
    {
        if (_controller is LocalPlayerController local)
        {
            local.StartJump();
        }
        _controller.animator.Play("jump forward");
    }
    /// <summary>
    /// 이동 및 중력 처리 후, 수직 속도 감소 시 낙하 상태로 전환
    /// </summary>
    public void Update()
    {
        if (_controller is LocalPlayerController local)
        {
            local.HandleMovement();
            local.ApplyGravity();

            if (local.GetVerticalVelocity() <= 0)
                _controller.StateMachine.ChangeState(new PlayerFallState(_controller));
        }
    }

    public void Exit() { }
}