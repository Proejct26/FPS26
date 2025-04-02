using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatDataManager : IManager
{
    private List<ChatMessageData> _messages = new List<ChatMessageData>();
    private Dictionary<uint, PlayerInfo> _playerInfos = new Dictionary<uint, PlayerInfo>();

    public event Action<ChatMessageData> OnMessageAdded;
    public void Init()
    {
        _messages.Clear();
        _playerInfos.Clear();
    }

    public void AddMessage(ChatMessageData message)
    {
        _messages.Add(message);
        OnMessageAdded?.Invoke(message);
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
        return GetPlayerInfo((uint)Managers.GameSceneManager.PlayerId).TeamId; // 로컬 데이터로 대체
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