using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Chat : UI_Scene
{
    [SerializeField] private ScrollRect _scrollRect;        // 채팅 로그 + 스크롤바
    [SerializeField] private TMP_InputField _inputField;    // 메세지 입력 필드
    [SerializeField] private Transform _content;            // ScrollRect의 Content 오브젝트
    [SerializeField] private GameObject _myMessage;         // 내 메시지 프리팹
    [SerializeField] private GameObject _otherMessage;      // 상대 메시지 프리팹
    [SerializeField] private TextMeshProUGUI _chatModeText; // 채팅창 좌측 [전체] / [팀]

    private bool _isTeamChat = false;
    
    public override void Init()
    {
        base.Init();
        _inputField.gameObject.SetActive(false);
        _scrollRect.gameObject.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UpdateChatMode();
    }
    
    // 상대 메세지 테스트용
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // P 키로 테스트
        {
            AddMessage(new ChatMessageData
            {
                PlayerId = 3,
                Nickname = "Player2",
                Message = "안녕하세요",
                TeamId = 1
            });
        }
        if (Input.GetKeyDown(KeyCode.L)) // L 키로 테스트
        {
            AddMessage(new ChatMessageData
            {
                PlayerId = 5,
                Nickname = "Player3",
                Message = "잘 부탁드립니다! 즐겜해요~",
                TeamId = 2
            });
        }
        
        // 팀 채팅 테스트
        if (Input.GetKeyDown(KeyCode.O)) // 내 팀
        {
            AddMessage(new ChatMessageData
            {
                PlayerId = 3,
                Nickname = "Player4",
                Message = "왼쪽으로 가죠",
                TeamId = 1,
                IsTeamChat = true
            });
        }
        if (Input.GetKeyDown(KeyCode.K)) // 적팀 (표시 안 돼야 함)
        {
            AddMessage(new ChatMessageData
            {
                PlayerId = 5,
                Nickname = "Player5",
                Message = "저 팀 못하는듯",
                TeamId = 2,
                IsTeamChat = true
            });
        }
    }

    // 입력한 메시지를 채팅창에 추가
    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(_inputField.text))    // 텍스트 확인
        {
            var myInfo = Managers.Chat.GetPlayerInfo(Managers.Network.PlayerId);
            ChatMessageData message = new ChatMessageData()
            {
                PlayerId = Managers.Network.PlayerId,
                Nickname = myInfo.Nickname,
                Message = _inputField.text,
                TeamId = myInfo.TeamId,
                IsTeamChat = _isTeamChat
            };
            Managers.Chat.AddMessage(message);
            AddMessage(message);
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
        
        bool isSelf = data.PlayerId == Managers.Network.PlayerId;   // 내 메세지인지 확인
        GameObject prefab = isSelf ? _myMessage : _otherMessage;
        GameObject messageObj = Instantiate(prefab, _content);
        
        string messagePrefix = data.IsTeamChat ? "[팀]  " : "[전체]  ";

        if (isSelf) // 내 메세지 처리
        {
            TextMeshProUGUI messageText = messageObj.transform.Find("Message")?.GetComponent<TextMeshProUGUI>();
            messageText.text = messagePrefix + data.Message;
            messageText.color = Color.white;

            // TODO 데이터 송신
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
}
