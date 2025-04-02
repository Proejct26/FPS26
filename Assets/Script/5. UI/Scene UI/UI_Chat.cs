using Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Chat : UI_Base
{
    [SerializeField] private ScrollRect _scrollRect;        // 채팅 로그 + 스크롤바
    [SerializeField] private TMP_InputField _inputField;    // 메세지 입력 필드
    [SerializeField] private Transform _content;            // ScrollRect의 Content 오브젝트
    [SerializeField] private GameObject _myMessage;         // 내 메시지 프리팹
    [SerializeField] private GameObject _otherMessage;      // 상대 메시지 프리팹
    [SerializeField] private TextMeshProUGUI _chatModeText; // 채팅창 좌측 [전체] / [팀]

    private bool _isTeamChat = false;
    private bool _isChatActive = false;
    public bool IsChatActive => _isChatActive;
    public override void Init()
    {
        
    }

    public void Awake()
    {
        _inputField.gameObject.SetActive(false);
        _scrollRect.gameObject.SetActive(true);
        UpdateChatMode();
    }

    private void Start()
    {
        Managers.Input.GetInput(EPlayerInput.Chat).started += InputChat;
        Managers.Input.GetInput(EPlayerInput.Tab).started += InputChatToggleMode;  
        Managers.Chat.OnMessageAdded += AddMessage;
    }

    // 상대 메세지 테스트용

    // 입력한 메시지를 채팅창에 추가
    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(_inputField.text))    // 텍스트 확인
        {
            var myInfo = Managers.Chat.GetPlayerInfo((uint)Managers.GameSceneManager.PlayerId);
            ChatMessageData message = new ChatMessageData()
            {
                PlayerId = (uint)Managers.GameSceneManager.PlayerId, 
                Nickname = myInfo.Nickname,
                Message = _inputField.text,
                TeamId = myInfo.TeamId, 
                IsTeamChat = _isTeamChat
            };

            AddMessage(message); 

            CS_SEND_MESSAGE_ALL sendMessageAllPacket = new CS_SEND_MESSAGE_ALL()
            {
                Message = _inputField.text,
                PlayerId = (uint)Managers.GameSceneManager.PlayerId, 
            };
            Managers.Network.Send(sendMessageAllPacket); 
 

        }
        ToggleChat(false);
    }
    
    // 받은 메세지 보드에 추가
    public void AddMessage(ChatMessageData data)
    {
        if (data.IsTeamChat && data.TeamId != Managers.Chat.GetMyTeamId())  // 같은 팀채팅만 표시
        {
            return; // 다른 팀채팅 무시
        }
        
        bool isSelf = data.PlayerId == Managers.GameSceneManager.PlayerId;   // 내 메세지인지 확인




        GameObject prefab = isSelf ? _myMessage : _otherMessage;
        GameObject messageObj = Instantiate(prefab, _content);
        
        string messagePrefix = data.IsTeamChat ? "[팀]  " : "[전체]  ";

        if (isSelf) // 내 메세지 처리
        {
            TextMeshProUGUI messageText = messageObj.transform.Find("Message")?.GetComponent<TextMeshProUGUI>();
            messageText.text = messagePrefix + data.Message;
            messageText.color = Color.white;
        }
        else
        {
            TextMeshProUGUI nameText = messageObj.transform.Find("NickName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI bodyText = messageObj.transform.Find("Message")?.GetComponent<TextMeshProUGUI>();
 
            nameText.text = data.Nickname;
            bodyText.text = messagePrefix + data.Message;
            
            Color myTeamColor = new Color(109f / 255f, 204f / 255f, 255f / 255f);
            Color enemyTeamColor = new Color(255f / 255f, 117f / 255f, 109f / 255f);
            
            nameText.color = data.TeamId == Managers.Chat.GetMyTeamId() ? myTeamColor : enemyTeamColor;
        }
        ScrollToBottom();
    }




    // 입력창, 스크롤바 On/Off (엔터키)
    public void ToggleChat(bool active)
    {
        _inputField.gameObject.SetActive(active);
        if (active)
        {
            _inputField.ActivateInputField();
            _inputField.text = "";
            ScrollToBottom();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            UpdateChatMode();
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Managers.Input.SetActive(true);
        }
    }
    
    // 탭 키로 팀채팅 모드 전환
    public void ToggleChatMode()
    {
        if (_inputField.gameObject.activeSelf) // 입력창 켜진 상태에서만 전환
        {
            _isTeamChat = !_isTeamChat;
            UpdateChatMode();
        }
    }
    
    // [전체] / [팀] 모드 전환
    private void UpdateChatMode()
    {
        if (_chatModeText != null)
        {
            _chatModeText.text = _isTeamChat ? "[팀]" : "[전체]";
        }
    }

    // 스크롤 맨 아래로 이동
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
    }


    private void InputChat(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;
        
        if (!_isChatActive)
        {
            _isChatActive = true;
            ToggleChat(true); // 입력창 켜기
        }
        else
        {
            SendMessage(); // 메시지 전송
            _isChatActive = false;
 
        }

        // 채팅 활성화 시 다른 입력 무시
        // if (_isChatActive)
        // {
           
        // }
    }

    private void InputChatToggleMode(InputAction.CallbackContext context)
    {
        ToggleChatMode();
        
    }

}
