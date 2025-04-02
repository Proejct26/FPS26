using Game;
using Google.Protobuf;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

class PacketHandler
{ 
    // SC_ATTACK 패킷을 처리하는 함수
    public static void SC_Attack(PacketSession session, IMessage packet)
    {
        SC_ATTACK attackPacket = packet as SC_ATTACK;  
         
        if(Managers.GameSceneManager.PlayerManager.TryGetPlayer(attackPacket.PlayerId, out var controller))
        {
            if (controller.DummyGunController != null)
                controller.DummyGunController.Fire(); 
        }
        // WeaponBaseController.SpawnMuzzleFlash(new Vector3(attackPacket.PosX, attackPacket.PosY, attackPacket.PosZ), 
        //     new Vector3(attackPacket.NormalX, attackPacket.NormalY, attackPacket.NormalZ));

        WeaponBaseController.SpawnHitEffect(new Vector3(attackPacket.PosX, attackPacket.PosY, attackPacket.PosZ), 
            new Vector3(attackPacket.NormalX, attackPacket.NormalY, attackPacket.NormalZ), attackPacket.HittedTargetId != 99999);     
        // TODO: SC_Attack 패킷 처리 로직을 여기에 구현
    } 

    // SC_CHANGE_WEAPON 패킷을 처리하는 함수
    public static void SC_ChangeWeapon(PacketSession session, IMessage packet)
    {
        SC_CHANGE_WEAPON changeWeaponPacket = packet as SC_CHANGE_WEAPON;
        if(Managers.GameSceneManager.PlayerManager.TryGetPlayer(changeWeaponPacket.PlayerId, out var controller))
        {
            controller.EquipWeapon((int)changeWeaponPacket.Weapon);
        } 

        // TODO: SC_ChangeWeapon 패킷 처리 로직을 여기에 구현
    }


    public static void SC_CharacterDown(PacketSession session, IMessage packet)
    {
        SC_CHARACTER_DOWN characterDownPacket = packet as SC_CHARACTER_DOWN;
        MyDebug.Log($"[{characterDownPacket.PlayerId}] 플레이어가 죽었습니다.");  

        if(Managers.GameSceneManager.PlayerManager.TryGetPlayer(characterDownPacket.PlayerId, out var controller))
        {
            PlayerStatHandler playerStatHandler = controller.GetComponentInChildren<PlayerStatHandler>();
            if (playerStatHandler != null)
            {
                playerStatHandler.HandleDeath();
            }
        }
        
        if (Managers.GameSceneManager.PlayerId == characterDownPacket.PlayerId)
        {
            Managers.Player.GetComponentInChildren<PlayerStatHandler>().HandleDeath(); 
        }   
    }


    public static void SC_CharacterKillLog(PacketSession session, IMessage packet)
    {
        SC_CHARACTER_KILL_LOG characterKillLogPacket = packet as SC_CHARACTER_KILL_LOG;
 
        
    }


