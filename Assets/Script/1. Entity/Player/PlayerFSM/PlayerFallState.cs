/// <summary>
/// 플레이어의 낙하(Fall) 상태
/// 공중에 있을 때 진입하며, 땅에 닿으면 Idle 상태로 전환
/// </summary>
public class PlayerFallState : IPlayerState
{
    private PlayerController controller;

    public PlayerFallState(PlayerController ctrl) => controller = ctrl;


    public void Enter() { /* 애니메이션 등 */ }

    /// <summary>
    /// 이동 및 중력 처리 후, 바닥에 닿으면 Idle 상태로 전환
    /// </summary>
    public void Update()
    {
        controller.HandleMovement();
        controller.ApplyGravity();

        if (controller.IsGrounded())
            controller.StateMachine.ChangeState(new PlayerIdleState(controller));
    }

    public void Exit() { }
}