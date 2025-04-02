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
    
    public uint PlayerId { get; set; } // Chat 테스트용

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }

    public void Init()
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        //IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 12201);

        Connector connector = new Connector();

        connector.Connect(endPoint,
            () => { return _session; },
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