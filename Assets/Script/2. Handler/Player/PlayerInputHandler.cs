using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using System;


/// <summary>
/// 플레이어의 입력을 받아 저장해두는 클래스.
/// FSM이나 네트워크 동기화, 컨트롤러 등에서 가져다 씀.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveInput { get; private set; } = Vector3.zero;      // 이동 방향 (Vector3)
    public Vector2 LookInput { get; private set; } = Vector2.zero;     // 시선 방향 (Vector2)

    public float RotationX { get; private set; } = 0f;       // 마우스 X 회전 (pitch)
    public float RotationY { get; private set; } = 0f;       // 마우스 Y 회전 (yaw)

    public bool IsJumping { get; private set; } = false;
    public bool IsFiring { get; private set; } = false;

    public bool IsZooming { get; private set; } = false;
    public bool IsScorePopup { get; private set; } = false;

    private UI_ScorePopup _scorePopup;
    private UI_Chat _chatUI;
    private bool _isChatActive;
    


    private void Start()
    {
        Managers.Input.GetInput(EPlayerInput.Info).started += InputInfo;
        Managers.Input.GetInput(EPlayerInput.Info).canceled += InputInfo;

        Managers.Input.GetInput(EPlayerInput.Chat).started += InputChat;

        Managers.Input.GetInput(EPlayerInput.Look).performed += InputLook; 

        Managers.Input.GetInput(EPlayerInput.move).performed += InputMove;
        Managers.Input.GetInput(EPlayerInput.move).canceled += InputMove; 

        Managers.Input.GetInput(EPlayerInput.Jump).started += InputJump;
        Managers.Input.GetInput(EPlayerInput.Jump).canceled += InputJump; 
    }

    private void InputLook(InputAction.CallbackContext context)
    {

        // 입력값을 안전하게 읽기
        LookInput = context.ReadValue<Vector2>();
        
        // 현재 회전값을 20도 단위로 반올림
        float currentRotationX = Mathf.Round(LookInput.x / 20f) * 20f;
        float currentRotationY = Mathf.Round(LookInput.y / 20f) * 20f; 
        
        // 이전 회전값과 현재 회전값의 차이가 20도 이상일 때만 서버로 전송
        if (Mathf.Abs(currentRotationX - RotationX) >= 20f || 
            Mathf.Abs(currentRotationY - RotationY) >= 20f) 
        {
            RotationX = currentRotationX; 
            RotationY = currentRotationY;
            SendInput();
            MyDebug.Log($"회전 입력: {currentRotationX}, {currentRotationY}");
        }  
    }

    private void InputMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        if(moveInput.sqrMagnitude < 0.01f)
        {
            MoveInput = Vector3.zero;
        }
        else
        {
            MoveInput = new Vector3(moveInput.x, 0f, moveInput.y);
        }
        MyDebug.Log($"이동 입력: {MoveInput}");
        SendInput();
    }

    private void InputJump(InputAction.CallbackContext context)
    {
        IsJumping = context.phase != InputActionPhase.Canceled;
        MyDebug.Log($"점프 입력: {IsJumping}");
        SendInput();
    }
  
    private void SendInput()   
    {
        if (Managers.Player == null)
            return;
            
        CS_KEY_INPUT keyInput = new CS_KEY_INPUT();
        keyInput.KeyA = MoveInput.x > 0.5f;
        keyInput.KeyD = MoveInput.x < -0.5f;
        keyInput.KeyW = MoveInput.z > 0.5f;
        keyInput.KeyS = MoveInput.z < -0.5f;
        
        // Euler 각도를 0~360도 범위로 정규화
        Vector3 eulerAngles = Managers.Player.transform.rotation.eulerAngles;
        float normalizedX = eulerAngles.x < 0 ? eulerAngles.x + 360f : eulerAngles.x;
        float normalizedY = eulerAngles.y < 0 ? eulerAngles.y + 360f : eulerAngles.y;
        
        keyInput.RotateAxisX = (uint)normalizedX;
        keyInput.RotateAxisY = (uint)normalizedY;
        keyInput.Jump = (uint)(IsJumping ? 1 : 0);
        
        MyDebug.Log($"키 입력: {keyInput.KeyA}, {keyInput.KeyD}, {keyInput.KeyW}, {keyInput.KeyS}, {keyInput.RotateAxisX}, {keyInput.RotateAxisY}, {keyInput.Jump}");
        Managers.Network.Send(keyInput); 
    }

    private void InputChat(InputAction.CallbackContext context)
    {
        if (!_isChatActive)
        {
            _isChatActive = true;
            _chatUI.ToggleChat(true); // 입력창 켜기
         //   Managers.Input.SetActive(false); 
        }
        else
        {
            _chatUI.SendMessage(); // 메시지 전송
            _isChatActive = false;
           // Managers.Input.SetActive(true);
        }

                // 채팅 활성화 시 다른 입력 무시
        if (_isChatActive)
        {
            MoveInput = Vector3.zero;    // 이동 입력 초기화
            LookInput = Vector2.zero; // 방향 입력 초기화
            RotationX = 0f;
            RotationY = 0f;
            IsJumping = false;
            IsFiring = false;
            return;
        }
    }
    
    private void InputInfo(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _scorePopup = Managers.UI.ShowPopupUI<UI_ScorePopup>("ScorePopup");

        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            Managers.UI.ClosePopupUI(_scorePopup);
            _scorePopup = null;
        }
    }
}
