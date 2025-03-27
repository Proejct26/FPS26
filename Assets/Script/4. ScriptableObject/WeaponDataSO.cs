using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/WeaponData")]
public class WeaponDataSO : ScriptableObject 
{
    [Header("Weapon Base")]
    [SerializeField] public int _damage;
    [SerializeField] public float _attackDelay;   

    [Header("Ammo")]
    [SerializeField] public GameObject _bulletPrefab; // 총알 프리펩
    [SerializeField, Range(0, 100)] public int initializeAmmo; // 초기 탄약
    [SerializeField, Range(0, 100)] public int maxLoadedAmmo; // 최대 장탄수
    [SerializeField, Range(0f, 5f)] public float reloadTime;

    [Header("Recoil")]
    [SerializeField, Range(0, 10f)] public float recoilX; // 좌우 반동 크기
    [SerializeField, Range(0, 10f)] public float recoilY; // 상하 반동 크기
    [SerializeField, Range(0, 10f)] public float recoilRecoverySpeed; // 반동 회복 속도

    [Header("Bullet Spread")]
    [SerializeField, Range(0, 5f)] public float originBulletSpread;  // 원래 탄퍼짐
    [SerializeField, Range(0, 5f)] public float bulletSpread = 1f;  // 탄 퍼짐 정도
    [SerializeField, Range(0, 5f)] public float maxSpread = 1f;  // 최대 탄퍼짐 

    [Header("Grenade")]
    [SerializeField, Range(0.1f, 10f)] public float _grenadeWeight = 1f;   
} 
