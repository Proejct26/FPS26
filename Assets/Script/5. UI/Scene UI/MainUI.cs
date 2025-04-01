using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : UI_Scene
{
    public TextMeshProUGUI RoundText;//라운드 텍스트

    public Image HealthBar;//체력바

    public Image WeaponIcon;//무기 아이콘

    //탄창 ui
    public Image MagazineBar;//탄창 게이지
    public TextMeshProUGUI MagazineText;//탄창 텍스트

    private PlayerStatHandler playerStatHandler;
    private PlayerWeaponHandler playerWeaponHandler;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponentInChildren<Crosshair>();
        gameObject.GetComponentInChildren<MiniMap>();
        playerStatHandler = Managers.Player.GetComponent<PlayerStatHandler>();
        playerWeaponHandler = Managers.Player.GetComponent<PlayerWeaponHandler>();

        playerWeaponHandler.OnChangeMagazine += UpdateMagazine;
        playerWeaponHandler.OnChangeWeapon += UpdateWeaponIcon;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerData();
    }

    void UpdatePlayerData()
    {
        //플레이어의 잔탄수등 세부 스텟을 실시간으로 업데이트 하기
        ProgressBar(HealthBar, playerStatHandler.CurrentHealth, playerStatHandler.MaxHealth);
    }

    /// <summary>
    /// 게이지 바 연동 메서드
    /// </summary>
    /// <param name="image"></param>
    /// <param name="curValue"></param>
    /// <param name="maxValue"></param>
    void ProgressBar(Image image, float curValue, float maxValue)
    {
        image.fillAmount = Mathf.Clamp01(curValue / maxValue);
    }

    private void UpdateMagazine(int loadedAmmo, int maxAmmo)
    {
        MagazineBar.fillAmount = (float)loadedAmmo / maxAmmo;
        MagazineText.text = $"{loadedAmmo}/{maxAmmo}"; 
    }

    private void UpdateWeaponIcon()
    {
        WeaponIcon.sprite = playerWeaponHandler.CurrentWeapon.WeaponIcon; 
    }
}
 