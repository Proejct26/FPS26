using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : UI_Scene
{
    //다른 플레이어도 이 스크립트를 가지고 있을까?
    //가지고 있을때는 자신을 뺀 모든플레이어의 위치를 미니맵에 표시해야한다

    //카메라의 사이즈 값으로 확대기능이 구현이 가능

    public Image playerIcon;  // 미니맵에서 플레이어 아이콘
    public GameObject enemyIconPrefab;//적군 아이콘

    Transform playerTransform; // 플레이어 Transform (Managers.Player 사용 가능)

    public RenderTexture miniMapTexture;//미니맵 창
    Camera miniMapCam;//미니맵 카메라


    void Start()
    {
        playerTransform = Managers.Player.gameObject.GetComponent<Transform>();

        OnMiniMapCamere();
    }

    void Update()
    {
        // 플레이어의 회전값을 미니맵 아이콘 회전에 적용
        float playerRotation = playerTransform.eulerAngles.y;
        playerIcon.rectTransform.rotation = Quaternion.Euler(0, 0, -playerRotation);

        OnLocalPlayerIcon();

    }
    /// <summary>
    /// 미니맵 카메라 생성 메서드
    /// </summary>
    void OnMiniMapCamere()
    {
        GameObject miniMapCamera = new GameObject("MiniMapCam");//오브젝트 생성

        miniMapCam = miniMapCamera.AddComponent<Camera>();//컴포넌트 생성

        miniMapCamera.transform.SetParent(Managers.Player.transform);//카메라 오브젝트 플레이어 자식에 위치변경

        miniMapCamera.transform.localPosition = new Vector3(0, 15f, 0);//카메라 높이
        miniMapCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);//카메라 각도

        miniMapCam.targetTexture = miniMapTexture;//미니맵 창이랑 연동
        miniMapCam.cullingMask = ~(1 << 3) | (1 << 5);//플레이어 오브젝트와 무기 오브젝트 카메라에서 제외
    }

    void OnLocalPlayerIcon()
    {
        //다른 객체인가?
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in allPlayers)
        {
            // 자신은 제외하고 적 플레이어만 처리
            if (player != Managers.Player.gameObject)
            {
                Transform localPlayerPos = Managers.Player.gameObject.transform;

                Instantiate(playerIcon,localPlayerPos);
            }
        }
    }

}