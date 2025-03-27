/// <summary>
/// 플레이어의 정지(Idle) 상태
/// </summary>
public class PlayerIdleState : IPlayerState
{
    private PlayerController controller;

    public PlayerIdleState(PlayerController ctrl) => controller = ctrl;

    public void Enter() { /* 애니메이션 등 */ }
    public void Update()
    {
        /// <summary>
        /// 중력을 적용하고, 이동/점프/낙하 입력에 따라 상태 전환을 처리
        /// </summary>
        if (controller.HasMoveInput())
            controller.StateMachine.ChangeState(new PlayerMoveState(controller));

        if (!controller.IsGrounded())
            controller.StateMachine.ChangeState(new PlayerFallState(controller));

        if (controller.IsJumpInput())
            controller.StateMachine.ChangeState(new PlayerJumpState(controller));
    }
    public void Exit() { }
}