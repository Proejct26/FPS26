/// <summary>
/// 플레이어의 이동(Move) 상태
/// </summary>
public class PlayerMoveState : IPlayerState
{
    private PlayerController controller;

    public PlayerMoveState(PlayerController ctrl) => controller = ctrl;

    public void Enter() {/* 애니메이션 등 */  }

    /// <summary>
    /// 이동 및 중력 처리 후 상태 전환 조건 검사 (입력 없음/점프/낙하)
    /// </summary>
    public void Update()
    {
        controller.HandleMovement();
        controller.ApplyGravity();

        if (!controller.HasMoveInput())
            controller.StateMachine.ChangeState(new PlayerIdleState(controller));

        if (!controller.IsGrounded())
            controller.StateMachine.ChangeState(new PlayerFallState(controller));

        if (controller.IsJumpInput())
            controller.StateMachine.ChangeState(new PlayerJumpState(controller));
    }
    public void Exit() { }
}