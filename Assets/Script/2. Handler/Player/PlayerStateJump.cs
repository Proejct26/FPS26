
public class PlayerJumpState : IPlayerState
{
    private PlayerController controller;

    public PlayerJumpState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.StartJump();
    }

    public void Update()
    {
        controller.HandleMovement();
        controller.ApplyGravity();

        if (controller.GetVerticalVelocity() <= 0)
            controller.StateMachine.ChangeState(new PlayerFallState(controller));
    }

    public void Exit() { }
}