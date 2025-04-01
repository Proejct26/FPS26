using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartPopup : UI_Popup
{
    enum Buttons
    {
        StartButton,
        SettingButton,
        QuitButton
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.StartButton).onClick.AddListener(OnStartButtonClick);
        GetButton((int)Buttons.SettingButton).onClick.AddListener(OnSettingButtonClick);
        GetButton((int)Buttons.QuitButton).onClick.AddListener(OnQuitButtonClick);
    }


    private void OnStartButtonClick()
    {
        Managers.UI.ShowPopupUI<UI_CustomPopup>("CustomPopup");
        // ClosePopupUI();
    }

    private void OnSettingButtonClick()
    {
        Managers.UI.ShowPopupUI<UI_SettingPopup>("SettingPopup");
        // ClosePopupUI();
        
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
