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
            }
            else
            {
                Debug.Log("FireDelay");
            }
            yield return new WaitForSeconds(_weaponDataSO.attackDelay); 
        }
    }
}
