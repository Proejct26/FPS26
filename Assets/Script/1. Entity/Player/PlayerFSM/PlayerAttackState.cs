using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private readonly PlayerControllerBase _controller;
    public PlayerAttackState(PlayerControllerBase ctrl) => _controller = ctrl;

    public void Enter()
    {
        _controller.PlayFireAnim();

        if (_controller is LocalPlayerController local)
        {
            local.HandleFire(true);
        }
    }

    public void Update()
    {
        if (!_controller.IsFiring())
        {
            _controller.StateMachine.ChangeState(new PlayerIdleState(_controller));
        }
        //시간 경과 or 애니메이션 종료 시 다른 state로 이동?
    }
    public void Exit()
    {
        if (_controller is LocalPlayerController local)
        {
            local.HandleFire(false);
        }
        //만약 애니메이션이 작동 중이더라도 끊고 넘어가야 하나?
    }
}
