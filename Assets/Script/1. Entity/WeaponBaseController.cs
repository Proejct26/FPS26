using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponBaseController : MonoBehaviour
{
    [SerializeField] protected WeaponDataSO _weaponDataSO;

    private Coroutine _fireCoroutine;
    float lastFireTime;
    protected virtual void Start()
    {
        BindInputAction();
    }

    public void OnEnable() => BindInputAction();
    public void OnDisable() => UnbindInputAction();
    
    protected virtual void BindInputAction()
    {
        if (Managers.IsNull)
            return; 

        Managers.Input.Fire.started += OnFire;
        Managers.Input.Fire.canceled += OnFire;
    } 
    protected virtual void UnbindInputAction()
    {
        if (Managers.IsNull)
            return;
        Managers.Input.Fire.started -= OnFire;
        Managers.Input.Fire.canceled -= OnFire;
    }

    public abstract void Attack();

    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _fireCoroutine = StartCoroutine(FireCoroutine());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            StopCoroutine(_fireCoroutine);
        }
    }


    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            if (Time.time - lastFireTime >= _weaponDataSO._attackDelay * 0.7f)
            {
                Attack();
                lastFireTime = Time.time;
            }
            else
            {
                Debug.Log("FireDelay");
            }
            yield return new WaitForSeconds(_weaponDataSO._attackDelay); 
        }
    }
}
