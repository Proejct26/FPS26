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
            $"PosIndex : {createMyCharacterPacket.PosIndex}, " +
            $"Look : {createMyCharacterPacket.DirX}, {createMyCharacterPacket.DirY}, {createMyCharacterPacket.DirZ}, " +
            $"HP : {createMyCharacterPacket.MaxHP}, " +
            $"TeamID : {createMyCharacterPacket.TeamID}"
        );
 
        // crateCharacterPacket 에 있는 정보 추출 후 사용 
        Debug.Log("플레이어 생성 완료!");
        Managers.GameSceneManager.SpawnLocalPlayer((int)createMyCharacterPacket.PosIndex, (int)createMyCharacterPacket.TeamID);

        // // 서버에 다시 신호 보내기
        // CS_SEND_NICKNAME onSendNicknamePacket = new CS_SEND_NICKNAME();
        // onSendNicknamePacket.Name = "TestPlayer";
        // Managers.Network.Send(onSendNicknamePacket);
    }

    // SC_CREATE_OTHER_CHARACTER 패킷을 처리하는 함수
    public static void SC_CreateOtherCharacter(PacketSession session, IMessage packet)
    {
        SC_CREATE_OTHER_CHARACTER createOtherCharacterPacket = packet as SC_CREATE_OTHER_CHARACTER;

        // TODO: SC_CreateOtherCharacter 패킷 처리 로직을 여기에 구현

        //패킷 데이터 기반으로 상태 설정 완료
        PlayerStateData stat = new PlayerStateData()
        {
            id = createOtherCharacterPacket.PlayerId.ToString(),
            name = createOtherCharacterPacket.Name,
            team = createOtherCharacterPacket.TeamID,
            maxHp = createOtherCharacterPacket.MaxHP,
            curHp = createOtherCharacterPacket.CurHP,
            weapon = createOtherCharacterPacket.Weapon,

            kills = createOtherCharacterPacket.KdaInfo.Kill,
            deaths = createOtherCharacterPacket.KdaInfo.Death,
            assists = createOtherCharacterPacket.KdaInfo.Assist,
            isAlive = createOtherCharacterPacket.CurHP != 0,

            position = new Vector3((float)createOtherCharacterPacket.PosX, (float)createOtherCharacterPacket.PosY, (float)createOtherCharacterPacket.PosZ),
            lookInput = new Vector3(0, (float)createOtherCharacterPacket.RotateAxisY, 0),
            rotationX = (float)createOtherCharacterPacket.RotateAxisX,
            rotationY = (float)createOtherCharacterPacket.RotateAxisY
        };

        //PlayerManager에 삽입하기
        Managers.GameSceneManager.PlayerManager.OnReceivePlayerState(stat);
    }

    // SC_GRENADE_EXPLOSITION_POS 패킷을 처리하는 함수
    public static void SC_GrenadeExplositionPos(PacketSession session, IMessage packet)
    {
        SC_GRENADE_EXPLOSITION_POS grenadeExplositionPosPacket = packet as SC_GRENADE_EXPLOSITION_POS;

        // TODO: SC_GrenadeExplositionPos 패킷 처리 로직을 여기에 구현
    }

    // SC_ITEM_PICK_FAIL 패킷을 처리하는 함수
    public static void SC_ItemPickFail(PacketSession session, IMessage packet)
    {
        SC_ITEM_PICK_FAIL itemPickFailPacket = packet as SC_ITEM_PICK_FAIL;

        // TODO: SC_ItemPickFail 패킷 처리 로직을 여기에 구현
    }

    // SC_ITEM_PICK_SUCCESS 패킷을 처리하는 함수
    public static void SC_ItemPickSuccess(PacketSession session, IMessage packet)
    {
        SC_ITEM_PICK_SUCCESS itemPickSuccessPacket = packet as SC_ITEM_PICK_SUCCESS;

        // TODO: SC_ItemPickSuccess 패킷 처리 로직을 여기에 구현
    }

    // SC_ITEM_SPAWNED 패킷을 처리하는 함수
    public static void SC_ItemSpawned(PacketSession session, IMessage packet)
    {
        SC_ITEM_SPAWNED itemSpawnedPacket = packet as SC_ITEM_SPAWNED;

        // TODO: SC_ItemSpawned 패킷 처리 로직을 여기에 구현
    }

    // SC_KEY_INPUT 패킷을 처리하는 함수
    public static void SC_KeyInput(PacketSession session, IMessage packet)
    {
        SC_KEY_INPUT keyInputPacket = packet as SC_KEY_INPUT;

        // TODO: SC_KeyInput 패킷 처리 로직을 여기에 구현

    }

    // SC_ON_ACCEPT 패킷을 처리하는 함수
    public static void SC_OnAccept(PacketSession session, IMessage packet)
    {
        SC_ON_ACCEPT onAcceptPacket = packet as SC_ON_ACCEPT;

        // TODO: SC_OnAccept 패킷 처리 로직을 여기에 구현
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
