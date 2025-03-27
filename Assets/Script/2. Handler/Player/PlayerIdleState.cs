public class PlayerIdleState : IPlayerState
{
    private PlayerController controller;

    public PlayerIdleState(PlayerController ctrl) => controller = ctrl;

    public void Enter() { /* 애니메이션 등 */ }
    public void Update()
    {
        if (controller.HasMoveInput())
            controller.StateMachine.ChangeState(new PlayerMoveState(controller));

        if (!controller.IsGrounded())
            controller.StateMachine.ChangeState(new PlayerFallState(controller));

        if (controller.IsJumpInput())
            controller.StateMachine.ChangeState(new PlayerJumpState(controller));
    }
    public void Exit() { }
}