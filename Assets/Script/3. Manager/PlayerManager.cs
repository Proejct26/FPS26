using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : IManager
{
    [Header("팀별 프리팹")]
    [SerializeField] public GameObject redTeamPrefab;
    [SerializeField] public GameObject blueTeamPrefab;

    private Dictionary<uint, RemotePlayerController> remotePlayers = new();
  
     public void Init()
    {
        remotePlayers.Clear();  // 초기화 시 클리어
        Debug.Log("[PlayerManager] Init 호출됨");
    }

    public void Clear()
    {
        ClearAllRemotePlayers();  // 모든 원격 플레이어 비활성화 및 딕셔너리 제거
        Debug.Log("[PlayerManager] Clear 호출됨");
    }

    public void OnReceivePlayerState(PlayerStateData data)
    {
        if (remotePlayers.TryGetValue(data.networkId, out var controller)) //dict에 해당 id 값이 있는지 확인. 있으면 해당 컨트롤러에 받은 데이터 삽입
        {
            controller.ApplyNetworkState(data);
        }
        else                                                        //데이터가 없으면 pool에서 remotePlayerPrefab을 꺼내온다.
        {
            GameObject prefab = data.team == 0 ? redTeamPrefab : blueTeamPrefab;

            GameObject remoteObj = Managers.Pool.Get(prefab);
            if (remoteObj == null)
            {
                Debug.LogWarning($"[PlayerManager] Prefab 가져오기 실패: {data.id}"); 
                return;
            }

            remoteObj.transform.position = data.position;
            remoteObj.transform.rotation = Quaternion.Euler(data.rotationX, data.rotationY, 0);

            RemotePlayerController newController = remoteObj.GetComponentInChildren<RemotePlayerController>();
            newController.ApplyNetworkState(data);  

            remotePlayers.Add(data.networkId, newController); 
        }
    }

    public void OnRemotePlayerDisconnected(uint networkId)     //플레이어 게임 종료 했을 경우
    {
        if (remotePlayers.TryGetValue(networkId, out var controller))
        {
            controller.nameTag?.gameObject.SetActive(false);
            controller.gameObject.SetActive(false);
            remotePlayers.Remove(networkId);
        }
    }

    public void ClearAllRemotePlayers()
    {
        foreach (var player in remotePlayers.Values)
            player.gameObject.SetActive(false);

        remotePlayers.Clear();
    }

    public bool TryGetPlayer(uint id, out RemotePlayerController controller)
    {
        return remotePlayers.TryGetValue(id, out controller);
    } 
}
