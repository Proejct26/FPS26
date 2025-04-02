using Game;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 플레이어의 입력을 받아 저장해두는 클래스.
/// FSM이나 네트워크 동기화, 컨트롤러 등에서 가져다 씀.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveInput { get; private set; } = Vector3.zero;      // 이동 방향 (Vector3)
    public Vector3 LookInput { get; private set; } = Vector2.zero;     // 시선 방향 (Vector3)

    public float RotationX { get; private set; } = 0f;       // 마우스 X 회전 (pitch)
    public float RotationY { get; private set; } = 0f;       // 마우스 Y 회전 (yaw)

    public bool IsJumping { get; private set; } = false;
    public bool IsFiring { get; private set; } = false;

    public bool IsZooming { get; private set; } = false;
    public bool IsScorePopup { get; private set; } = false;

    private UI_ScorePopup _scorePopup;
    private UI_Chat _chatUI;
    private bool _isChatActive;
    
    private float _prevRotationX = 0f; 
    private float _prevRotationY = 0f; 


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
    private void Update()
    {
        UpdateInput(); 
    }


    /// <summary>
    /// 매 프레임 입력 갱신
    /// </summary>
    public void UpdateInput() 
    {
        // // WASD 이동 입력 (Vector2 → Vector3 변환)
        // Vector2 input2D = Managers.Input.GetInput(EPlayerInput.move).ReadValue<Vector2>();
        // MoveInput = new Vector3(input2D.x, 0f, input2D.y);

        // // 마우스 회전 (Vector2 → rotation 값으로 직접 사용)
        // Vector2 look = Managers.Input.GetInput(EPlayerInput.Look).ReadValue<Vector2>();
        // RotationX = look.y;  // 상하 (Pitch)
        // RotationY = look.x;  // 좌우 (Yaw)    

        // // LookInput 벡터 방향 (기본적으로 정면 방향 기준 벡터. 카메라 forward 등에서 계산 가능)
        // LookInput = new Vector2(RotationX, RotationY);

        // IsJumping = Managers.Input.GetInput(EPlayerInput.Jump).IsPressed();
        // IsFiring = Managers.Input.GetInput(EPlayerInput.Fire).IsPressed();
        // IsScorePopup = Managers.Input.GetInput(EPlayerInput.Info).IsPressed();


        
        // 채팅
        // if (_chatUI == null)
        // {
        //     _chatUI = Managers.UI.ShowSceneUI<UI_Chat>("ChatUI");
        // }
        
        // // 엔터 키로 채팅 토글
        // if (Managers.Input.GetInput(EPlayerInput.Chat).WasPressedThisFrame())
        // {
            
        // }
        



    }

    private void InputLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
        
        // 현재 회전값을 5도 단위로 반올림
        float currentRotationX = Mathf.Round(LookInput.x / 5f) * 5f;
        float currentRotationY = Mathf.Round(LookInput.y / 5f) * 5f;
        
        // 이전 회전값과 현재 회전값의 차이가 5도 이상일 때만 서버로 전송
        if (Mathf.Abs(currentRotationX - _prevRotationX) >= 5f || 
            Mathf.Abs(currentRotationY - _prevRotationY) >= 5f)
        {
            _prevRotationX = currentRotationX;
            _prevRotationY = currentRotationY;
            SendInput();
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
        SendInput();
    }

    private void InputJump(InputAction.CallbackContext context)
    {
        IsJumping = context.phase != InputActionPhase.Canceled;
        SendInput();
    }
 
    private void SendInput()   
    {
        CS_KEY_INPUT keyInput = new CS_KEY_INPUT();
        keyInput.KeyA = (uint)(MoveInput.x > 0.5f ? 1 : 0);
        keyInput.KeyD = (uint)(MoveInput.x < -0.5f ? 1 : 0); 
        keyInput.KeyW = (uint)(MoveInput.z > 0.5f ? 1 : 0);
        keyInput.KeyS = (uint)(MoveInput.z < -0.5f ? 1 : 0);
        keyInput.RotateAxisX = (uint)LookInput.x;
        keyInput.RotateAxisY = (uint)LookInput.y;
        keyInput.Jump = (uint)(IsJumping ? 1 : 0);

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
