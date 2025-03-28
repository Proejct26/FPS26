﻿
using UnityEngine;


/// <summary>
/// 플레이어의 입력을 받아 저장해두는 클래스.
/// FSM이나 네트워크 동기화, 컨트롤러 등에서 가져다 씀.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveInput { get; private set; }     // 이동 방향 (Vector3)
    public Vector3 LookInput { get; private set; }     // 시선 방향 (Vector3)

    public float RotationX { get; private set; }       // 마우스 X 회전 (pitch)
    public float RotationY { get; private set; }       // 마우스 Y 회전 (yaw)

    public bool IsJumping { get; private set; }
    public bool IsFiring { get; private set; }

    public bool IsZooming { get; private set; }

    private void Update()
    {
        UpdateInput();
    }


    /// <summary>
    /// 매 프레임 입력 갱신
    /// </summary>
    public void UpdateInput()
    {
        // WASD 이동 입력 (Vector2 → Vector3 변환)
        Vector2 input2D = Managers.Input.GetInput(EPlayerInput.move).ReadValue<Vector2>();
        MoveInput = new Vector3(input2D.x, 0f, input2D.y);

        // 마우스 회전 (Vector2 → rotation 값으로 직접 사용)
        Vector2 look = Managers.Input.GetInput(EPlayerInput.Look).ReadValue<Vector2>();
        RotationX = look.y;  // 상하 (Pitch)
        RotationY = look.x;  // 좌우 (Yaw)

        // LookInput 벡터 방향 (기본적으로 정면 방향 기준 벡터. 카메라 forward 등에서 계산 가능)
        LookInput = new Vector2(RotationX, RotationY);

        IsJumping = Managers.Input.GetInput(EPlayerInput.Jump).IsPressed();
        IsFiring = Managers.Input.GetInput(EPlayerInput.Fire).IsPressed();
    }

}
