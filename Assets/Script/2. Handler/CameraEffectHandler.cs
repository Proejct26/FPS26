using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraEffectHandler : MonoBehaviour
{

    [SerializeField] private GameObject _cameraHolder;
    [SerializeField] private Camera _weaponCam;
    [SerializeField] private CinemachineVirtualCamera _virtualMainCam;

    [SerializeField] private float _aimFOVOffset = 20f;
    [SerializeField] private float _sniperFOVOffset = 30f;


    private CinemachineBasicMultiChannelPerlin _perlin;
    private PlayerCameraHandler _playerCameraHandler;
    private Coroutine _recoilCoroutine; 
    private Coroutine _fovCoroutine; 
    private float _defaultFOV;
    private float _defaultWeaponCamFOV;

    private void Awake() 
    {
        _perlin = _virtualMainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _playerCameraHandler = FindFirstObjectByType<PlayerCameraHandler>();
        _defaultFOV = _virtualMainCam.m_Lens.FieldOfView;
        _defaultWeaponCamFOV = _weaponCam.fieldOfView;
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

    public void SetAimMode(bool active)
    {
        if (_fovCoroutine != null)
            StopCoroutine(_fovCoroutine);

        float mainCamfov = active ? _defaultFOV - _aimFOVOffset : _defaultFOV;
        float weaponCamfov = active ? _defaultWeaponCamFOV - _aimFOVOffset : _defaultWeaponCamFOV;
        _fovCoroutine = StartCoroutine(SetFOV(mainCamfov, weaponCamfov, 0.5f)); 
    }
 
    public void SetSniperMode(bool active)
    {
        if (_fovCoroutine != null)
            StopCoroutine(_fovCoroutine);

        float mainCamfov = active ? _defaultFOV - _sniperFOVOffset : _defaultFOV;
        float weaponCamfov = active ? _defaultWeaponCamFOV - _sniperFOVOffset : _defaultWeaponCamFOV;
        _fovCoroutine = StartCoroutine(SetFOV(mainCamfov, weaponCamfov, 0.5f));  
    } 

    private IEnumerator SetFOV(float mainCamfov, float weaponCamfov, float speed)
    {
        float currentMainCamFOV = _virtualMainCam.m_Lens.FieldOfView;
        float currentWeaponCamFOV = _weaponCam.fieldOfView;
        float curTime = 0;

        while (curTime < speed)
        {
            curTime += Time.deltaTime;
            currentMainCamFOV = Mathf.Lerp(currentMainCamFOV, mainCamfov, curTime / speed);
            currentWeaponCamFOV = Mathf.Lerp(currentWeaponCamFOV, weaponCamfov, curTime / speed);

            _weaponCam.fieldOfView = currentWeaponCamFOV; 
            _virtualMainCam.m_Lens.FieldOfView = currentMainCamFOV;
            
            yield return null;
        }
    }
}
