using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropItemDataSO", menuName = "DropItemDataSO")]
public class DropItemDataSO : ScriptableObject
{
    public GameObject itemPrefab; 
    public EWeaponType weaponType;
}
