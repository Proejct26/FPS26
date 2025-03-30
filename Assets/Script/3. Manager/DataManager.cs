using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : IManager
{
    private GameSettings _settings = new GameSettings();

    public void Init()
    {
        // 초기화 시 기본값 설정
        _settings = new GameSettings();
    }

    public void Clear()
    {
        _settings = new GameSettings();
    }

    // 데이터를 저장하는 메서드
    public void SettingData(string nickname = null, float? bgmVolume = null, float? sfxVolume = null, float? mouseSensitivity = null)
    {
        if (nickname != null)
            _settings.Nickname = nickname;
        if (bgmVolume.HasValue)
            _settings.BGMVolume = bgmVolume.Value;
        if (sfxVolume.HasValue)
            _settings.SFXVolume = sfxVolume.Value;
        if (mouseSensitivity.HasValue)
            _settings.MouseSensitivity = mouseSensitivity.Value;

        Debug.Log("Settings updated in DataManager");
    }

    // Getter
    public string Nickname => _settings.Nickname;
    public float BGMVolume => _settings.BGMVolume;
    public float SFXVolume => _settings.SFXVolume;
    public float MouseSensitivity => _settings.MouseSensitivity;
}

[System.Serializable]
public class GameSettings
{
    public string Nickname { get; set; }
    public float BGMVolume { get; set; }
    public float SFXVolume { get; set; }
    public float MouseSensitivity { get; set; }

    public GameSettings()
    {
        Nickname = "";
        BGMVolume = 50f;
        SFXVolume = 50f;
        MouseSensitivity = 50f;
    }
}
