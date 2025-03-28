using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AmmoSettings
{
    [SerializeField] public GameObject _bulletPrefab; // 총알 프리펩
    [SerializeField, Range(0, 100)] public int initializeAmmo; // 초기 탄약
    [SerializeField, Range(0, 100)] public int ammoLimit; // 최대 장탄수
    [SerializeField, Range(0f, 5f)] public float reloadTime;
}

[Serializable]
public class RecoilSettings
{
    [SerializeField, Range(0, 50f)] public float verticalRecoil = 1f;    // 수직 반동 강도
    [SerializeField, Range(0, 50f)] public float horizontalRecoil = 0.5f; // 수평 반동 강도
    [SerializeField, Range(0, 1f)] public float recoilSpeed = 1f;      // 반동 적용 속도
    [SerializeField, Range(0, 1f)] public float returnSpeed = 1f;       // 원위치 복귀 속도
    [SerializeField, Range(10, 90f)] public float maxRecoilAngle = 90f;   // 최대 반동 각도 
} 


[Serializable]
public class SpreadSettings
{
    [SerializeField, Range(0, 1f)] public float originBulletSpread;  // 원래 탄퍼짐
    [SerializeField, Range(0, 1f)] public float maxSpread = 1f;  // 최대 탄퍼짐  
    [SerializeField, Range(0, 1f)] public float spreadRecoverySpeed = 0.1f; // 회복 속도 
    [SerializeField, Range(0, 1f)] public float spreadIncrease = 0.1f; // 사격시 명중률 떨어지는 정도   
    [SerializeField, Range(0, 1f)] public float aimingModeSpread = 0.5f; // 에임 모드시 명중률 
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/WeaponData")]
public class WeaponDataSO : ScriptableObject 
{
    [Header("Weapon Base")]
    [SerializeField] public int damage;
    [SerializeField] public float attackDelay;   

    [SerializeField] public AmmoSettings ammoSettings;
    [SerializeField] public RecoilSettings recoilSettings; 
    [SerializeField] public SpreadSettings spreadSettings;
    
    [Header("Grenade")]
    [SerializeField, Range(0.1f, 10f)] public float grenadeWeight = 1f;   


} 
