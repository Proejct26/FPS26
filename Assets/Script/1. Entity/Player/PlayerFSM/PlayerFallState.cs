/// <summary>
/// 플레이어의 낙하(Fall) 상태
/// 공중에 있을 때 진입하며, 땅에 닿으면 Idle 상태로 전환
/// </summary>
public class PlayerFallState : IPlayerState
{
    private PlayerController _controller;

    public PlayerFallState(PlayerController ctrl) => _controller = ctrl;


    public void Enter() { /* 애니메이션 등 */ }

    /// <summary>
    /// 이동 및 중력 처리 후, 바닥에 닿으면 Idle 상태로 전환
    /// </summary>
    public void Update()
    {
        _controller.HandleMovement();
        _controller.ApplyGravity();

        if (_controller.IsGrounded())
            _controller.StateMachine.ChangeState(new PlayerIdleState(_controller));
    }

    public void Exit() { }
}