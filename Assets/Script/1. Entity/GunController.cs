using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : WeaponBaseController
{
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Transform _ejectionPort;
    private int LoadedAmmo = 0; // 장전된 탄약  
    private int RemainAmmo = 0; // 남은 탄약

    private void Start() 
    {
        Managers.Input.Fire.started += OnFire;
        Managers.Input.Jump.started += (InputAction.CallbackContext context) => Debug.Log("점프");
    }  

    private void OnFire(InputAction.CallbackContext context)
    {
        Attack();
    }
 
    public override void Attack()
    {
        Debug.Log("총 쐈다다");
         
        Vector3 dir = (_ejectionPort.up + _ejectionPort.forward).normalized;
        GameObject bullet = Instantiate(_weaponDataSO._bulletPrefab, _ejectionPort.position, _ejectionPort.rotation);
        bullet.GetOrAddComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);  

        Destroy(bullet, 3f); 
    }


}
