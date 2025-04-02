using System.Collections.Generic;
using UnityEngine;

public class ChatDataManager : IManager
{
    private List<ChatMessageData> _messages = new List<ChatMessageData>();
    private Dictionary<uint, PlayerInfo> _playerInfos = new Dictionary<uint, PlayerInfo>();

    public void Init()
    {
        _messages.Clear();
        _playerInfos.Clear();

        // 테스트용 데이터
        _playerInfos[1] = new PlayerInfo { PlayerId = 1, Nickname = "Player1", TeamId = 1 }; // 내 팀 (블루)
        _playerInfos[2] = new PlayerInfo { PlayerId = 2, Nickname = "Enemy1", TeamId = 2 }; // 적팀 (레드)
        _playerInfos[3] = new PlayerInfo { PlayerId = 3, Nickname = "Player2", TeamId = 1 }; // 내 팀 (블루)

        // 테스트용 내 Id 설정 (나중에 서버값으로 대체)
        Managers.Network.PlayerId = 1;
    }

    public void AddMessage(ChatMessageData message)
    {
        _messages.Add(message);
    }

    public List<ChatMessageData> GetMessages()
    {
        return _messages;
    }

    public PlayerInfo GetPlayerInfo(uint playerId)
    {
        _playerInfos.TryGetValue(playerId, out PlayerInfo info);
        return info ?? new PlayerInfo { PlayerId = playerId, Nickname = "Unknown", TeamId = 0 };
    }

    public uint GetMyTeamId()
    {
        return GetPlayerInfo(Managers.Network.PlayerId).TeamId; // 로컬 데이터로 대체
    }

    public void Clear()
    {
        _messages.Clear();
        _playerInfos.Clear();
    }
}

public class PlayerInfo
{
    public uint PlayerId { get; set; }
    public string Nickname { get; set; }
    public uint TeamId { get; set; }
}

public class ChatMessageData
{
    public uint PlayerId { get; set; }
    public string Nickname { get; set; }
    public string Message { get; set; }
    public uint TeamId { get; set; }
    public bool IsTeamChat { get; set; }
}