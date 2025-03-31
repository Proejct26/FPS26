using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : UI_Scene
{
    //카메라의 사이즈 값으로 확대기능이 구현이 가능

    public Image playerIcon;  // 미니맵에서 플레이어 아이콘
    public Image enemyIcon;
    
    Transform playerTransform; // 플레이어 Transform (Managers.Player 사용 가능)
    //Transform enemyTransform;

    public RenderTexture miniMapTexture;//미니맵 창


    void Start()
    {
        playerTransform = Managers.Player.gameObject.GetComponent<Transform>();

        OnMiniMapCamere();//미니맵 연동
    }

    void Update()
    {
        // 플레이어의 회전값을 미니맵 아이콘 회전에 적용
        float playerRotation = playerTransform.eulerAngles.y;
        playerIcon.rectTransform.rotation = Quaternion.Euler(0, 0, -playerRotation);

        /*적 아이콘 시야각도 연동
        float enemyRotation = enemyTransform.eulerAngles.y;
        enemyIcon.rectTransform.rotation = Quaternion.Euler(0, 0, -enemyRotation);
        */
    }


    /// <summary>
    /// 미니맵 카메라 생성 메서드
    /// </summary>
    void OnMiniMapCamere()
    {
        GameObject miniMapCamera = new GameObject("MiniMapCam");//오브젝트 생성
        Camera miniMapCam = miniMapCamera.AddComponent<Camera>();//컴포넌트 생성

        miniMapCamera.transform.SetParent(Managers.Player.transform);//카메라 오브젝트 플레이어 자식에 위치변경

        miniMapCamera.transform.localPosition = new Vector3(0, 15f, 0);//카메라 높이
        miniMapCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);//카메라 각도

        miniMapCam.targetTexture = miniMapTexture;//미니맵 창이랑 연동
        miniMapCam.cullingMask = ~(1 << 3) | (1 << 5);//플레이어 오브젝트와 무기 오브젝트 카메라에서 제외
    }

    /// <summary>
    /// 적군 미니맵 표시 메서드
    /// </summary>
    void OnBecameVisible()
    {
        if (gameObject.CompareTag("Player"))
        {
            return; // 플레이어는 인식 없슴
        }


    }
}
