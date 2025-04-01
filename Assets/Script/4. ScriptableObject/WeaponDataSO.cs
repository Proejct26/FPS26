using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AmmoSettings
{
    [SerializeField] public GameObject _bulletPrefab; // 총알 프리펩
    [SerializeField, Range(0, 100)] public int initializeAmmo; // 초기 탄약
    [SerializeField, Range(0, 1000)] public int ammoLimit; // 최대 장탄수
    [SerializeField, Range(0f, 5f)] public float reloadTime;
    [SerializeField, Range(1, 15)] public int projectileCount = 1; 
}

[Serializable]
public class RecoilSettings
{  
    [SerializeField, Range(0, 50)] public int recoilAmount = 1;    // 수직 반동 강도
    [SerializeField, Range(0, 50)] public int aimModeRecoilAmount = 1;    // 수직 반동 강도
    [SerializeField, Range(0, 1f)] public float recoilSpeed = 1f;      // 반동 적용 속도
    [SerializeField, Range(10, 90)] public int maxRecoilAngle = 80;   // 최대 반동 각도 
}  
 

[Serializable]
public class SpreadSettings
{ 
    [SerializeField, Range(0, 10f)] public float originBulletSpread;  // 원래 탄퍼짐
    [SerializeField, Range(0, 10f)] public float aimingModeSpread; // 에임 모드시 명중률 
    [SerializeField, Range(0, 30f)] public float maxSpread;  // 최대 탄퍼짐  
    [SerializeField, Range(0, 2f)] public float spreadRecoverySpeed = 0.1f; // 회복 속도  
    [SerializeField, Range(0, 1f)] public float spreadIncrease = 0.1f; // 사격시 명중률 떨어지는 정도   
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/WeaponData")]
public class WeaponDataSO : ScriptableObject 
{
    [Header("UI")]
    [SerializeField] public Sprite weaponIcon;
    [SerializeField] public GameObject dropItemPrefab;
    [SerializeField] public GameObject itemPrefab;
    [SerializeField] public EWeaponType weaponType;
    [SerializeField] public string weaponName; 

    [Header("Weapon Base")]
    [SerializeField] public int damage;
    [SerializeField] public float attackDelay;   

    [Header("Settings")]
    [SerializeField] public AmmoSettings ammoSettings;
    [SerializeField] public RecoilSettings recoilSettings; 
    [SerializeField] public SpreadSettings spreadSettings;
    


    [Header("Grenade")]
    [SerializeField, Range(0.1f, 10f)] public float grenadeWeight = 1f;   


} 
