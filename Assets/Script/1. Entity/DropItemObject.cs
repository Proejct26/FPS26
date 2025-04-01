using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemObject : MonoBehaviour
{
    [SerializeField] WeaponDataSO _weaponDataSO;

    private void OnCollisionEnter(Collision other)
    { 
         if (other.gameObject.TryGetComponent(out PlayerWeaponHandler playerWeaponHandler))
         {
            if (!playerWeaponHandler.AddWeapon(_weaponDataSO))
                return;
              
            Managers.Pool.Release(gameObject);  
         }

    }
}
