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
    private GameObject[] allPlayers;//현재 플레이어 정보


    public RenderTexture miniMapTexture;//미니맵 창
    private Camera miniMapCam;//미니맵 카메라

    public GameObject playerIconPrefab; // 미니맵 아이콘 프리팹
    private List<GameObject> enemyIcons = new List<GameObject>(); // 적 아이콘 리스트

    void Start()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        OnMiniMapCamere();//미니맵 카메라 생성

        InvokeRepeating("UpdatePlayerList", 0.5f, 0.5f);//실시간 플레이어 감지
        InvokeRepeating("LocalPlayIconUpdata", 0.5f, 0.1f);//실시간 아이콘 위치 연동
    }


    /// <summary>
    /// 미니맵 카메라 생성 메서드
    /// </summary>
    void OnMiniMapCamere()
    {
        GameObject miniMapCamera = new GameObject("MiniMapCam");//오브젝트 생성
        miniMapCam = miniMapCamera.AddComponent<Camera>();//컴포넌트 생성
        miniMapCamera.transform.SetParent(playerTransform);//카메라 오브젝트 플레이어 자식에 위치변경

        miniMapCamera.transform.localPosition = new Vector3(0, 45f, 0);//카메라 높이
        miniMapCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);//카메라 각도

        miniMapCam.orthographic = true;

        miniMapCam.targetTexture = miniMapTexture;//미니맵 창이랑 연동
        miniMapCam.cullingMask = ~(1 << 3) & ~(1 << 6) & ~(1 << 8);//플레이어 오브젝트와 무기 오브젝트 카메라에서 제외
    }

    /// <summary>
    /// 플레이어 실시간 감지
    /// </summary>
    void UpdatePlayerList()
    {
        GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("LocalPlayer");//생성된 플레이어 배열화
        if (currentPlayers.Length != allPlayers?.Length)//현재 플레이어와 최신플레이어 목록이 다를시
        {
            allPlayers = currentPlayers;//최신플레이어 데이터을 현재 플레이어 데이터 덮어쓰기? (현재 = 최신)

            Debug.Log(allPlayers.Length);//현재 플레이어 목록수 확인
            SpawnEnemyIcons();//아이콘 실시간 관리
        }
    }


    /// <summary>
    /// 아이콘 실시간 플레이어의 개수에 맞추기
    /// </summary>
    void SpawnEnemyIcons()
    {
        foreach (var icon in enemyIcons)
        {
            Destroy(icon);//아이콘 삭제
        }
        enemyIcons.Clear();//리스트 데이터 초기화

        foreach (var player in allPlayers)//현재 플레이어만큼 아이콘 생성
        {
            GameObject enemyIcon = Instantiate(playerIconPrefab);//복제
            enemyIcon.transform.position = new Vector3(player.transform.position.x, 35f, player.transform.position.z);//아이콘 위치 세팅
            enemyIcon.transform.rotation = Quaternion.Euler(90, 0, 0);//아이콘 각도 세팅
            enemyIcon.SetActive(true);
            enemyIcons.Add(enemyIcon);//아이콘 오브젝트 리스트 등록
        }
    }

    /// <summary>
    /// 아이콘 과 플레이어 위치,각도 연동 
    /// </summary>
    void LocalPlayIconUpdata()
    {
        for (int i = 0; i < allPlayers.Length; i++)//현재 플레이어 
        {
            RemotePlayerController localplayer = allPlayers[i].GetComponentInChildren<RemotePlayerController>(true);//현재 플레이어 위치값 꺼내기
            if (allPlayers[i].gameObject == null) return;

            Transform localPlayerIcon = enemyIcons[i].transform;//아이콘 위치 변수 지정

            localPlayerIcon.position = new Vector3(localplayer.transform.position.x, 35f, localplayer.transform.position.z);//위치 실시간 세팅
            localPlayerIcon.rotation = Quaternion.Euler(90, localplayer.transform.eulerAngles.y, 0);//플레이어들의 시야 각도 아이콘에 연동
        }
    }
}
