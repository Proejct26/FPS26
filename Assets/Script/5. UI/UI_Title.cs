using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject customPopup;
    public GameObject settingPopup;
    
    public Button startButton;
    public Button settingButton;
    public Button quitButton;
    public Button enterGameButton;
    public Button customBackButton;
    public Button settingBackButton;
    public Button settingInitButton;
    public Button settingSaveButton;
    
    void Start()
    {
        startPanel.SetActive(true);
        customPopup.SetActive(false);
        settingPopup.SetActive(false);
        
        startButton.onClick.AddListener(OpenCustomPopup);
        settingButton.onClick.AddListener(OpenSettingPopup);
        quitButton.onClick.AddListener(QuitGame);
        enterGameButton.onClick.AddListener(OpenEnterGame);
        customBackButton.onClick.AddListener(BackToStart);
        settingBackButton.onClick.AddListener(BackToStart);
        settingInitButton.onClick.AddListener(BackToStart);
        settingSaveButton.onClick.AddListener(BackToStart);
    }
    
    void OpenCustomPopup()
    {
        startPanel.SetActive(false);
        customPopup.SetActive(true);
    }

    void OpenSettingPopup()
    {
        startPanel.SetActive(false);
        settingPopup.SetActive(true);
    }
    
    void BackToStart()
    {
        startPanel.SetActive(true);
        customPopup.SetActive(false);
        settingPopup.SetActive(false);
    }

    void OpenEnterGame()
    {
        Debug.Log("Enter Game");
        BackToStart();
    }

    void QuitGame() // 테스트용
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        // Managers.Instance.QuitGame();
    }
}
