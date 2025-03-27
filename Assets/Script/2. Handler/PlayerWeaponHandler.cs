using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private Transform weaponParent;
    [SerializeField] private GameObject mainWeaponPrefab;
    [SerializeField] private GameObject subWeaponPrefab;
    [SerializeField] private GameObject knifePrefab;

    WeaponBaseController mainWeapon;
    WeaponBaseController subWeapon;
    WeaponBaseController knife;

    void Awake()
    {
        InitWeapons();
    }
 
    private void Start() 
    {
        BindInputAction();
        SetActiveWeapon(0); 
    }

    private void InitWeapons()
    {
        mainWeapon = Instantiate(mainWeaponPrefab, weaponParent).GetComponent<WeaponBaseController>(); 
        subWeapon = Instantiate(subWeaponPrefab, weaponParent).GetComponent<WeaponBaseController>();
        knife = Instantiate(knifePrefab, weaponParent).GetComponent<WeaponBaseController>();
    }
    private void BindInputAction()
    {
        Managers.Input.MainWeapon.started += (InputAction.CallbackContext context) => SetActiveWeapon(0);
        Managers.Input.SubWeapon.started += (InputAction.CallbackContext context) => SetActiveWeapon(1);
        Managers.Input.KnifeWeapon.started += (InputAction.CallbackContext context) => SetActiveWeapon(2);
    }
 
    public void SetActiveWeapon(int index)
    {
        mainWeapon.gameObject.SetActive(index == 0); 
        subWeapon.gameObject.SetActive(index == 1);
        knife.gameObject.SetActive(index == 2);
    }
}