using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UI_CustomPopup : UI_Popup
{
    enum Buttons
    {
        EnterGameButton,
        CustomBackButton
    }

    enum InputFields
    {
        NickNameInputField
    }

    enum Texts
    {
        CustomErrorText
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<TextMeshProUGUI>(typeof(Texts));
        
        GetButton((int)Buttons.EnterGameButton).onClick.AddListener(OnEnterGameButtonClick);
        GetButton((int)Buttons.CustomBackButton).onClick.AddListener(OnBackButtonClick);
        
        Get<TextMeshProUGUI>((int)Texts.CustomErrorText).gameObject.SetActive(false);
        Get<TMP_InputField>((int)InputFields.NickNameInputField).text = Managers.Data.Nickname;
    }

    private void OnEnterGameButtonClick()
    {
        string nickName = Get<TMP_InputField>((int)InputFields.NickNameInputField).text.Trim();
        if (!IsValidNickName(nickName, out string errorMessage))
        {
            Get<TextMeshProUGUI>((int)Texts.CustomErrorText).text = errorMessage;
            Get<TextMeshProUGUI>((int)Texts.CustomErrorText).gameObject.SetActive(true);
            return;
        }

        Managers.Data.SettingData(nickname: nickName);
        Debug.Log($"NickName 설정: {nickName}");

        ClosePopupUI();
        SceneManager.LoadScene("MainScene");
    }

    private void OnBackButtonClick()
    {
        ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_StartPopup>("StartPopup");
    }

    private bool IsValidNickName(string nickName, out string errorMessage)
    {
        errorMessage = "";
        
        if (string.IsNullOrWhiteSpace(nickName))
        {
            errorMessage = "닉네임은 비워둘 수 없습니다.";
            return false;
        }
        if (nickName.Contains(" "))
        {
            errorMessage = "닉네임에 공백을 포함할 수 없습니다.";
            return false;
        }
        foreach (char c in nickName)
        {
            if (!char.IsLetterOrDigit(c) && !IsHangul(c))
            {
                errorMessage = "특수문자는 사용할 수 없습니다.";
                return false;
            }
        }
        return true;
    }

    private bool IsHangul(char c)
    {
        return (c >= 0xAC00 && c <= 0xD7A3) || (c >= 0x3131 && c <= 0x318E);
    }
}
