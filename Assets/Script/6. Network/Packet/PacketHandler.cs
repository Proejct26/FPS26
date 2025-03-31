using Game;
using Google.Protobuf;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    // SC_ATTACK 패킷을 처리하는 함수
    public static void SC_Attack(PacketSession session, IMessage packet)
    {
        SC_ATTACK attackPacket = packet as SC_ATTACK;

        // TODO: SC_Attack 패킷 처리 로직을 여기에 구현
    }

    // SC_CHANGE_WEAPON 패킷을 처리하는 함수
    public static void SC_ChangeWeapon(PacketSession session, IMessage packet)
    {
        SC_CHANGE_WEAPON changeWeaponPacket = packet as SC_CHANGE_WEAPON;

        // TODO: SC_ChangeWeapon 패킷 처리 로직을 여기에 구현
    }

    // SC_CHARACTER_DOWN 패킷을 처리하는 함수
    public static void SC_CharacterDown(PacketSession session, IMessage packet)
    {
        SC_CHARACTER_DOWN characterDownPacket = packet as SC_CHARACTER_DOWN;

        // TODO: SC_CharacterDown 패킷 처리 로직을 여기에 구현
    }

    // SC_CHARACTER_KILL_LOG 패킷을 처리하는 함수
    public static void SC_CharacterKillLog(PacketSession session, IMessage packet)
    {
        SC_CHARACTER_KILL_LOG characterKillLogPacket = packet as SC_CHARACTER_KILL_LOG;

        // TODO: SC_CharacterKillLog 패킷 처리 로직을 여기에 구현
    }

    // SC_CREATE_MY_CHARACTER 패킷을 처리하는 함수
    public static void SC_CreateMyCharacter(PacketSession session, IMessage packet)
    {
        SC_CREATE_MY_CHARACTER createMyCharacterPacket = packet as SC_CREATE_MY_CHARACTER;

        // TODO: SC_CreateMyCharacter 패킷 처리 로직을 여기에 구현
        Debug.Log(
            $"ID : {createMyCharacterPacket.PlayerId}, " +
            $"Pos : {createMyCharacterPacket.PosX}, {createMyCharacterPacket.PosY}, {createMyCharacterPacket.PosZ}, " +
            $"Look : {createMyCharacterPacket.DirX}, {createMyCharacterPacket.DirY}, {createMyCharacterPacket.DirZ}" +
            $"HP : {createMyCharacterPacket.MaxHP}"
        );

        // crateCharacterPacket 에 있는 정보 추출 후 사용
        Debug.Log("플레이어 생성 완료!");

        // 서버에 다시 신호 보내기
        CS_ON_ACCEPT onAcceptPacket = new CS_ON_ACCEPT();
        onAcceptPacket.Name = "TestPlayer";
        Managers.Network.Send(onAcceptPacket);
    }

    // SC_CREATE_OTHER_CHARACTER 패킷을 처리하는 함수
    public static void SC_CreateOtherCharacter(PacketSession session, IMessage packet)
    {
        SC_CREATE_OTHER_CHARACTER createOtherCharacterPacket = packet as SC_CREATE_OTHER_CHARACTER;

        // TODO: SC_CreateOtherCharacter 패킷 처리 로직을 여기에 구현
    }

    // SC_GRENADEEXPLOSITIONPOS 패킷을 처리하는 함수
    public static void SC_Grenadeexplositionpos(PacketSession session, IMessage packet)
    {
        SC_GRENADEEXPLOSITIONPOS grenadeexplositionposPacket = packet as SC_GRENADEEXPLOSITIONPOS;

        // TODO: SC_Grenadeexplositionpos 패킷 처리 로직을 여기에 구현
    }

    // SC_KEY_INPUT 패킷을 처리하는 함수
    public static void SC_KeyInput(PacketSession session, IMessage packet)
    {
        SC_KEY_INPUT keyInputPacket = packet as SC_KEY_INPUT;

        // TODO: SC_KeyInput 패킷 처리 로직을 여기에 구현
    }

    // SC_POS_INTERPOLATION 패킷을 처리하는 함수
    public static void SC_PosInterpolation(PacketSession session, IMessage packet)
    {
        SC_POS_INTERPOLATION posInterpolationPacket = packet as SC_POS_INTERPOLATION;

        // TODO: SC_PosInterpolation 패킷 처리 로직을 여기에 구현
    }

    // SC_SHOT_HIT 패킷을 처리하는 함수
    public static void SC_ShotHit(PacketSession session, IMessage packet)
    {
        SC_SHOT_HIT shotHitPacket = packet as SC_SHOT_HIT;

        // TODO: SC_ShotHit 패킷 처리 로직을 여기에 구현
    }

    // SC_THROW_GRENADE 패킷을 처리하는 함수
    public static void SC_ThrowGrenade(PacketSession session, IMessage packet)
    {
        SC_THROW_GRENADE throwGrenadePacket = packet as SC_THROW_GRENADE;

        // TODO: SC_ThrowGrenade 패킷 처리 로직을 여기에 구현
    }
}