using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject remotePlayerPrefab;

    private Dictionary<string, RemotePlayerController> remotePlayers = new();

    public void OnReceivePlayerState(PlayerStateData data)
    {
        if (remotePlayers.TryGetValue(data.id, out var controller)) //dict에 해당 id 값이 있는지 확인. 있으면 해당 컨트롤러에 받은 데이터 삽입
        {
            controller.ApplyNetworkState(data);
        }
        else                //데이터가 없으면 pool에서 remotePlayerPrefab을 꺼내온다.
        {
            GameObject remoteObj = Managers.Pool.Get(remotePlayerPrefab);
            remoteObj.transform.position = data.position;

            RemotePlayerController newController = remoteObj.GetComponent<RemotePlayerController>();
            newController.ApplyNetworkState(data);

            remotePlayers.Add(data.id, newController);
        }
    }

    public void OnRemotePlayerDisconnected(string playerId)     //플레이어 게임 종료 했을 경우
    {
        if (remotePlayers.TryGetValue(playerId, out var controller))
        {
            controller.gameObject.SetActive(false); // Or controller.ReturnToPool(); if using IPoolable
            remotePlayers.Remove(playerId);
        }
    }

    public void ClearAllRemotePlayers()
    {
        foreach (var player in remotePlayers.Values)
        {
            player.gameObject.SetActive(false);
        }
        remotePlayers.Clear();
    }
}
