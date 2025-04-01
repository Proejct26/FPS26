using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class WeaponData
{
    [SerializeField] public List<WeaponDataSO> weaponDatas = new List<WeaponDataSO>();

    public List<WeaponDataSO> GetWeaponData(EWeaponType weaponType)
    {
        return weaponDatas.FindAll(data => data.weaponType == weaponType);  
    }
    
    
}

[System.Serializable]
public class GameSettings
{
    public string Nickname { get; set; }
    public float BGMVolume { get; set; }
    public float SFXVolume { get; set; }
    public float MouseSensitivity { get; set; }
    public int SelectedIconIndex { get; set; }

    public GameSettings()
    {
        Nickname = "";
        BGMVolume = 50f;
        SFXVolume = 50f;
        MouseSensitivity = 50f;
        SelectedIconIndex = 0;
    }
}

[Serializable]
public class DataManager : IManager
{
    [SerializeField] private GameSettings _settings = new GameSettings();
    [field: SerializeField] public WeaponData WeaponData {get; private set;} = new WeaponData(); 
 
    private GameSettings _mySettings = new GameSettings();
    
    public void Init()
    {
        _mySettings = new GameSettings();
    }

    public void SettingData(string nickname = null, float? bgmVolume = null, float? sfxVolume = null,
                            float? mouseSensitivity = null, int? selectedIconIndex = null)
    {
        if (nickname != null) _mySettings.Nickname = nickname;
        if (bgmVolume.HasValue) _mySettings.BGMVolume = bgmVolume.Value;
        if (sfxVolume.HasValue) _mySettings.SFXVolume = sfxVolume.Value;
        if (mouseSensitivity.HasValue) _mySettings.MouseSensitivity = mouseSensitivity.Value;
        if (selectedIconIndex.HasValue) _mySettings.SelectedIconIndex = selectedIconIndex.Value;

        Debug.Log($"세팅 데이터 업데이트: 닉네임: {_mySettings.Nickname}, Icon: {_mySettings.SelectedIconIndex}");
    }

    public string Nickname => _mySettings.Nickname;
    public int SelectedIconIndex => _mySettings.SelectedIconIndex;
    public float BGMVolume => _mySettings.BGMVolume;
    public float SFXVolume => _mySettings.SFXVolume;
    public float MouseSensitivity => _mySettings.MouseSensitivity;

    public void Clear()
    {
        _mySettings = new GameSettings();
    }
}
