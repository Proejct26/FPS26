using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponBaseController : MonoBehaviour
{
    // Field
    [SerializeField] protected WeaponDataSO _weaponDataSO;

    // Data 
    protected AmmoSettings _ammoSettings;
    protected RecoilSettings _recoilSettings; 
    protected SpreadSettings _spreadSettings;

    // Variable
    private Coroutine _fireCoroutine;
    private float lastFireTime;
    protected int _LoadedAmmo = 0;// 장전된 탄약   
    protected int _RemainAmmo = 0; // 남은 탄약 

    // Property
    public Sprite WeaponIcon => _weaponDataSO.weaponIcon;
    public WeaponDataSO WeaponDataSO => _weaponDataSO; 
    public int GetCurrentAmmo => _LoadedAmmo;
    public int GetMaxAmmo => _ammoSettings.initializeAmmo;

    // Component
    protected WeaponAnimationHandler _weaponAnimationHandler; 

    public event Action<int, int> OnChangeMagazine;

    protected virtual void Awake()
    {
        _ammoSettings = _weaponDataSO.ammoSettings;
        _recoilSettings = _weaponDataSO.recoilSettings;
        _spreadSettings = _weaponDataSO.spreadSettings;
    }

    protected virtual void Start()
    {
        BindInputAction();
    }

    protected virtual void OnEnable()
    {
        BindInputAction();
        OnChangeMagazine?.Invoke(_LoadedAmmo, _ammoSettings.initializeAmmo);
    }
    protected virtual void OnDisable()
    {
        UnbindInputAction();
    }
    
    protected virtual void BindInputAction()
    {
        if (Managers.IsNull)
            return;  

        Managers.Input.Fire.started += InputAttack; 
        Managers.Input.Fire.canceled += InputAttack;
    } 
    protected virtual void UnbindInputAction()
    {
        if (Managers.IsNull)
            return;
        Managers.Input.Fire.started -= InputAttack;
        Managers.Input.Fire.canceled -= InputAttack;
    }

    private void InputAttack(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
            Attack(true);
        else if (context.phase == InputActionPhase.Canceled)  
            Attack(false);
    }

    public void Attack(bool started)
    {
        if (started)  
            _fireCoroutine = StartCoroutine(FireCoroutine());
        else
            StopCoroutine(_fireCoroutine); 
    }
 
    protected abstract void Fire();

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            if (Time.time - lastFireTime >= _weaponDataSO.attackDelay * 0.7f)
            {
                Fire();
                lastFireTime = Time.time;
                OnChangeMagazine?.Invoke(_LoadedAmmo, _ammoSettings.initializeAmmo); 
            } 
            else
            {
                Debug.Log("FireDelay");
            }
            yield return new WaitForSeconds(_weaponDataSO.attackDelay); 
        }
    }

    protected virtual void ReloadAmmo()
    {
        _LoadedAmmo = Mathf.Min(_LoadedAmmo + _RemainAmmo, _ammoSettings.initializeAmmo);
        _RemainAmmo -= Mathf.Max(0, _LoadedAmmo - _ammoSettings.initializeAmmo);   
        OnChangeMagazine?.Invoke(_LoadedAmmo, _ammoSettings.initializeAmmo); 
    }
}
