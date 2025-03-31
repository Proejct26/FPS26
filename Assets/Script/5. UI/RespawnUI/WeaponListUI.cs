using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponListUI : UI_Popup    
{
    [SerializeField] private Button _closeButton; 
    [SerializeField] private GameObject _buttonPrefab; 
    [SerializeField] private Transform _listParent; 
 
    private List<WeaponDataSO> _weaponDatas;

    private void Awake()
    {
        _closeButton.onClick.AddListener(ClosePopupUI); 
    }

    public void InitWeaponList(EWeaponType weaponType)
    {
        _weaponDatas = Managers.Data.WeaponData.GetWeaponData(weaponType);

        foreach (var weaponData in _weaponDatas)
        {
            GameObject button = Instantiate(_buttonPrefab, _listParent); 
            Util.FindChild<Image>(button, "WeaponIcon", true).sprite = weaponData.weaponIcon;
            Util.FindChild<TextMeshProUGUI>(button, "WeaponName", true).text = weaponData.weaponName;
            button.GetComponent<Button>().onClick.AddListener(() => SelectWeapon(weaponData)); 
        }   
    } 
  
    private void SelectWeapon(WeaponDataSO weaponData)
    {   
        ClosePopupUI();
       Managers.UI.ShowPopupUI<WeaponSelectUI>().InitWeaponInfo(weaponData); 
    } 
 
    


}
 