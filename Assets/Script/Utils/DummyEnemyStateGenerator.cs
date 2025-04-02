using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class DummyRemotePlayersSimulator : MonoBehaviour
{
    [SerializeField] private int dummyCount = 5;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float moveSpeed = 2f;

    private class Dummy
    {
        public string id;
        public int team;
        public Vector3 position;
        public float yaw;
        public float pitch;
        public Vector3 target;
    }

    private Dictionary<string, Dummy> dummies = new();

    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null; // 한 프레임 대기
        StartCoroutine(SimulateDummies());
    }


    private IEnumerator SimulateDummies()
    {
        // Red team (team 0)
        for (int i = 0; i < dummyCount; i++)
        {
            int team = 0;
            string id = $"{i}";

            var dummy = new Dummy
            {
                id = id,
                team = team,
                position = GetRandomSpawnPosition(),
                yaw = Random.Range(0f, 360f),
                pitch = 0f,
                target = GetRandomSpawnPosition()
            };

            dummies[id] = dummy;
            SendCreatePacket(dummy);
        }

        // Blue team (team 1)
        for (int i = dummyCount; i < dummyCount+dummyCount; i++)
        {
            int team = 1;
            string id = $"{i}";

            var dummy = new Dummy
            {
                id = id,
                team = team,
                position = GetRandomSpawnPosition(),
                yaw = Random.Range(0f, 360f),
                pitch = 0f,
                target = GetRandomSpawnPosition()
            };

            dummies[id] = dummy;
            SendCreatePacket(dummy);
        }

        while (true)
        {
            foreach (var dummy in dummies.Values)
                SimulateMoveAndSend(dummy);

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void SendCreatePacket(Dummy dummy)
    {
        PlayerStateData data = new PlayerStateData
        {
            id = dummy.id,
            name = dummy.id,
            team = (uint)dummy.team,
            position = dummy.position,
            rotationY = dummy.yaw,
            rotationX = dummy.pitch,
            weapon = 0,
            maxHp = 100,
            curHp = 100,
            isAlive = true
        };

        Managers.GameSceneManager.PlayerManager.OnReceivePlayerState(data);
    }

    private void SimulateMoveAndSend(Dummy dummy)
    {

        Vector3 dir = dummy.target - dummy.position;
        Vector3 flatDir = new Vector3(dir.x, 0, dir.z);

        if (flatDir.magnitude < 0.5f)
        {
            dummy.target = GetRandomSpawnPosition();
        }
        else
        {
            Vector3 moveDir = flatDir.normalized;
            dummy.position += moveDir * moveSpeed * updateInterval;
            dummy.yaw = Quaternion.LookRotation(moveDir).eulerAngles.y;

            uint playerId = uint.Parse(dummy.id);

            SC_KEY_INPUT inputPacket = new SC_KEY_INPUT
            {
                PlayerId = playerId,
                KeyW = moveDir.z > 0 ? 1u : 0u,
                KeyS = moveDir.z < 0 ? 1u : 0u,
                KeyA = moveDir.x < 0 ? 1u : 0u,
                KeyD = moveDir.x > 0 ? 1u : 0u,
                RotateAxisX = (uint)dummy.pitch,
                RotateAxisY = (uint)dummy.yaw,
                Jump = Random.value < 0.01f ? 1u : 0u
            };

            PacketHandler.SC_KeyInput(null, inputPacket);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(5f, 25f);
        float z = Random.Range(5f, 25f);
        //float y = 5f; // 바닥보다 약간 위 (중력과 충돌처리 가능하게)

        return new Vector3(x, 0.2f, z); // fallback
    }
}
