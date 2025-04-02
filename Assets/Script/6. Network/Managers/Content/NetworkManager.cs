using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager : IManager
{
    ServerSession _session = new ServerSession();
    

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }

    public void Init()
    {
        string serverIp = "13.125.73.110";
        int port = 12201;

        IPAddress ipAddr = IPAddress.Parse(serverIp);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

        Connector connector = new Connector();

        // _session은 미리 정의된 Session 클래스의 인스턴스를 반환하는 람다입니다.
        // 예: MySession : Session 클래스 상속
        connector.Connect(endPoint,
            () => { return _session; },  // _session은 Session 타입이어야 함
            1);
    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = Managers.Packet.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }


    public void Clear()
    {
    }
}