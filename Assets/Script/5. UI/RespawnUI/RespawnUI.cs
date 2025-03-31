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

    private float _startTime = 0f;
    private float _respawnTime = 5f;

    private void Awake() 
    {
        BindInit(); 
        Init();
        
       // Cursor.)
        _startTime = Time.time;
        InvokeRepeating(nameof(UpdateRespawnTimeText), 0f, 1f);   
    }


    private void BindInit()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts)); 
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.PrimaryWeaponButton).onClick.AddListener(() => OpenWeaponListUI(EWeaponType.Primary));
        GetButton((int)Buttons.SecondaryWeaponButton).onClick.AddListener(() => OpenWeaponListUI(EWeaponType.Secondary));
        GetButton((int)Buttons.ThrowableWeaponButton).onClick.AddListener(() => OpenWeaponListUI(EWeaponType.Throwable));
        GetButton((int)Buttons.RespawnButton).onClick.AddListener(Respawn);
    }

    private void OpenWeaponListUI(EWeaponType weaponType)
    {
        WeaponListUI weaponListUI = Managers.UI.ShowPopupUI<WeaponListUI>();
        weaponListUI.InitWeaponList(weaponType); 
    }

    private void UpdateRespawnTimeText()
    {
        Get<TextMeshProUGUI>((int)Texts.respawnTimeText).text = $"{(int)(_respawnTime - (Time.time - _startTime))}";  
    } 

    private void Respawn()
    {
        if (Time.time - _startTime < _respawnTime)
            return;

         
    }

}
