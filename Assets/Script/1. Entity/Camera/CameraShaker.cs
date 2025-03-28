using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
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
        // 현재 회전값을 Quaternion으로 저장
        Quaternion startRotation = _virtualCamera.transform.rotation;
        
        // 목표 회전값 계산 (오일러 각도를 Quaternion으로 변환)
        Vector3 targetEuler = new Vector3(targetY, targetX, 0f);
        targetEuler = Vector3.ClampMagnitude(targetEuler, maxRecoilAngle);
        Quaternion targetRotation = Quaternion.Euler(targetEuler);
        
        float currentTime = 0f;
        
        // 반동 적용
        while (currentTime < speed)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / speed;
            // Slerp를 사용하여 회전을 부드럽게 보간
            _virtualCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        currentTime = 0f;
        Quaternion currentRotation = _virtualCamera.transform.rotation;

        // 원위치로 복귀 
        while (currentTime < returnSpeed)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / returnSpeed;
            // Slerp를 사용하여 회전을 부드럽게 보간
            _virtualCamera.transform.rotation = Quaternion.Slerp(currentRotation, Quaternion.identity, t);
            yield return null;
        }
        
        // 정확한 원위치로 복귀
        _virtualCamera.transform.rotation = Quaternion.identity;
    } 

}
