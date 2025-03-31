using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : UI_Popup 
{
    enum Texts
    {
        WeaponTypeText,
        WeaponNameText,
        damageText,
        fireRateText,
        RecoilText,
    }

    enum Images
    {
        WeaponIcon,
    }

    enum Buttons
    {
        CloseButton,
        SelectButton,
    }

    private WeaponDataSO _weaponData;

    private void Awake()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images)); 
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(ClosePopupUI);
        GetButton((int)Buttons.SelectButton).onClick.AddListener(SelectWeapon);
    } 
 
    public void InitWeaponInfo(WeaponDataSO weaponData)
    {
        _weaponData = weaponData;
        GetText((int)Texts.WeaponTypeText).text = _weaponData.weaponType.ToString();
        //GetText((int)Texts.WeaponNameText).text = _weaponData.weaponName;

        int damageLevel = Mathf.Clamp(_weaponData.damage * 2, 10, 100) / 10;   
        // 발사 속도 레벨 계산 (0.1초 이하: 10점, 1.5초 이상: 0점)
        float fireRateScore = Mathf.InverseLerp(1.5f, 0.1f, _weaponData.attackDelay);
        int fireRateLevel = Mathf.RoundToInt(fireRateScore * 10);

        // 반동 점수 계산 (recoilAmount: 3 이하 10점, 30 이상 1점)
        float recoilAmountScore = Mathf.InverseLerp(30f, 3f, _weaponData.recoilSettings.recoilAmount);
        recoilAmountScore = Mathf.Clamp01(recoilAmountScore) * 10;

        // 탄퍼짐 점수 계산 (originBulletSpread: 0.3 이하 10점, 10 이상 1점)
        float spreadScore = Mathf.InverseLerp(10f, 0.3f, _weaponData.spreadSettings.originBulletSpread);
        spreadScore = Mathf.Clamp01(spreadScore) * 10;

        // 최종 반동 레벨 계산 (두 점수의 평균)
        int recoilLevel = Mathf.RoundToInt((recoilAmountScore + spreadScore) / 2f);

        Get<TextMeshProUGUI>((int)Texts.damageText).text = damageLevel.ToString();
        Get<TextMeshProUGUI>((int)Texts.fireRateText).text = fireRateLevel.ToString();
        Get<TextMeshProUGUI>((int)Texts.RecoilText).text = recoilLevel.ToString();  

        GetImage((int)Images.WeaponIcon).sprite = _weaponData.weaponIcon; 
    } 

    private void SelectWeapon()
    {
        
    }


}
