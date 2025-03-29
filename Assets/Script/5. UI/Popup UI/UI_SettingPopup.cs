using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SettingPopup : UI_Popup
{
    enum Sliders
    {
        BgmSlider,
        SfxSlider,
        MouseSensitivitySlider
    }

    enum Buttons
    {
        SettingBackButton,
        SettingInitButton,
        SettingSaveButton
    }

    enum Texts
    {
        BgmValueText,
        SfxValueText,
        MouseSensitivityValueText
    }

    private const float DEFAULT_BGM = 50f;
    private const float DEFAULT_SFX = 50f;
    private const float DEFAULT_MOUSE_SENSITIVITY = 50f;

    public override void Init()
    {
        base.Init();
        Bind<Slider>(typeof(Sliders));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetButton((int)Buttons.SettingBackButton).onClick.AddListener(OnBackButtonClick);
        GetButton((int)Buttons.SettingInitButton).onClick.AddListener(OnInitButtonClick);
        GetButton((int)Buttons.SettingSaveButton).onClick.AddListener(OnSaveButtonClick);

        Get<Slider>((int)Sliders.BgmSlider).onValueChanged.AddListener(UpdateBgmVolume);
        Get<Slider>((int)Sliders.SfxSlider).onValueChanged.AddListener(UpdateSfxVolume);
        Get<Slider>((int)Sliders.MouseSensitivitySlider).onValueChanged.AddListener(UpdateMouseSensitivity);

        LoadSettings();
    }

    private void LoadSettings()
    {
        Get<Slider>((int)Sliders.BgmSlider).value = Managers.Data.BGMVolume;
        Get<Slider>((int)Sliders.SfxSlider).value = Managers.Data.SFXVolume;
        Get<Slider>((int)Sliders.MouseSensitivitySlider).value = Managers.Data.MouseSensitivity;

        UpdateBgmVolume(Get<Slider>((int)Sliders.BgmSlider).value);
        UpdateSfxVolume(Get<Slider>((int)Sliders.SfxSlider).value);
        UpdateMouseSensitivity(Get<Slider>((int)Sliders.MouseSensitivitySlider).value);
    }

    private void OnBackButtonClick()
    {
        ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_StartPopup>("StartPopup");
    }

    private void OnInitButtonClick()
    {
        Get<Slider>((int)Sliders.BgmSlider).value = DEFAULT_BGM;
        Get<Slider>((int)Sliders.SfxSlider).value = DEFAULT_SFX;
        Get<Slider>((int)Sliders.MouseSensitivitySlider).value = DEFAULT_MOUSE_SENSITIVITY;

        UpdateBgmVolume(DEFAULT_BGM);
        UpdateSfxVolume(DEFAULT_SFX);
        UpdateMouseSensitivity(DEFAULT_MOUSE_SENSITIVITY);
    }

    private void OnSaveButtonClick()
    {
        float bgmVolume = Get<Slider>((int)Sliders.BgmSlider).value;
        float sfxVolume = Get<Slider>((int)Sliders.SfxSlider).value;
        float mouseSensitivity = Get<Slider>((int)Sliders.MouseSensitivitySlider).value;

        Managers.Data.SettingData(bgmVolume: bgmVolume, sfxVolume: sfxVolume, mouseSensitivity: mouseSensitivity);
        Debug.Log("설정 저장됨");

        ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_StartPopup>("StartPopup");
    }

    private void UpdateBgmVolume(float value)
    {
        Managers.Sound.SetVolume(Define.Sound.Bgm, value / 100f);
        Get<TextMeshProUGUI>((int)Texts.BgmValueText).text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateSfxVolume(float value)
    {
        Managers.Sound.SetVolume(Define.Sound.Effect, value / 100f);
        Get<TextMeshProUGUI>((int)Texts.SfxValueText).text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateMouseSensitivity(float value)
    {
        Get<TextMeshProUGUI>((int)Texts.MouseSensitivityValueText).text = Mathf.RoundToInt(value).ToString();
    }
}
