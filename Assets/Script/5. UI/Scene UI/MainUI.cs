using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : UI_Scene
{
    public TextMeshProUGUI redroundText;//레드팀 승리 카운트 텍스트
    public TextMeshProUGUI blueroundText;//블루팀 승리 카운트 텍스트

    public Image healthBar;//체력바

    public Image weaponIcon;//무기 아이콘

    

    //탄창 ui
    public Image magazineBar;//탄창 게이지
    public TextMeshProUGUI magazineText;//탄창 텍스트

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
        Managers.GameSceneManager.OnChangedTeamScore += UpdateTeamScore;
        playerStatHandler.OnHealthChanged += UpdatePlayerData; 
        UpdateTeamScore();

    }

    // Update is called once per frame


    void UpdatePlayerData()
    {
        //플레이어의 잔탄수등 세부 스텟을 실시간으로 업데이트 하기
        ProgressBar(healthBar, playerStatHandler.CurrentHealth, playerStatHandler.MaxHealth);
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
        magazineBar.fillAmount = (float)loadedAmmo / maxAmmo;
        magazineText.text = $"{loadedAmmo}/{maxAmmo}"; 
    }

    private void UpdateWeaponIcon()
    {
        weaponIcon.sprite = playerWeaponHandler.CurrentWeapon.WeaponIcon; 
    }

    private void UpdateTeamScore()
    {
        redroundText.text = Managers.GameSceneManager.RedTeamScore.ToString();
        blueroundText.text = Managers.GameSceneManager.BlueTeamScore.ToString(); 
    }
}
 