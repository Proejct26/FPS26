using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : WeaponBaseController
{
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Transform _ejectionPort;

    // Component
    private GunAnimationHandler _gunAnimationHandler;

    // Variable
    private Vector3 _targetPosition;
    private int LoadedAmmo = 0; // 장전된 탄약  
    private int RemainAmmo = 0; // 남은 탄약
    private int spentBullet = 1;

    void Awake()
    {
        _gunAnimationHandler = gameObject.GetOrAddComponent<GunAnimationHandler>();  
    }

    protected override void BindInputAction()
    {
        base.BindInputAction();

        if (Managers.IsNull)
            return;

        Managers.Input.GetInput(EPlayerInput.Reload).started += OnReload;
        Managers.Input.GetInput(EPlayerInput.AimMode).started += OnAimMode;
        Managers.Input.GetInput(EPlayerInput.AimMode).canceled += OnAimMode;

 
    }

    protected override void UnbindInputAction()
    {
        base.UnbindInputAction();

        if (Managers.IsNull)
            return;

        Managers.Input.GetInput(EPlayerInput.Reload).started -= OnReload;
        Managers.Input.GetInput(EPlayerInput.AimMode).started -= OnAimMode;
        Managers.Input.GetInput(EPlayerInput.AimMode).canceled -= OnAimMode;
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        _gunAnimationHandler.Reload(); 
    }

    private void OnAimMode(InputAction.CallbackContext context)
    {
        _gunAnimationHandler.AimMode(context.phase == InputActionPhase.Started); 
    }

    void OnDrawGizmos()
    {
        if (_targetPosition == Vector3.zero)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_muzzle.position, _targetPosition);  
    } 

    private void GizmoInit()
    {
        _targetPosition = Vector3.zero;
    }

    public override void Attack() 
    { 
        SpawnEffect();
        Trigger();
        Invoke("GizmoInit", 0.1f);
        _gunAnimationHandler.Fire();
    }

    private void Trigger()
    {
        _targetPosition = Camera.main.transform.position + GetShootDir() * 100f; 
    }



    private void SpawnEffect()
    {
        // 총알
        GameObject bullet = Managers.Pool.Get(_weaponDataSO._bulletPrefab);
        bullet.transform.position = _ejectionPort.position;
        bullet.transform.rotation = _ejectionPort.rotation;
        Managers.Pool.Release(bullet, 3f);  

        // 탄피 효과
        Vector3 dir = (transform.up + transform.right).normalized;  
        Rigidbody rigidbody = bullet.GetOrAddComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero; 
        rigidbody.AddForce(dir, ForceMode.Impulse);  

        // 머즐 효가
        GameObject muzzleFlash = Managers.Pool.Get($"Weapon/muzzelFlash 0{Random.Range(1, 6)}"); 
        muzzleFlash.transform.position = _muzzle.position;
        muzzleFlash.transform.rotation = _muzzle.rotation; 
        Managers.Pool.Release(muzzleFlash, 0.05f);

        
    }

    private Vector3 GetShootDir()
    {
        Vector3 _direction = Camera.main.transform.forward;  // 전방 기준
        float _spread = UnityEngine.Random.Range(-_weaponDataSO.bulletSpread, _weaponDataSO.bulletSpread) * spentBullet;
        _spread *= 0.01f;
        // 카메라의 위치 동기화 + 탄퍼짐
        if (_spread < _weaponDataSO.maxSpread)
        {
            _direction += _muzzle.up * _spread; 
            _direction += _muzzle.right * _spread;
        }
        else
        {
        _direction = _muzzle.up * _weaponDataSO.maxSpread;
        _direction = _muzzle.right * _weaponDataSO.maxSpread;
        }

    // 결과값 리턴
    return _direction.normalized;
  }

}
