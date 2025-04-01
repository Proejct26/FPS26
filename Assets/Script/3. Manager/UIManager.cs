using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class UIManager : IManager
{
    [SerializeField] private Transform _sceneUIParent; 
    [SerializeField] private Transform _popupUIParent; 

    private int _order = 10;

    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    private UI_Scene _sceneUI = null;

    public void Init()
    {

        // 실행 파일의 화면 크기 설정
        Screen.SetResolution(720, 480, false);

        //Managers.UI.ShowSceneUI<MainUI>();  //같은 이름일 경우 텍스트 생략
        
        // UI_Popup popup = Managers.UI.ShowPopupUI<UI_Popup>("StartPopup");
       
       // 씬에 따라 초기 UI 다르게 설정
       string currentScene = SceneManager.GetActiveScene().name;
       if (currentScene == "TitleScene")
       {
           ShowPopupUI<UI_StartPopup>("StartPopup");
       }
       // else if (currentScene == "MainScene")
       // {
       //     ShowSceneUI<MainUI>();
       // }
    } 
  
    public void Clear()
    {
        
    }
    
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
		if (parent != null)
			go.transform.SetParent(parent);

		return Util.GetOrAddComponent<T>(go);
	}

    public T ShowSceneChildUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        go.transform.SetParent(_sceneUIParent.transform, false);
 
        return Util.GetOrAddComponent<T>(go); 
    }

	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
		T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI; 
  
		go.transform.SetParent(_sceneUIParent.transform, false);

		return sceneUI; 
	}

	public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        // if(_popupStack.Count > 0)
        //     ClosePopupUI(_popupStack.Peek()); 

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(_popupUIParent.transform, false);

        popup.Init();

		return popup; 
    }

    public void ClosePopupUI(UI_Popup popup, float time = 0.0f)
    {
		if (_popupStack.Count == 0)
			return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI(time);
    }

    public void ClosePopupUI(float time = 0.0f)
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        GameObject.Destroy(popup.gameObject, time); 
        popup = null; 
        _order--; 
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }
}
