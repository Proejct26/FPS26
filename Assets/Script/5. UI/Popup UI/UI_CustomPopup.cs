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
        CustomBackButton,
        IconButton1,
        IconButton2,
        IconButton3,
        IconButton4,
    }

    enum InputFields
    {
        NickNameInputField
    }

    enum Texts
    {
        CustomErrorText
    }

    private int _selectedIconIndex = 0;
    private Button[] _iconButtons;
    private Image[] _outlineImages;

    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<TextMeshProUGUI>(typeof(Texts));
        
        GetButton((int)Buttons.EnterGameButton).onClick.AddListener(OnEnterGameButtonClick);
        GetButton((int)Buttons.CustomBackButton).onClick.AddListener(OnBackButtonClick);
        
        _outlineImages = new Image[4];
        
        _iconButtons = new Button[4];
        _iconButtons[0] = GetButton((int)Buttons.IconButton1);
        _iconButtons[1] = GetButton((int)Buttons.IconButton2);
        _iconButtons[2] = GetButton((int)Buttons.IconButton3);
        _iconButtons[3] = GetButton((int)Buttons.IconButton4);
        
        // 아웃라인 이미지 초기화
        for (int i = 0; i < _iconButtons.Length; i++)
        {
            if (_iconButtons[i] == null)
            {
                Debug.Log($"IconButton{i + 1} is null");
                continue;
            }
            _outlineImages[i] = _iconButtons[i].transform.Find("OutlineImage")?.GetComponent<Image>();
            if (_outlineImages[i] == null)
            {
                Debug.Log($"IconButton{i + 1}의 OutlineImage 누락");
                continue;
            }
            int index = i;
            _iconButtons[i].onClick.AddListener(() => SelectIcon(index));
        }
        
        Get<TextMeshProUGUI>((int)Texts.CustomErrorText).gameObject.SetActive(false);
        Get<TMP_InputField>((int)InputFields.NickNameInputField).text = Managers.Data.Nickname;
        _selectedIconIndex = Managers.Data.SelectedIconIndex;
        
        UpdateIconSelectionUI();
    }

    private void SelectIcon(int index)
    {
        _selectedIconIndex = index;
        UpdateIconSelectionUI();
    }
    
    private void UpdateIconSelectionUI()
    {
        for (int i = 0; i < _iconButtons.Length; i++)
        {
            if (_outlineImages[i] != null)
            {
                _outlineImages[i].color = (i == _selectedIconIndex) ? Color.green : Color.gray;
            }
        }
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
        
        Managers.Data.SettingData(nickname: nickName, selectedIconIndex: _selectedIconIndex);
        Debug.Log($"닉네임 설정: {nickName}, Icon: {_selectedIconIndex}");

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
