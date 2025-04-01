using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponStorageSO", menuName = "ScriptableObject/WeaponStorageSO")]
public class WeaponStorageSO : ScriptableObject
{
    [SerializeField] private List<WeaponDataSO> _weaponDatas;

    public List<WeaponDataSO> GetWeaponData(EWeaponType weaponType)
    {
        return _weaponDatas.FindAll(data => data.weaponType == weaponType);  
    }
}
