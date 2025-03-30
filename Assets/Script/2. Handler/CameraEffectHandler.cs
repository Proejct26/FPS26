using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraEffectHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualMainCam;

    private CinemachineBasicMultiChannelPerlin _perlin;
    private PlayerCameraHandler _playerCameraHandler;
    private Coroutine _recoilCoroutine; 

    private void Awake() 
    {
        _perlin = _virtualMainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _playerCameraHandler = FindFirstObjectByType<PlayerCameraHandler>();
    } 
 
    public void SetRecoil(float X, float speed, float returnSpeed, float maxRecoilAngle = 50f)
    {
        if (_recoilCoroutine != null)
            StopCoroutine(_recoilCoroutine);
 
        _recoilCoroutine = StartCoroutine(Recoil(-X, speed, returnSpeed, maxRecoilAngle));  
    } 

    private IEnumerator Recoil(float targetX, float speed, float returnSpeed, float maxRecoilAngle)
    {
        // 현재 회전값을 저장
       
        float currentTime = 0;

        while (currentTime < speed)
        {
            currentTime += Time.deltaTime;
            _playerCameraHandler.xRotation += targetX * (Time.deltaTime / speed);
            
            yield return null; 
        } 
        
    } 
}
