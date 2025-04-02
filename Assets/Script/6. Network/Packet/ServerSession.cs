using Game;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.XR;

public class ServerSession : PacketSession
{
    public void Send(IMessage packet)
    {
        /*
            다음과 같은 방식으로 코드를 변환
            Game.SC_CreateMyCharacter -> Game.PacketID.ScCreateMyCharacter
         */

        // 패킷 이름에서 Packet ID 구하기
        string name = packet.Descriptor.Name;
        string enumName = name.Replace("_", string.Empty);

        // 대소문자 비교없이 구분
        if (!System.Enum.TryParse(typeof(PacketID), enumName, true, out object value))
        {
            Debug.LogError($"패킷 ID 매칭 실패: {name} → {enumName}");
            MyDebug.Log($"패킷 ID 매칭 실패: {name} → {enumName}");
            return;
        }

        ushort id = (ushort)(PacketID)value;


        /*
            C++에서의 구조
            typedef struct _tagPACKET_HEADER {
                UINT8 byCode;    // 패킷 코드 0x89
                UINT8 bySize;    // 패킷 사이즈
                UINT8 byType;    // 패킷 타입
            } PACKET_HEADER; 
         */

        ushort byCode = 0x89;
        ushort bySize = (ushort)packet.CalculateSize();
        ushort byType = id;

        byte[] sendBuffer = new byte[bySize + 3];
        Array.Copy(BitConverter.GetBytes(byCode), 0, sendBuffer, 0, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes(bySize), 0, sendBuffer, 1, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes(byType), 0, sendBuffer, 2, sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 3, bySize);

        // 전송
        Send(new ArraySegment<byte>(sendBuffer));
    }
    
    public override void OnConnected(EndPoint endPoint) 
    {
        Debug.Log($"OnConnected : {endPoint}"); 
        MyDebug.Log($"OnConnected : {endPoint}");

        Managers.Packet.CustomHandler = (s, m, i) =>
        {
            PacketQueue.Instance.Push(i, m);
        };
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        MyDebug.Log($"OnDisconnected : {endPoint}");
        
    }

    public override void OnRecvPacket(ushort id, ArraySegment<byte> buffer)
    {
        Managers.Packet.OnRecvPacket(this, id, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        //Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}