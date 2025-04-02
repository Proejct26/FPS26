using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyGunController : MonoBehaviour
{
    [SerializeField] private WeaponDataSO _weaponDataSO;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Transform _enjectionPort;

    public void Fire()
    {
        WeaponBaseController.SpawnMuzzleFlash(_muzzle.position, _muzzle.rotation.eulerAngles);
        WeaponBaseController.SpawnBullet(_weaponDataSO.ammoSettings._bulletPrefab, _enjectionPort.position, 
            _muzzle.forward, (transform.up + transform.right).normalized);

        Managers.Sound.Play3D("Sound/Weapon/shotSound", _muzzle.position); 
    } 
}
 