using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyStateGenerator : MonoBehaviour
{
    [SerializeField] private RemotePlayerController remoteEnemy;
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;

    private int currentPoint = 0;
    private float rotationY = 0f;
    private float rotationX = 0f;

    private void Start()
    {
        if (remoteEnemy == null)
            remoteEnemy = GetComponent<RemotePlayerController>();

        StartCoroutine(SendFakeStateToRemote());
    }

    private IEnumerator SendFakeStateToRemote()
    {
        while (true)
        {
            Vector3 targetPos = pathPoints[currentPoint].position;
            Vector3 currentPos = remoteEnemy.transform.position;
            Vector3 direction = (targetPos - currentPos).normalized;

            // 시야 각도 조정 (랜덤하게 흔들림)
            rotationY += Random.Range(-3f, 3f);
            rotationX = Mathf.Clamp(rotationX + Random.Range(-1f, 1f), -60f, 60f);

            // 이동
            Vector3 nextPos = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
            bool reached = Vector3.Distance(currentPos, targetPos) < 0.2f;

            // 랜덤 점프 / 사격 입력
            bool isJumping = Random.value < 0.02f;  // 2% 확률 점프
            bool isFiring = Random.value < 0.1f;    // 10% 확률 사격
            Vector3 moveInput = reached || Random.value < 0.05f ? Vector3.zero : direction; // 가끔 멈추기

            PlayerStateData dummyData = new PlayerStateData
            {
                position = nextPos,
                moveInput = moveInput,
                rotationX = rotationX,
                rotationY = rotationY,
                isJumping = isJumping,
                isFiring = isFiring,
                hitSuccess = false,
                hitTargetId = ""
            };

            remoteEnemy.ApplyNetworkState(dummyData);

            if (reached)
            {
                currentPoint = (currentPoint + 1) % pathPoints.Length;
                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }
    }

}
