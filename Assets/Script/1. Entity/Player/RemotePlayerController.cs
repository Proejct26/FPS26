using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{
    [SerializeField] private Transform _headPivot;

    public void ApplyNetworkState(PlayerStateData data)
    {
        // 위치 보간
        transform.position = Vector3.Lerp(transform.position, data.position, Time.deltaTime * 10f);

        // 좌우 회전 적용 (본체 회전)
        transform.rotation = Quaternion.Euler(-data.rotationX, data.rotationY, 0);
    }
}
