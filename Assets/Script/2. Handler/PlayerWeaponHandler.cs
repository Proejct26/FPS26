using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EWeaponType
{
    Primary = 0,
    Secondary = 1,
    Knife = 2,
    Throwable = 3, 
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

    // Variable
    private int _selectedWeaponIndex = 0;

    // Property
    public WeaponBaseController CurrentWeapon => _weapons[_selectedWeaponIndex];
 
    // Event
    public event Action<int, int> OnChangeMagazine;
    public event Action OnChangeWeapon; 

    //public event Action On 
    private void Awake()
    {
        _weapons = new WeaponBaseController[Enum.GetValues(typeof(EWeaponType)).Length];
        
    }
 
    private void Start() 
    {
        InitWeapons();
        BindInputAction();
        EquipWeapon(EWeaponType.Primary); 
    }

    private void InitWeapons()
    {
        AddWeapon(EWeaponType.Primary, _mainWeaponPrefab);
        AddWeapon(EWeaponType.Secondary, _subWeaponPrefab);
        AddWeapon(EWeaponType.Knife, _knifePrefab); 

        EquipWeapon(EWeaponType.Primary);
    }

    private void BindInputAction()
    {
        if (Managers.IsNull)
            return; 
 
        Managers.Input.GetInput(EPlayerInput.MainWeapon).started += (InputAction.CallbackContext context) => EquipWeapon(EWeaponType.Primary);
        Managers.Input.GetInput(EPlayerInput.SubWeapon).started += (InputAction.CallbackContext context) => EquipWeapon(EWeaponType.Secondary);
        Managers.Input.GetInput(EPlayerInput.KnifeWeapon).started += (InputAction.CallbackContext context) => EquipWeapon(EWeaponType.Knife);
    }
 
    private void EquipWeapon(EWeaponType weaponType)
    {
        if (_weapons[(int)weaponType] == null)
            return;

        _weapons[_selectedWeaponIndex].OnChangeMagazine -= OnChangeMagazine; 
        _weapons[(int)weaponType].OnChangeMagazine += OnChangeMagazine;

        _selectedWeaponIndex = (int)weaponType;

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null)
                continue; 

            _weapons[i].gameObject.SetActive(i == _selectedWeaponIndex);
        }
 
        OnChangeWeapon?.Invoke();
    }
 
    public bool AddWeapon(WeaponDataSO weaponDataSO)
    {
        int index = (int)weaponDataSO.weaponType;
        if (_weapons[index] != null) // 이미 해당 타입의 무기를 장착중이라면 false 반환
            return false;

        AddWeapon(weaponDataSO.weaponType, weaponDataSO.itemPrefab); 
        return true; 
    }
    private void AddWeapon(EWeaponType weaponType, GameObject itemPrefab) 
    {
        if (itemPrefab == null)
            return;

        int index = (int)weaponType;
        _weapons[index] = Instantiate(itemPrefab, _weaponParent).GetComponent<WeaponBaseController>();
        EquipWeapon(weaponType); 
    } 
 
    private void ThrowWeapon()
    {
        EWeaponType weaponType = CurrentWeapon.WeaponDataSO.weaponType; 
        int index = (int)weaponType;
        if (_weapons[index] == null || weaponType == EWeaponType.Knife) 
            return;

        GameObject weapon = _weapons[index].gameObject;
        if (weapon.TryGetComponent(out WeaponBaseController weaponBaseController))
        {
            WeaponDataSO weaponDataSO = weaponBaseController.WeaponDataSO;
            GameObject dropItem = Managers.Pool.Get(weaponDataSO.dropItemPrefab);

            dropItem.transform.position = Managers.Player.transform.position + Managers.Player.transform.forward * 1f;
            dropItem.GetOrAddComponent<Rigidbody>().AddForce(Managers.Player.transform.forward * 4f, ForceMode.Impulse);
        } 
        
        Destroy(weapon); 
        EquipWeapon(EWeaponType.Knife);
        _weapons[index] = null;
    }
}