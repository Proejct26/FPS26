using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{

    [SerializeField] private GameObject _cameraHolder;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _perlin;

    void Awake()
    {
        _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    } 

    private Coroutine _recoilCoroutine;
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
        Vector3 currentRotation = _cameraHolder.transform.localRotation.eulerAngles;
        
        // 목표 회전값 계산 (현재 회전값에 반동값을 더함)
        Vector3 targetEuler = new Vector3(
            currentRotation.x + targetY, 
            currentRotation.y + targetX,
            currentRotation.z
        ); 
         
        // 최대 반동 각도 제한
        targetEuler = Vector3.ClampMagnitude(targetEuler - currentRotation, maxRecoilAngle) + currentRotation;
        
        float currentTime = 0f;
        
        // 반동 적용
        while (currentTime < speed)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / speed;
            // 현재 회전값에서 목표 회전값으로 부드럽게 보간
            _cameraHolder.transform.localRotation = Quaternion.Euler(
                Vector3.Lerp(currentRotation, targetEuler, t)
            );
            yield return null;
        }
        
    } 
 
}
