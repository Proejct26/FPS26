using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponStorageSO", menuName = "ScriptableObject/WeaponStorageSO")]
public class WeaponStorageSO : ScriptableObject
{
    [SerializeField] private List<WeaponDataSO> _weaponDatas;

    public List<WeaponDataSO> GetWeaponsByType(EWeaponType weaponType)
    {
        return _weaponDatas.FindAll(data => data.weaponType == weaponType);  
    }

    public WeaponDataSO GetWeaponData(int key)
    {
        return _weaponDatas.Find(data => data.key == key);   
    } 
}