    public static void SC_CreateMyCharacter(PacketSession session, IMessage packet)
    {
        SC_CREATE_MY_CHARACTER createMyCharacterPacket = packet as SC_CREATE_MY_CHARACTER;


        Debug.Log(
            $"ID : {createMyCharacterPacket.PlayerId}, " +
            $"PosIndex : {createMyCharacterPacket.PosIndex}, " +
            $"HP : {createMyCharacterPacket.MaxHP}, " +
            $"TeamID : {createMyCharacterPacket.TeamID}"
        );
 
        // crateCharacterPacket 에 있는 정보 추출 후 사용  
        MyDebug.Log("플레이어 생성 완료!"); 
        Managers.GameSceneManager.SpawnLocalPlayer((int)createMyCharacterPacket.PosIndex, (int)createMyCharacterPacket.TeamID, (int)createMyCharacterPacket.PlayerId);
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
            networkId = createOtherCharacterPacket.PlayerId,
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
        Debug.Log("Debug : 리무트 플레이어 생성 완료!");  
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

        // 디버깅: 패킷 데이터 로깅
        Debug.Log($"SC_KEY_INPUT 패킷 수신: PlayerId={keyInputPacket.PlayerId}, " +
                  $"RotateAxisX={keyInputPacket.RotateAxisX}, RotateAxisY={keyInputPacket.RotateAxisY}, " +
                  $"Jump={keyInputPacket.Jump}");

        // TODO: SC_KeyInput 패킷 처리 로직을 여기에 구현
        if (Managers.GameSceneManager.PlayerManager.TryGetPlayer(keyInputPacket.PlayerId, out var controller))
        {
            Vector3 moveInput = Vector3.zero;
            if (keyInputPacket.KeyW) moveInput.z += 1;
            if (keyInputPacket.KeyS) moveInput.z -= 1;
            if (keyInputPacket.KeyA) moveInput.x -= 1; 
            if (keyInputPacket.KeyD) moveInput.x += 1;

            Vector3 lookDir = new Vector3(
                keyInputPacket.NormalX,
                keyInputPacket.NormalY,
                keyInputPacket.NormalZ
            );

            //결과 적용
            controller.SetNetworkInput(
                moveInput,
                keyInputPacket.RotateAxisX,
                keyInputPacket.RotateAxisY,
                keyInputPacket.Jump == 1,
                lookDir
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

        if(Managers.GameSceneManager.PlayerManager.TryGetPlayer(posInterpolationPacket.PlayerId, out var controller))
        {
            Vector3 pos = new Vector3(posInterpolationPacket.PosX, posInterpolationPacket.PosY, posInterpolationPacket.PosZ);
            controller.SetNetworkPosition(pos); 
        }
    }


    // SC_SEND_MESSAGE 패킷을 처리하는 함
        // SC_SEND_MESSAGE_ALL 패킷을 처리하는 함수
    public static void SC_SendMessageAll(PacketSession session, IMessage packet)
    {
        SC_SEND_MESSAGE_ALL sendMessageAllPacket = packet as SC_SEND_MESSAGE_ALL;

        // ChatMessageData chatData = new ChatMessageData()
        // {
        //     PlayerId = sendMessageAllPacket.PlayerId,
        //     Message = sendMessageAllPacket.Message,

        //     Nickname = sendMessageAllPacket.Nickname,
        //     TeamId = sendMessageAllPacket.TeamId,
        //     IsTeamChat = false;
        // };


        // TODO: SC_SendMessageAll 패킷 처리 로직을 여기에 구현
        string unityString = sendMessageAllPacket.Message;
        Debug.Log($"SC_SendMessageAll : {unityString}");

    }

    // SC_SEND_MESSAGE_TEAM 패킷을 처리하는 함수
    public static void SC_SendMessageTeam(PacketSession session, IMessage packet)
    {
        SC_SEND_MESSAGE_TEAM sendMessageTeamPacket = packet as SC_SEND_MESSAGE_TEAM;

        // TODO: SC_SendMessageTeam 패킷 처리 로직을 여기에 구현
        string unityString = sendMessageTeamPacket.Message;

        Debug.Log($"SC_SendMessageTeam : {unityString}");

    }
    
    // SC_SHOT_HIT 패킷을 처리하는 함수
    public static void SC_ShotHit(PacketSession session, IMessage packet)
    {
        SC_SHOT_HIT shotHitPacket = packet as SC_SHOT_HIT;
        MyDebug.Log($"SC_SHOT_HIT 패킷 수신: PlayerId={shotHitPacket.PlayerId}, Hp={shotHitPacket.Hp}"); 
        if(Managers.GameSceneManager.PlayerManager.TryGetPlayer(shotHitPacket.PlayerId, out var controller))
        {
            PlayerStatHandler playerStatHandler = controller.GetComponentInChildren<PlayerStatHandler>();
            if (playerStatHandler != null)
            {
                playerStatHandler.SetHealth(shotHitPacket.Hp);    
            }
              
        }

        if (Managers.GameSceneManager.PlayerId == shotHitPacket.PlayerId)
        {
            Managers.Player.GetComponentInChildren<PlayerStatHandler>().SetHealth(shotHitPacket.Hp); 
        }
    }
}
 