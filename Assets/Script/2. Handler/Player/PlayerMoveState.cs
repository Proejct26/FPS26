public class PlayerMoveState : IPlayerState
{
    private PlayerController controller;

    public PlayerMoveState(PlayerController ctrl) => controller = ctrl;

    public void Enter() { }
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