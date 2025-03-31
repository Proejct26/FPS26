using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("팀별 프리팹")]
    [SerializeField] private GameObject redTeamPrefab;
    [SerializeField] private GameObject blueTeamPrefab;

    private Dictionary<string, RemotePlayerController> remotePlayers = new();

    public void OnReceivePlayerState(PlayerStateData data)
    {
        if (remotePlayers.TryGetValue(data.id, out var controller)) //dict에 해당 id 값이 있는지 확인. 있으면 해당 컨트롤러에 받은 데이터 삽입
        {
            controller.ApplyNetworkState(data);
        }
        else                //데이터가 없으면 pool에서 remotePlayerPrefab을 꺼내온다.
        {
            GameObject prefab = data.team == 0 ? redTeamPrefab : blueTeamPrefab;

            GameObject remoteObj = Managers.Pool.Get(prefab);
            remoteObj.transform.position = data.position;

            RemotePlayerController newController = remoteObj.GetComponentInChildren<RemotePlayerController>();
            newController.ApplyNetworkState(data);

            remotePlayers.Add(data.id, newController);
        }
    }

    public void OnRemotePlayerDisconnected(string playerId)     //플레이어 게임 종료 했을 경우
    {
        if (remotePlayers.TryGetValue(playerId, out var controller))
        {
            controller.gameObject.SetActive(false);
            remotePlayers.Remove(playerId);
        }
    }

    public void ClearAllRemotePlayers()
    {
        foreach (var player in remotePlayers.Values)
            player.gameObject.SetActive(false);

        remotePlayers.Clear();
    }
}
