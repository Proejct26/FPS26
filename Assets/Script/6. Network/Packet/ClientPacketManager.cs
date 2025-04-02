using Game;
using Google.Protobuf;
using ServerCore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PacketManager : IManager
{
    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Init()
    {
        Register();
    }

    public void Clear()
    {
    }

    // 패킷 수신 및 핸들러를 등록하는 메서드입니다.
    public void Register()
    {
        _onRecv.Add((ushort)Game.PacketID.ScAttack, MakePacket<SC_ATTACK>);
        _handler.Add((ushort)Game.PacketID.ScAttack, PacketHandler.SC_Attack);

        _onRecv.Add((ushort)Game.PacketID.ScChangeWeapon, MakePacket<SC_CHANGE_WEAPON>);
        _handler.Add((ushort)Game.PacketID.ScChangeWeapon, PacketHandler.SC_ChangeWeapon);

        _onRecv.Add((ushort)Game.PacketID.ScCharacterDown, MakePacket<SC_CHARACTER_DOWN>);
        _handler.Add((ushort)Game.PacketID.ScCharacterDown, PacketHandler.SC_CharacterDown);

        _onRecv.Add((ushort)Game.PacketID.ScCharacterKillLog, MakePacket<SC_CHARACTER_KILL_LOG>);
        _handler.Add((ushort)Game.PacketID.ScCharacterKillLog, PacketHandler.SC_CharacterKillLog);

        _onRecv.Add((ushort)Game.PacketID.ScCreateMyCharacter, MakePacket<SC_CREATE_MY_CHARACTER>);
        _handler.Add((ushort)Game.PacketID.ScCreateMyCharacter, PacketHandler.SC_CreateMyCharacter);

        _onRecv.Add((ushort)Game.PacketID.ScCreateOtherCharacter, MakePacket<SC_CREATE_OTHER_CHARACTER>);
        _handler.Add((ushort)Game.PacketID.ScCreateOtherCharacter, PacketHandler.SC_CreateOtherCharacter);

        _onRecv.Add((ushort)Game.PacketID.ScItemPickFail, MakePacket<SC_ITEM_PICK_FAIL>);
        _handler.Add((ushort)Game.PacketID.ScItemPickFail, PacketHandler.SC_ItemPickFail);

        _onRecv.Add((ushort)Game.PacketID.ScItemPickSuccess, MakePacket<SC_ITEM_PICK_SUCCESS>);
        _handler.Add((ushort)Game.PacketID.ScItemPickSuccess, PacketHandler.SC_ItemPickSuccess);

        _onRecv.Add((ushort)Game.PacketID.ScItemSpawned, MakePacket<SC_ITEM_SPAWNED>);
        _handler.Add((ushort)Game.PacketID.ScItemSpawned, PacketHandler.SC_ItemSpawned);

        _onRecv.Add((ushort)Game.PacketID.ScKeyInput, MakePacket<SC_KEY_INPUT>);
        _handler.Add((ushort)Game.PacketID.ScKeyInput, PacketHandler.SC_KeyInput);

        _onRecv.Add((ushort)Game.PacketID.ScOnAccept, MakePacket<SC_ON_ACCEPT>);
        _handler.Add((ushort)Game.PacketID.ScOnAccept, PacketHandler.SC_OnAccept);

        _onRecv.Add((ushort)Game.PacketID.ScPosInterpolation, MakePacket<SC_POS_INTERPOLATION>);
        _handler.Add((ushort)Game.PacketID.ScPosInterpolation, PacketHandler.SC_PosInterpolation);

        _onRecv.Add((ushort)Game.PacketID.ScSendMessageAll, MakePacket<SC_SEND_MESSAGE_ALL>);
        _handler.Add((ushort)Game.PacketID.ScSendMessageAll, PacketHandler.SC_SendMessageAll);

        _onRecv.Add((ushort)Game.PacketID.ScSendMessageTeam, MakePacket<SC_SEND_MESSAGE_TEAM>);
        _handler.Add((ushort)Game.PacketID.ScSendMessageTeam, PacketHandler.SC_SendMessageTeam); 
 
        _onRecv.Add((ushort)Game.PacketID.ScShotHit, MakePacket<SC_SHOT_HIT>);
        _handler.Add((ushort)Game.PacketID.ScShotHit, PacketHandler.SC_ShotHit);
    } 

    public void OnRecvPacket(PacketSession session, ushort id, ArraySegment<byte> buffer)
    {
        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
         if (_onRecv.TryGetValue(id, out action))
                action.Invoke(session, buffer, id);
  
            
        
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();

        // Protobuf 메시지를 buffer 전체에서 파싱
        pkt.MergeFrom(buffer.Array, buffer.Offset, buffer.Count);

        if (CustomHandler != null)
        {
            CustomHandler.Invoke(session, pkt, id);
        }
        else
        {
            Action<PacketSession, IMessage> action = null;
             if (_handler.TryGetValue(id, out action))
                JobQueue.Push(()=>action.Invoke(session, pkt));   

        }
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
    {
        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            return action;
        return null;
    }
}