using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{

    [SerializeField] private GameObject _cameraHolder;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private PlayerCameraHandler _playerCameraHandler;
    private Coroutine _recoilCoroutine; 

    private void Awake()
    {
        _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _playerCameraHandler = FindFirstObjectByType<PlayerCameraHandler>();
    } 
 
    public void SetRecoil(float Y, float X, float speed, float returnSpeed, float maxRecoilAngle = 50f)
    {
        if (_recoilCoroutine != null)
            StopCoroutine(_recoilCoroutine);
 
        X = Random.Range(-X, X);
        _recoilCoroutine = StartCoroutine(Recoil(-Y, X, speed, returnSpeed, maxRecoilAngle));  
    } 

    private IEnumerator Recoil(float targetY, float targetX, float speed, float returnSpeed, float maxRecoilAngle)
    {
        // 현재 회전값을 저장
     
        float currentTime = 0;

        while (currentTime < speed)
        {
            currentTime += Time.deltaTime;
            _playerCameraHandler.xRotation += targetY * (Time.deltaTime / speed);
            
            yield return null; 
        }
        
    } 
 
}
