using Ricimi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_ScorePopup : UI_Popup
{
    [System.Serializable]
    private class PlayerSlot
    {
        public Image iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI stateText;
        public TextMeshProUGUI killText;
        public TextMeshProUGUI deadText;
        public TextMeshProUGUI assistantText;
    }
    
    [SerializeField] private PlayerSlot[] teamASlots = new PlayerSlot[5];   // 팀A (0 ~ 4)
    [SerializeField] private PlayerSlot[] teamBSlots = new PlayerSlot[5];   // 팀B (5 ~ 9)
    [SerializeField] private Sprite[] iconSprites;       // 설정 창에서 선택한 아이콘 idx와 매핑



    void Start()
    {
        UpdateScoreboard(Managers.GameSceneManager.GameKdaData.GetTeamData(0), Managers.GameSceneManager.GameKdaData.GetTeamData(1));
    } 

    public override void Init()
    {
        base.Init();
        InitializeSlots();
    }
    
    // 팀 슬롯 초기화
    private void InitializeSlots()
    {
        for (int i = 0; i < teamASlots.Length; i++)
        {
            ClearSlot(teamASlots[i]);   // 팀A
        }
        
        for (int i = 0; i < teamBSlots.Length; i++)
        {
            ClearSlot(teamBSlots[i]);   // 팀B
        }
    }
    
    // 개별 슬롯 데이터 삭제
    private void ClearSlot(PlayerSlot slot)
    {
        slot.iconImage.sprite = null;
        slot.iconImage.enabled = false;
        slot.nameText.text = "";
        slot.stateText.text = "";
        slot.killText.text = "";
        slot.deadText.text = "";
        slot.assistantText.text = "";
    }
    
    public void UpdateScoreboard(List<KDAData> teamAPlayers, List<KDAData> teamBPlayers)
    {
        // 팀A 업데이트
        for (int i = 0; i < teamASlots.Length; i++)
        {
            if (i < teamAPlayers.Count)
            {
                UpdateSlot(teamASlots[i], teamAPlayers[i]);
            }
            else
            {
                ClearSlot(teamASlots[i]);
            }
        }

        // 팀B 업데이트
        for (int i = 0; i < teamBSlots.Length; i++)
        {
            if (i < teamBPlayers.Count)
            {
                UpdateSlot(teamBSlots[i], teamBPlayers[i]);
            }
            else
            {
                ClearSlot(teamBSlots[i]);
            }
        }
    }
    
    private void UpdateSlot(PlayerSlot slot, KDAData playerData)
    {
        
        // ID, 이름, 팀, 킬 데스 어시, 살아있는지



        bool isRemotePlayer = Managers.GameSceneManager.PlayerManager.TryGetPlayer((uint)playerData.playerId, out var controller);
        bool isAlive = false;
        string name = "";

        if (isRemotePlayer)
        {
            isAlive = controller.GetComponent<PlayerStatHandler>().CurrentHealth > 0;
            name = controller.PlayerStateData.name;

        }
        else
        {
            isAlive = Managers.Player.GetComponent<PlayerStatHandler>().CurrentHealth > 0;
            name = Managers.GameSceneManager.Nickname;
        }

        // 닉네임 (자신의 데이터면 DataManager에서 가져오고 아니면 ID 사용헤애 함)
        slot.nameText.text = name == "" ? "User": name;
        
        // 상태
        slot.stateText.text = isAlive ? "Alive" : "Dead";

        // KDA
        slot.killText.text = playerData.kill.ToString();
        slot.deadText.text = playerData.death.ToString();
        slot.assistantText.text = playerData.assist.ToString();

        // 아이콘 
        int iconIndex = (playerData.playerId == Managers.GameSceneManager.PlayerId) ? Managers.Data.SelectedIconIndex : 0;
        if (iconIndex >= 0 && iconIndex < iconSprites.Length)
        {
            slot.iconImage.sprite = iconSprites[iconIndex];
            slot.iconImage.enabled = true;
        }
        else
        {
            slot.iconImage.enabled = false;
        }
    }
    
    // 테스트용





}
