using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyStateGenerator : MonoBehaviour
{

    [Header("더미 플레이어 수")]
    [SerializeField] private int dummyCount = 10;

    [Header("생성 위치 범위")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(30f, 0f, 30f);

    [Header("상태 갱신 주기")]
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1.5f;

    private readonly string[] dummyNames = new string[]
    {
        "고양이단", "펭귄조", "화난개", "웃는사람", "도적왕", "작은거인", "눈물의레인", "토끼점퍼", "불타는곰", "미소천사"
    };

    private class DummyState
    {
        public PlayerStateData state;
        public Vector3 targetPosition;
        public float waitTimer = 0f;
    }

    private Dictionary<string, DummyState> dummyPlayers = new();

    private void Start()
    {
        StartCoroutine(SimulateDummies());
    }

    private IEnumerator SimulateDummies()
    {
        PlayerManager playerManager = Managers.GameSceneManager.PlayerManager;
        while (true)
        {
            for (int i = 0; i < dummyCount; i++)
            {
                string id = $"Dummy_{i}";

                if (!dummyPlayers.ContainsKey(id))
                {
                    var data = new DummyState
                    {
                        state = GenerateNewDummyData(i),
                        targetPosition = GetRandomSpawnPosition()
                    };
                    dummyPlayers[id] = data;
                }

                SimulateMovement(dummyPlayers[id]);
                playerManager.OnReceivePlayerState(dummyPlayers[id].state);
            }

            // 퇴장 시뮬레이션 (선택사항)
            if (Random.value < 0.02f)
            {
                int idx = Random.Range(0, dummyCount);
                string leaveId = $"Dummy_{idx}";
                if (dummyPlayers.TryGetValue(leaveId, out var dummy))
                {
                    playerManager.OnRemotePlayerDisconnected(leaveId);
                    dummyPlayers.Remove(leaveId);
                }
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void SimulateMovement(DummyState dummy)
    {
        var data = dummy.state;

        if (dummy.waitTimer > 0)
        {
            dummy.waitTimer -= updateInterval;
            data.moveInput = Vector3.zero;
            return;
        }

        Vector3 direction = (dummy.targetPosition - data.position);
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);
        float distance = flatDir.magnitude;

        if (distance < 0.5f)
        {
            dummy.waitTimer = waitTime;
            dummy.targetPosition = GetRandomSpawnPosition();
            data.moveInput = Vector3.zero;
        }
        else
        {
            Vector3 moveDir = flatDir.normalized;
            data.moveInput = moveDir;
            data.position += moveDir * moveSpeed * updateInterval;

            // 회전은 이동 방향을 기반으로
            if (moveDir != Vector3.zero)
            {
                data.rotationY = Quaternion.LookRotation(moveDir).eulerAngles.y;
            }
        }

        // 부가 입력
        data.isJumping = Random.value < 0.01f;
        data.isFiring = Random.value < 0.05f;
        data.rotationX = Mathf.Clamp(data.rotationX + Random.Range(-1f, 1f), -60f, 60f);
    }

    private PlayerStateData GenerateNewDummyData(int index)
    {
        string id = $"Dummy_{index}";
        uint team = (uint)Random.Range(0,1);
        string name = dummyNames[Random.Range(0, dummyNames.Length)];
        Vector3 pos = GetRandomSpawnPosition();

        return new PlayerStateData
        {
            id = id,
            name = name,
            team = team,
            weapon = (uint)Random.Range(0, 7),
            position = pos,
            moveInput = Vector3.zero,
            rotationX = 0f,
            rotationY = Random.Range(0f, 360f),
            isJumping = false,
            isFiring = false,
            hitSuccess = false,
            hitTargetId = ""
        };
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return spawnAreaCenter + new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            0,
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );
    }
}