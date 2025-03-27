public class PlayerFallState : IPlayerState
{
    private PlayerController controller;

    public PlayerFallState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter() { }

    public void Update()
    {
        controller.HandleMovement();
        controller.ApplyGravity();

        if (controller.IsGrounded())
            controller.StateMachine.ChangeState(new PlayerIdleState(controller));
    }

    public void Exit() { }
}