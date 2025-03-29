using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _customPopup;
    [SerializeField] private GameObject _settingPopup;
    
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _quitButton;
    
    [SerializeField] private Button _enterGameButton;
    [SerializeField] private Button _customBackButton;
    [SerializeField] private Button _settingBackButton;
    [SerializeField] private Button _settingInitButton;
    [SerializeField] private Button _settingSaveButton;
    
    [SerializeField] private TMP_InputField _nickNameInputField;
    [SerializeField] private TextMeshProUGUI _customErrorText;
    
    [SerializeField] private Slider _bgmSlider;       
    [SerializeField] private Slider _sfxSlider;      
    [SerializeField] private Slider _mouseSensitivitySlider;
    
    [SerializeField] private TMP_Text _bgmValueText; 
    [SerializeField] private TMP_Text _sfxValueText; 
    [SerializeField] private TMP_Text _mouseSensitivityValueText;
    
    private const float DEFAULT_BGM = 80f;
    private const float DEFAULT_SFX = 50f;
    private const float DEFAULT_MOUSE_SENSITIVITY = 50f;
    
    private void Start()
    {
        InitCanvas();
        SetInitPanelState();
        BindButtonEvents();
        LoadNickname();
        
        // 사운드 테스트용
        StartCoroutine(PlayBgmAfterDelay());
    }
    
    // 사운드 테스트용
    private IEnumerator PlayBgmAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        Managers.Sound.Play("BgmTest", 0.8f, Define.Sound.Bgm);
    }

    private void InitCanvas()
    {
        Managers.UI.SetCanvas(gameObject, false);
    }

    private void SetInitPanelState()
    {
        _startPanel.SetActive(true);
        _customPopup.SetActive(false);
        _settingPopup.SetActive(false);
    }

    private void BindButtonEvents()
    {
        // 버튼 이벤트 연결
        _startButton.onClick.AddListener(OpenCustomPopup);
        _settingButton.onClick.AddListener(OpenSettingPopup);
        _quitButton.onClick.AddListener(QuitGame);
        _enterGameButton.onClick.AddListener(OpenEnterGame);
        _customBackButton.onClick.AddListener(BackToStart);
        _settingBackButton.onClick.AddListener(BackToStart);
        _settingInitButton.onClick.AddListener(LoadSettings);
        _settingSaveButton.onClick.AddListener(SaveSettings);
        
        // 슬라이더 이벤트 연결
        _bgmSlider.onValueChanged.AddListener(UpdateBgmVolume);
        _sfxSlider.onValueChanged.AddListener(UpdateSfxVolume);
        _mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
    }
    
    private void LoadNickname()
    {
        _nickNameInputField.text = PlayerPrefs.GetString("PlayerNickName", "");
    }
    
    private void OpenCustomPopup()
    {
        _startPanel.SetActive(false);
        _customPopup.SetActive(true);
        _customErrorText.gameObject.SetActive(false);
        // Managers.Sound.Play("Sfx/Click");
    }

    public void OpenSettingPopup()
    {
        _startPanel.SetActive(false);
        _settingPopup.SetActive(true);
        // Managers.Sound.Play("Sfx/Click");
    }
    
    private void BackToStart()
    {
        _startPanel.SetActive(true);
        _customPopup.SetActive(false);
        _settingPopup.SetActive(false);
        // Managers.Sound.Play("Sfx/Click");
    }

    private void OpenEnterGame()
    {
        string nickName = _nickNameInputField.text.Trim();

        if (!IsValidNickName(nickName, out string errorMessage))
        {
            _customErrorText.text = errorMessage;
            _customErrorText.gameObject.SetActive(true);
            return;
        }
        
        PlayerPrefs.SetString("PlayerNickName", nickName);
        PlayerPrefs.Save();
        Debug.Log($"NickName 설정: {nickName}");
        
        Debug.Log("Enter Game");
        BackToStart();
        // Managers.Event.TriggerEvent("GameStart");
        // Managers.Sound.Play("Sfx/Click");
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
                errorMessage = "특수문자 / 띄어쓰기는 사용할 수 없습니다.";
                return false;
            }
        }
        
        return true;
    }
    
    private bool IsHangul(char c)
    {
        return (c >= 0xAC00 && c <= 0xD7A3) || (c >= 0x3131 && c <= 0x318E);
    }

    private void QuitGame() // 테스트용
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        // Managers.Instance.QuitGame();
    }
    
    private void LoadSettings()
    {
        // 저장된 값 로드 (기본값 설정)
        _bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", DEFAULT_BGM);
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", DEFAULT_SFX);
        _mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", DEFAULT_MOUSE_SENSITIVITY);

        // 초기 값 적용
        UpdateBgmVolume(_bgmSlider.value);
        UpdateSfxVolume(_sfxSlider.value);
        UpdateMouseSensitivity(_mouseSensitivitySlider.value);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", _bgmSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _sfxSlider.value);
        PlayerPrefs.SetFloat("MouseSensitivity", _mouseSensitivitySlider.value);
        PlayerPrefs.Save();
        Debug.Log("설정 저장됨");
        BackToStart();
    }

    private void ResetSettings()
    {
        _bgmSlider.value = DEFAULT_BGM;
        _sfxSlider.value = DEFAULT_SFX;
        _mouseSensitivitySlider.value = DEFAULT_MOUSE_SENSITIVITY;

        // 초기화 후 값 적용
        UpdateBgmVolume(DEFAULT_BGM);
        UpdateSfxVolume(DEFAULT_SFX);
        UpdateMouseSensitivity(DEFAULT_MOUSE_SENSITIVITY);
    }

    private void UpdateBgmVolume(float value)
    {
        //Managers.Sound.SetVolume(Define.Sound.Bgm, value / 100f); // BGM (0번 AudioSource)
        _bgmValueText.text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateSfxVolume(float value)
    {
        //Managers.Sound.SetVolume(Define.Sound.Effect, value / 100f); // SFX (1번 AudioSource)
        _sfxValueText.text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateMouseSensitivity(float value)
    {
        // 마우스 감도는 일단 저장만..
        _mouseSensitivityValueText.text = Mathf.RoundToInt(value).ToString();
    }
}
