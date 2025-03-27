using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseController : MonoBehaviour
{
    [SerializeField] protected WeaponDataSO _weaponDataSO;


    public abstract void Attack();
}
