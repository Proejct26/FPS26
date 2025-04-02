
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
    public bool IsScorePopup { get; private set; }

    private UI_ScorePopup _scorePopup;
    private UI_Chat _chatUI;
    private bool _isChatActive;
    
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
        // IsScorePopup = Managers.Input.GetInput(EPlayerInput.Info).IsPressed();
        
        // Tab 체크
        bool newScorePopupState = Managers.Input.GetInput(EPlayerInput.Info).IsPressed();
        if (newScorePopupState != IsScorePopup)
        {
            IsScorePopup = newScorePopupState;
            if (IsScorePopup && _scorePopup == null)
            {
                _scorePopup = Managers.UI.ShowPopupUI<UI_ScorePopup>("ScorePopup");
            }
            else if (!IsScorePopup && _scorePopup != null)
            {
                Managers.UI.ClosePopupUI(_scorePopup);
                _scorePopup = null;
            }
        }
        
        // 채팅
        if (_chatUI == null)
        {
            _chatUI = Managers.UI.ShowSceneUI<UI_Chat>("ChatUI");
        }
        
        // 엔터 키로 채팅 토글
        if (Managers.Input.GetInput(EPlayerInput.Chat).WasPressedThisFrame())
        {
            if (!_isChatActive)
            {
                _isChatActive = true;
                _chatUI.ToggleChat(true); // 입력창 켜기
            }
            else
            {
                _chatUI.SendMessage(); // 현재 모드로 전송
                _isChatActive = false;
            }
        }
        
        // 탭 키로 챗모드 전환
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _chatUI.ToggleChatMode();
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
}
