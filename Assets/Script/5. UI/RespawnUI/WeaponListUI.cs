using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponListUI : UI_Popup    
{
    [SerializeField] private GameObject _buttonPrefab; 
    [SerializeField] private Transform _listParent; 
 
    private List<WeaponDataSO> _weaponDatas;

    public void InitWeaponList(EWeaponType weaponType)
    {
        _weaponDatas = Managers.Data.WeaponData.GetWeaponData(weaponType);

        foreach (var weaponData in _weaponDatas)
        {
            GameObject button = Instantiate(_buttonPrefab, _listParent); 
            Util.FindChild<Image>(button, "WeaponIcon", true).GetComponent<Image>().sprite = weaponData.weaponIcon;
            button.GetComponent<Button>().onClick.AddListener(() => SelectWeapon(weaponData)); 
        }  
    } 
 
    private void SelectWeapon(WeaponDataSO weaponData)
    {
        Managers.UI.ShowPopupUI<WeaponSelectUI>().InitWeaponInfo(weaponData); 
    }

    


}
 