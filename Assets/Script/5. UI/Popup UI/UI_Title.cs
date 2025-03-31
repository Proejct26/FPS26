using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Title : UI_Scene
{
    public override void Init()
    {
        base.Init();
        Managers.UI.ShowPopupUI<UI_StartPopup>("StartPopup");

        StartCoroutine(PlayBgmAfterDelay());
    }

    private IEnumerator PlayBgmAfterDelay()
    {
        while (Managers.Sound == null)
        {
            Debug.Log("잠시 대기");
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        Debug.Log("테스트 BGM 재생");
        Managers.Sound.Play("BgmTest", 0.8f, Define.Sound.Bgm);
    }
}
