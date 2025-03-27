using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController controller;
    public PlayerAttackState(PlayerController ctrl) => controller = ctrl;

    public void Enter()
    {
        //애니메이션 출력
        controller.HandleFire();                    // 무기 발사
    }

    public void Update()
    {
        //시간 경과 or 애니메이션 종료 시 다른 state로 이동?
    }
    public void Exit()
    {
        //만약 애니메이션이 작동 중이더라도 끊고 넘어가야 하나?
    }
}
