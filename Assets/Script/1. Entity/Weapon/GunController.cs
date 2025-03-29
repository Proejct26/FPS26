using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : WeaponBaseController
{
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Transform _ejectionPort;
    [SerializeField] private float _spread = 0; // 테스트용

    // Component
    private GunAnimationHandler _gunAnimationHandler;
 
    // Variable
    private int _LoadedAmmo = 0; // 장전된 탄약  
    private int _RemainAmmo = 0; // 남은 탄약
    private bool _isAimMode = false;

    private AmmoSettings _ammoSettings;
    private RecoilSettings _recoilSettings;
    private SpreadSettings _spreadSettings;

    // Property
    public bool IsAimMode => _isAimMode;
    public float Spread => _spread * 10;

    private Coroutine _changeSpreadCoroutine;

    private void Awake()
    {
        _gunAnimationHandler = gameObject.GetOrAddComponent<GunAnimationHandler>();  

        _ammoSettings = _weaponDataSO.ammoSettings;
        _recoilSettings = _weaponDataSO.recoilSettings;
        _spreadSettings = _weaponDataSO.spreadSettings;
    
        _LoadedAmmo = _ammoSettings.initializeAmmo;   
        _RemainAmmo = _ammoSettings.ammoLimit;
        _spread = _spreadSettings.originBulletSpread;
    }
 

    protected override void BindInputAction()
    {
        base.BindInputAction();

        if (Managers.IsNull)
            return;

        Managers.Input.GetInput(EPlayerInput.Reload).started += OnReload;
        Managers.Input.GetInput(EPlayerInput.AimMode).started += InputAimMode;
        Managers.Input.GetInput(EPlayerInput.AimMode).canceled += InputAimMode;

 
    }

    protected override void UnbindInputAction()
    {
        base.UnbindInputAction();

        if (Managers.IsNull)
            return;

        Managers.Input.GetInput(EPlayerInput.Reload).started -= OnReload;
        Managers.Input.GetInput(EPlayerInput.AimMode).started -= InputAimMode;
        Managers.Input.GetInput(EPlayerInput.AimMode).canceled -= InputAimMode;
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (_RemainAmmo <= 0)
            return;

        _gunAnimationHandler.HasAmmo(_LoadedAmmo > 0);  
        _gunAnimationHandler.Reload(); 
        Managers.Sound.Play("Sound/Weapon/reloadSound");
        
        Invoke(nameof(ReloadAmmo), _ammoSettings.reloadTime);  
    }
 
    private void ReloadAmmo()
    {
        _LoadedAmmo = Mathf.Min(_LoadedAmmo + _RemainAmmo, _ammoSettings.initializeAmmo);
        _RemainAmmo -= Mathf.Max(0, _LoadedAmmo - _ammoSettings.initializeAmmo);   
    }

    private void InputAimMode(InputAction.CallbackContext context)
    {
        OnAimMode(context.phase == InputActionPhase.Started); 
    }

    public void OnAimMode(bool active)
    {
        _gunAnimationHandler.AimMode(active);
        _isAimMode = active;
        ResetSpread();
    }

    protected override void Fire()  
    {  
        if (_LoadedAmmo <= 0)
            return; 
            
        _LoadedAmmo--;  

        SpawnEffect();
        Trigger();
        SetRecoil();

        _gunAnimationHandler.Fire(); 
    }  
    private void SetRecoil() 
    {
        _spread *= 1f + _spreadSettings.spreadIncrease; 
 
        ResetSpread();
        int recoilAmount = _isAimMode ? _recoilSettings.aimModeRecoilAmount : _recoilSettings.recoilAmount;
        Camera.main.GetComponent<CameraShaker>().SetRecoil(recoilAmount, _recoilSettings.recoilSpeed, _recoilSettings.returnSpeed, _recoilSettings.maxRecoilAngle);
    }
    private void Trigger()
    {
        Vector3 startPos = Camera.main.transform.position;
        Ray ray = new Ray(startPos, Camera.main.transform.forward.RandomUnitVectorInCone(_spread));   

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        { 
            var particle = Managers.Pool.Get("Particle/HitEffect_Wall"); 
            particle.transform.position = hit.point;
            particle.transform.rotation = Quaternion.LookRotation(hit.normal); 
            Managers.Pool.Release(particle, 5f);  

            // 충돌 체크 hit.collider.tag == "Head"
        }
    }
    private void SpawnEffect()
    {
        // 총알
        GameObject bullet = Managers.Pool.Get(_ammoSettings._bulletPrefab);
        bullet.transform.position = _ejectionPort.position;
        bullet.transform.rotation = _ejectionPort.rotation;
        Managers.Pool.Release(bullet, 3f);  

        // 탄피 효과
        Vector3 dir = (transform.up + transform.right).normalized;  
        Rigidbody rigidbody = bullet.GetOrAddComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero; 
        rigidbody.AddForce(dir, ForceMode.Impulse);  
 
        // 머즐 효가
        GameObject muzzleFlash = Managers.Pool.Get($"Sprite/Weapon/muzzelFlash 0{Random.Range(1, 6)}"); 
        muzzleFlash.transform.position = _muzzle.position;
        muzzleFlash.transform.rotation = _muzzle.rotation; 
        Managers.Pool.Release(muzzleFlash, 0.05f);

        Managers.Sound.Play("Sound/Weapon/shotSound");
        
    }

    private void ResetSpread()
    {
        if (_changeSpreadCoroutine != null)
            StopCoroutine(_changeSpreadCoroutine);
        _changeSpreadCoroutine =  StartCoroutine(ChangeSpreadValue(_isAimMode ? _spreadSettings.aimingModeSpread : _spreadSettings.originBulletSpread, _spreadSettings.spreadRecoverySpeed));
    }

    private IEnumerator ChangeSpreadValue(float target, float time)
    {
        float currentSpread = _spread;
        float currentTime = 0f;

        while (currentTime < time)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / time;
            _spread = Mathf.Lerp(currentSpread, target, t);
            _spread = Mathf.Clamp(_spread, 0, _spreadSettings.maxSpread);
            yield return null;
        }
        _spread = target;
    }
    
}
