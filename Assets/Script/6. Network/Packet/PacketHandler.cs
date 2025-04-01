using Game;
using Google.Protobuf;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using static UnityEditor.PlayerSettings;

class PacketHandler
{ 
    // SC_ATTACK 패킷을 처리하는 함수
    public static void SC_Attack(PacketSession session, IMessage packet)
    {
        SC_ATTACK attackPacket = packet as SC_ATTACK;  
         

        // WeaponBaseController.SpawnMuzzleFlash(new Vector3(attackPacket.PosX, attackPacket.PosY, attackPacket.PosZ), 
        //     new Vector3(attackPacket.NormalX, attackPacket.NormalY, attackPacket.NormalZ));

        WeaponBaseController.SpawnHitEffect(new Vector3(attackPacket.PosX.ConvertToFloat(), attackPacket.PosY.ConvertToFloat(), attackPacket.PosZ.ConvertToFloat()), 
            new Vector3(attackPacket.NormalX.ConvertToFloat(), attackPacket.NormalY.ConvertToFloat(), attackPacket.NormalZ.ConvertToFloat()));  
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
            $"HP : {createMyCharacterPacket.MaxHP}, " +
            $"TeamID : {createMyCharacterPacket.TeamID}"
        );
 
        // crateCharacterPacket 에 있는 정보 추출 후 사용  
        MyDebug.Log("플레이어 생성 완료!"); 
        Managers.GameSceneManager.SpawnLocalPlayer((int)createMyCharacterPacket.PosIndex, (int)createMyCharacterPacket.TeamID);
    }

    // SC_CREATE_OTHER_CHARACTER 패킷을 처리하는 함수
    public static void SC_CreateOtherCharacter(PacketSession session, IMessage packet)
    {
        SC_CREATE_OTHER_CHARACTER createOtherCharacterPacket = packet as SC_CREATE_OTHER_CHARACTER;

        // TODO: SC_CreateOtherCharacter 패킷 처리 로직을 여기에 구현

        Transform spawnTf = Managers.GameSceneManager.SpawnData.GetSpawnPosition((int)createOtherCharacterPacket.TeamID, (int)createOtherCharacterPacket.PosIndex);

        //패킷 데이터 기반으로 상태 설정 완료
        PlayerStateData stat = new PlayerStateData()
        {
            id = createOtherCharacterPacket.PlayerId.ToString(),
            name = createOtherCharacterPacket.Name,
            team = createOtherCharacterPacket.TeamID,
            maxHp = createOtherCharacterPacket.MaxHP,
            curHp = createOtherCharacterPacket.CurHP,
            weapon = (int)createOtherCharacterPacket.Weapon,

            kills = createOtherCharacterPacket.KdaInfo.Kill,
            deaths = createOtherCharacterPacket.KdaInfo.Death,
            assists = createOtherCharacterPacket.KdaInfo.Assist,
            isAlive = createOtherCharacterPacket.CurHP != 0,

            // 이쪽 값들은 없습니다. 새로 작업해주셔야합니다.
            position = spawnTf.position,
            rotationX = spawnTf.eulerAngles.x,
            rotationY = spawnTf.eulerAngles.y,  
        };

        //PlayerManager에 삽입하기
        Managers.GameSceneManager.PlayerManager.OnReceivePlayerState(stat);
        MyDebug.Log("리무트 플레이어 생성 완료!");  
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
        if (Managers.GameSceneManager.PlayerManager.TryGetPlayer(keyInputPacket.PlayerId, out var controller))
        {
            Vector3 moveInput = Vector3.zero;
            if (keyInputPacket.KeyW == 1) moveInput.z += 1;
            if (keyInputPacket.KeyS == 1) moveInput.z -= 1;
            if (keyInputPacket.KeyA == 1) moveInput.x -= 1;
            if (keyInputPacket.KeyD == 1) moveInput.x += 1;

            //결과 적용
            controller.SetNetworkInput(
                moveInput,
                keyInputPacket.RotateAxisX,
                keyInputPacket.RotateAxisY,
                keyInputPacket.Jump == 1
            );
        }
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
        if(Managers.Player.TryGetComponent(out LocalPlayerController local))
        {
            Vector3 pos = new Vector3(posInterpolationPacket.PosX, posInterpolationPacket.PosY, posInterpolationPacket.PosZ);
            local.SetNetworkPosition(pos);
        }
    }

    // SC_SHOT_HIT 패킷을 처리하는 함수
    public static void SC_ShotHit(PacketSession session, IMessage packet)
    {
        SC_SHOT_HIT shotHitPacket = packet as SC_SHOT_HIT;
    
        if(Managers.GameSceneManager.PlayerManager.TryGetPlayer(shotHitPacket.PlayerId, out var controller))
        {
            if (controller.TryGetComponent(out PlayerStatHandler playerStatHandler))
                playerStatHandler.SetHealth(shotHitPacket.Hp);  
             
        }
    }

    // SC_THROW_GRENADE 패킷을 처리하는 함수
    public static void SC_ThrowGrenade(PacketSession session, IMessage packet) 
    {
        SC_THROW_GRENADE throwGrenadePacket = packet as SC_THROW_GRENADE;

        // TODO: SC_ThrowGrenade 패킷 처리 로직을 여기에 구현
    }
}
