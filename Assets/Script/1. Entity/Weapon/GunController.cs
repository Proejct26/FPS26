using Cinemachine;
using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : WeaponBaseController
{
    // Inspector
    [SerializeField] private CinemachineVirtualCamera _zoomCam;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Transform _ejectionPort;
    [SerializeField] private GameObject _sniperZoom;
    [SerializeField] private float _spread = 0; // 테스트용
 
    // Component
    private CameraEffectHandler _cameraEffectHandler;
    private PlayerCameraHandler _playerCameraHandler;


    // Variable
    private bool _isAimMode = false;
    private bool _isReloading = false;


    // Property
    public bool IsAimMode => _isAimMode;
    public float Spread => _spread * 10;

    private Coroutine _changeSpreadCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _LoadedAmmo = _ammoSettings.initializeAmmo;   
        _RemainAmmo = _ammoSettings.ammoLimit;
        _spread = _spreadSettings.originBulletSpread;

        if (_sniperZoom != null)
            _sniperZoom.SetActive(false);

        if (_zoomCam != null)
            _zoomCam.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();

        _cameraEffectHandler = FindFirstObjectByType<CameraEffectHandler>();
        _weaponAnimationHandler = gameObject.GetOrAddComponent<WeaponAnimationHandler>();  
        _playerCameraHandler = Managers.Player.GetComponent<PlayerCameraHandler>();    
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
        if (_RemainAmmo <= 0 || _LoadedAmmo == _ammoSettings.initializeAmmo || _isAimMode || _isReloading) 
            return; 
  
        _isReloading = true;
        _weaponAnimationHandler.HasAmmo(_LoadedAmmo > 0);  
        _weaponAnimationHandler.Reload(); 
        Managers.Sound.Play("Sound/Weapon/reloadSound");
        
        Invoke(nameof(ReloadAmmo), _ammoSettings.reloadTime);  
    }
 
    protected override void ReloadAmmo()
    {
        base.ReloadAmmo();
        _isReloading = false;
    }

    private void InputAimMode(InputAction.CallbackContext context)
    {
        OnAimMode(context.phase == InputActionPhase.Started); 
    }

    public void OnAimMode(bool active)
    {
        if (_isReloading) 
            return; 

        if (_sniperZoom != null)
            _sniperZoom.SetActive(active);

        if (_zoomCam != null)
            _zoomCam.gameObject.SetActive(active); 


        _weaponAnimationHandler.AimMode(active);
        _isAimMode = active;
        ResetSpread();
    }

    protected override void Fire()  
    {  
        if (_LoadedAmmo <= 0 || _isReloading)
            return;      
        _LoadedAmmo--;  

        SpawnEffect();
        SetRecoil();
        _weaponAnimationHandler.Fire(); 

        GameObject[] targets = RayCasting(); 
    }  

    private void SetRecoil() 
    {
        _spread *= 1f + _spreadSettings.spreadIncrease; 
 
        ResetSpread();
        int recoilAmount = _isAimMode ? _recoilSettings.aimModeRecoilAmount : _recoilSettings.recoilAmount;
        _cameraEffectHandler.SetRecoil(recoilAmount, _recoilSettings.recoilSpeed, _recoilSettings.maxRecoilAngle); 
    }
    private GameObject[] RayCasting() 
    {

        Camera cam = _playerCameraHandler.GetCamera(_sniperZoom != null && _sniperZoom.activeSelf && IsAimMode);   
        Vector3 startPos = cam.transform.position;   
        GameObject[] targets = new GameObject[_ammoSettings.projectileCount];   

        for (int i = 0; i < _ammoSettings.projectileCount; i++)
        {
            Ray ray = new Ray(startPos, cam.transform.forward.RandomUnitVectorInCone(_spread));    
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            { 
                // 히트 이펙트
                CS_ATTACK attackPacket = new CS_ATTACK();
                attackPacket.PosX = hit.point.x;
                attackPacket.PosY = hit.point.y;
                attackPacket.PosZ = hit.point.z;
                attackPacket.NormalX = hit.normal.x;
                attackPacket.NormalY = hit.normal.y;
                attackPacket.NormalZ = hit.normal.z; 
                 
                Managers.Network.Send(attackPacket); 
                //WeaponBaseController.SpawnHitEffect(hit.point, hit.normal, attackPacket.BAttack);    
                // 충돌 체크 hit.collider.tag == "Head"
                targets[i] = hit.collider.gameObject;

                if (hit.collider.TryGetComponent(out RemotePlayerController controller))
                {
                    CS_SHOT_HIT shotHitPacket = new CS_SHOT_HIT();
                    shotHitPacket.PlayerId = controller.PlayerStateData.networkId;
                    shotHitPacket.Hp = (uint)(controller.PlayerStateData.curHp - _weaponDataSO.damage);  
                    Managers.Network.Send(shotHitPacket);
                }
            } 
            else
            {
                targets[i] = null; 
            }
        }
        return targets;
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

        Managers.Sound.Play("Sound/Weapon/shotSound");
        WeaponBaseController.SpawnMuzzleFlash(_muzzle.position, transform.forward);
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
