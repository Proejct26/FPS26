using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScorePopup : MonoBehaviour
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
    
    [SerializeField] private PlayerSlot[] teamASlots = new PlayerSlot[5]; // 팀A (0 ~ 4)
    [SerializeField] private PlayerSlot[] teamBSlots = new PlayerSlot[5]; // 팀B (5 ~ 9)
    [SerializeField] private Sprite[] iconSprites; // CustomPopup에서 선택한 인덱스와 매핑
    
    private void Start()
    {
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
    
    public void UpdateScoreboard(List<PlayerStateData> teamAPlayers, List<PlayerStateData> teamBPlayers)
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
    
    private void UpdateSlot(PlayerSlot slot, PlayerStateData playerData)
    {
        // 닉네임 (자신의 데이터면 DataManager에서 가져오고 아니면 ID 사용헤애 함)
        slot.nameText.text = (playerData.id == Managers.Data.Nickname) ? Managers.Data.Nickname : playerData.id;

        // 상태
        slot.stateText.text = playerData.isAlive ? "Alive" : "Dead";

        // KDA
        slot.killText.text = playerData.kills.ToString();
        slot.deadText.text = playerData.deaths.ToString();
        slot.assistantText.text = playerData.assists.ToString();

        // 아이콘 (자신의 데이터면 DataManager에서, 아니면 기본값)
        int iconIndex = (playerData.id == Managers.Data.Nickname) ? Managers.Data.SelectedIconIndex : 0;
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<PlayerStateData> teamA = new List<PlayerStateData>
            {
                new PlayerStateData { id = Managers.Data.Nickname, kills = 1, deaths = 2, assists = 3, isAlive = true }, // 내 데이터
                new PlayerStateData { id = "Player2", kills = 0, deaths = 1, assists = 0, isAlive = false }
            };
            List<PlayerStateData> teamB = new List<PlayerStateData>
            {
                new PlayerStateData { id = "Player3", kills = 3, deaths = 2, assists = 1, isAlive = true }
            };
            UpdateScoreboard(teamA, teamB);
        }
    }
}
