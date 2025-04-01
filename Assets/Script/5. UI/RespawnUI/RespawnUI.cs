using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUI : UI_Popup
{
    enum Buttons
    {
        PrimaryWeaponButton,
        SecondaryWeaponButton,
        ThrowableWeaponButton, 
        KnifeWeaponButton,
        RespawnButton,
    }
    enum Texts
    {
        respawnTimeText
    } 
    enum Images
    {
        RespawnTimeImage
    }

    private PlayerWeaponHandler _playerWeaponHandler;

    private Image[] _weaponImages;
    private TextMeshProUGUI[] _weaponNames;

    private float _startTime = 0f;
    private float _respawnTime = 5f;


    private void Awake() 
    {
        BindInit(); 
        Init();
        
       // Cursor.)
        _startTime = Time.time;
        InvokeRepeating(nameof(UpdateRespawnTimeText), 0f, 1f);   
        _playerWeaponHandler = FindAnyObjectByType<PlayerWeaponHandler>();
        _playerWeaponHandler.OnChangeWeaponData += UpdateItemInfo; 
    }

    private void Start()
    {
 
        UpdateItemInfo();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;   
        Managers.Input.SetActive(false);

    } 


    private void BindInit()
    {
        _weaponImages = new Image[Enum.GetValues(typeof(EWeaponType)).Length];
        _weaponNames = new TextMeshProUGUI[Enum.GetValues(typeof(EWeaponType)).Length]; 

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts)); 
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.PrimaryWeaponButton).onClick.AddListener(() => OpenWeaponListUI(EWeaponType.Primary));
        GetButton((int)Buttons.SecondaryWeaponButton).onClick.AddListener(() => OpenWeaponListUI(EWeaponType.Secondary));
        GetButton((int)Buttons.ThrowableWeaponButton).onClick.AddListener(() => OpenWeaponListUI(EWeaponType.Throwable));
        GetButton((int)Buttons.KnifeWeaponButton).onClick.AddListener(() => OpenWeaponListUI(EWeaponType.Knife)); 
        GetButton((int)Buttons.RespawnButton).onClick.AddListener(Respawn);

        _weaponImages[(int)EWeaponType.Primary] = GetButton((int)Buttons.PrimaryWeaponButton).gameObject.FindChild<Image>("WeaponImage", true);
        _weaponImages[(int)EWeaponType.Secondary] = GetButton((int)Buttons.SecondaryWeaponButton).gameObject.FindChild<Image>("WeaponImage", true);
        _weaponImages[(int)EWeaponType.Throwable] = GetButton((int)Buttons.ThrowableWeaponButton).gameObject.FindChild<Image>("WeaponImage", true);
        _weaponImages[(int)EWeaponType.Knife] = GetButton((int)Buttons.KnifeWeaponButton).gameObject.FindChild<Image>("WeaponImage", true);

        _weaponNames[(int)EWeaponType.Primary] = GetButton((int)Buttons.PrimaryWeaponButton).gameObject.FindChild<TextMeshProUGUI>("WeaponName", true);
        _weaponNames[(int)EWeaponType.Secondary] = GetButton((int)Buttons.SecondaryWeaponButton).gameObject.FindChild<TextMeshProUGUI>("WeaponName", true);
        _weaponNames[(int)EWeaponType.Throwable] = GetButton((int)Buttons.ThrowableWeaponButton).gameObject.FindChild<TextMeshProUGUI>("WeaponName", true);
        _weaponNames[(int)EWeaponType.Knife] = GetButton((int)Buttons.KnifeWeaponButton).gameObject.FindChild<TextMeshProUGUI>("WeaponName", true);  
    }

    private void UpdateItemInfo()
    {
        foreach (EWeaponType weaponType in Enum.GetValues(typeof(EWeaponType)))
            UpdateWeaponInfo(weaponType);
    }

    private void UpdateWeaponInfo(EWeaponType weaponType)
    {
        WeaponDataSO weapon = _playerWeaponHandler.GetWeaponData(weaponType);
         
        _weaponImages[(int)weaponType].gameObject.SetActive(weapon != null);
        _weaponNames[(int)weaponType].gameObject.SetActive(weapon != null); 

        
        _weaponImages[(int)weaponType].sprite = weapon == null ? null : weapon.weaponIcon; 
        _weaponNames[(int)weaponType].text = weapon == null ? "" : weapon.weaponName;
    }


    private void OpenWeaponListUI(EWeaponType weaponType)
    {

        WeaponListUI weaponListUI = Managers.UI.ShowPopupUI<WeaponListUI>();
        weaponListUI.InitWeaponList(weaponType); 
    }

    private void UpdateRespawnTimeText()
    {

        int time = (int)(_respawnTime - (Time.time - _startTime));
        Get<TextMeshProUGUI>((int)Texts.respawnTimeText).text = time <= 0 ? "0" : time.ToString();    
    } 

    private void Respawn()
    {
        if (Time.time - _startTime < _respawnTime)
            return;

        _playerWeaponHandler.InitWeapons(); 
        ClosePopupUI(); 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;   

        Managers.Input.SetActive(true);
        Debug.Log("Respawn");
    }

}
