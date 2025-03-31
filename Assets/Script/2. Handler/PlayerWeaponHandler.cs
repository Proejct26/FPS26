using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EWeaponType
{
    Main = 0,
    Sub = 1,
    Knife = 2,
    Grenade = 3,
}

public class PlayerWeaponHandler : MonoBehaviour
{
    // Field
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private GameObject _mainWeaponPrefab;
    [SerializeField] private GameObject _subWeaponPrefab;
    [SerializeField] private GameObject _knifePrefab;

    // Weapon
    private WeaponBaseController[] _weapons;
    private WeaponBaseController _mainWeapon;
    private WeaponBaseController _subWeapon;
    private WeaponBaseController _knife;

    // Variable
    private int _selectedWeaponIndex = 0;

    // Property
    public WeaponBaseController CurrentWeapon => _weapons[_selectedWeaponIndex];
 

    // Event
    public event Action OnWeaponChanged;
   
    private void Awake()
    {
        InitWeapons();
        _weapons = new WeaponBaseController[Enum.GetValues(typeof(EWeaponType)).Length];
        _weapons[(int)EWeaponType.Main] = _mainWeapon;
        _weapons[(int)EWeaponType.Sub] = _subWeapon;
        _weapons[(int)EWeaponType.Knife] = _knife;

    }
 
    private void Start() 
    {
        BindInputAction();
        SetActiveWeapon(EWeaponType.Main); 
    }

    private void InitWeapons()
    {
        _mainWeapon = Instantiate(_mainWeaponPrefab, _weaponParent).GetComponent<WeaponBaseController>(); 
        _subWeapon = Instantiate(_subWeaponPrefab, _weaponParent).GetComponent<WeaponBaseController>();
        _knife = Instantiate(_knifePrefab, _weaponParent).GetComponent<WeaponBaseController>();
    }
    private void BindInputAction()
    {
        if (Managers.IsNull)
            return; 

        Managers.Input.GetInput(EPlayerInput.MainWeapon).started += (InputAction.CallbackContext context) => SetActiveWeapon(EWeaponType.Main);
        Managers.Input.GetInput(EPlayerInput.SubWeapon).started += (InputAction.CallbackContext context) => SetActiveWeapon(EWeaponType.Sub);
        Managers.Input.GetInput(EPlayerInput.KnifeWeapon).started += (InputAction.CallbackContext context) => SetActiveWeapon(EWeaponType.Knife);
    }
 
    private void SetActiveWeapon(EWeaponType weaponType)
    {
        int index = (int)weaponType;
        _selectedWeaponIndex = index;
        _mainWeapon.gameObject.SetActive(index == 0); 
        _subWeapon.gameObject.SetActive(index == 1);
        _knife.gameObject.SetActive(index == 2); 

        OnWeaponChanged?.Invoke();
    }
}