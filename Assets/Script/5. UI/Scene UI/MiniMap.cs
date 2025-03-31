using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : UI_Scene
{
    public Image playerIcon;  // 미니맵에서 플레이어 아이콘

    Transform playerTransform; // 플레이어 Transform (Managers.Player 사용 가능)
    private GameObject[] allPlayers;//모든 플레이어 정보


    public RenderTexture miniMapTexture;//미니맵 창
    Camera miniMapCam;//미니맵 카메라

    public GameObject playerIconPrefab; // 미니맵 아이콘 프리팹
    private List<GameObject> enemyIcons = new List<GameObject>(); // 적 아이콘 리스트

    private RectTransform miniMapRect; // 미니맵의 RectTransform (미니맵 크기 확인 용도)

    
    void Start()
    {
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        OnMiniMapCamere(); //미니맵 카메라 생성
        SpawnEnemyIcons();//멀티 플레이 아이콘 세팅
    }

    void Update()
    {
        if (miniMapCam != null) //플레이어 아이콘 각도 연동
        {
            float playerRotation = miniMapCam.gameObject.transform.rotation.z;
            playerIcon.rectTransform.rotation = Quaternion.Euler(0, 0, playerRotation);
        }

        OnLocalPlayerIcon();
    }
    /// <summary>
    /// 미니맵 카메라 생성 메서드
    /// </summary>
    void OnMiniMapCamere()
    {
        Debug.Log("OnMiniMapCamere 실행: 미니맵 카메라 생성 시작");

        GameObject miniMapCamera = new GameObject("MiniMapCam");//오브젝트 생성
        miniMapCam = miniMapCamera.AddComponent<Camera>();//컴포넌트 생성
        miniMapCamera.transform.SetParent(playerTransform);//카메라 오브젝트 플레이어 자식에 위치변경

        miniMapCamera.transform.localPosition = new Vector3(0, 15f, 0);//카메라 높이
        miniMapCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);//카메라 각도

        miniMapCam.targetTexture = miniMapTexture;//미니맵 창이랑 연동
        miniMapRect = miniMapTexture.GetComponent<RectTransform>();
        miniMapCam.cullingMask = ~(1 << 3) | (1 << 5);//플레이어 오브젝트와 무기 오브젝트 카메라에서 제외

    }
    /// <summary>
    /// 로컬 플레이어 아이콘 생성
    /// </summary>
    void SpawnEnemyIcons()
    {
        // 모든 로컬 플레이어들 가져오기
        allPlayers = GameObject.FindGameObjectsWithTag("LocalPlayer"); // 플레이어 다수 체크

        foreach (var player in allPlayers)
        {
            if (player != playerTransform)//자신을 제외한 모든 플레이어
            {
                // 각 플레이어 만큼 아이콘 복제
                GameObject enemyIcon = Instantiate(playerIconPrefab);//로컬 플레이어 아이콘 복제
                enemyIcon.transform.SetParent(gameObject.transform); //로컬 플레이어 아이콘 세팅
                enemyIcon.SetActive(false); // 아이콘 비활성화
                enemyIcons.Add(enemyIcon); // 아이콘 리스트에 추가
            }
        }
    }

    /// <summary>
    /// 모든 적 아이콘의 위치 업데이트
    /// </summary>
    void OnLocalPlayerIcon()
    {
        for (int i = 0; i < allPlayers.Length; i++) // 모든 플레이어 지정
        {
            if (allPlayers[i] != playerTransform) // 로컬 플레이어는 제외
            {
                GameObject enemyIcon = enemyIcons[i]; // 아이콘 1개 변수 지정

                Vector3 playerPos = allPlayers[i].transform.position; // 로컬플레이어 [i]의 위치 (반복문으로 전체의 위치값)

                // 미니맵 카메라에서 플레이어 위치를 2D 화면 좌표로 변환
                Vector3 viewportPos = miniMapCam.WorldToViewportPoint(playerPos);

                // 미니맵 카메라 범위 안에 있을 때만 아이콘을 표시
                if (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
                {
                    // 아이콘 활성화
                    enemyIcon.SetActive(true);

                    // 미니맵에서 2D 좌표로 변환
                    //Vector3 localMapPos = new Vector3(viewportPos.x * miniMapRect.rect.width, viewportPos.y * miniMapRect.rect.height, 0f);

                    // 미니맵 UI의 아이콘 위치로 대입
                    enemyIcon.transform.localPosition = playerPos;
                }
                else
                {
                    // 카메라 범위 밖에 있으면 아이콘 비활성화
                    enemyIcon.SetActive(false);
                }
            }
        }
    }
}