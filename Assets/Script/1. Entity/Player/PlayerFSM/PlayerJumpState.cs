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
        // RemotePlayerController일 경우 공중 상태 설정
        if (_controller is RemotePlayerController remote)
        {
            remote.SetAirborne(true);
        }

        _controller.StartJump();
        _controller.animator.Play("jump forward");
    }

    /// <summary>
    /// 이동 및 중력 처리 후, 수직 속도 감소 시 낙하 상태로 전환
    /// </summary>
    public void Update()
    {
        _controller.HandleMovement();
        _controller.ApplyGravity();

        if (_controller.GetVerticalVelocity() <= 0)
        {
            _controller.StateMachine.ChangeState(new PlayerFallState(_controller));
        }
    }

    public void Exit()
    {
        if (_controller is RemotePlayerController remote)
        {
            remote.SetAirborne(false);
        }
    }
}