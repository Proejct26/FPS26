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
        SpawnEnemyIcons();//멀티 플레이 아이콘 생성

        InvokeRepeating("LocalPlayIconUpdata", 0.5f, 0.1f);
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
    /// 로컬 플레이어 만큼 아이콘 생성
    /// </summary>
    void SpawnEnemyIcons()
    {
        // 모든 로컬 플레이어들 가져오기
        allPlayers = GameObject.FindGameObjectsWithTag("LocalPlayer");

        foreach (var player in allPlayers)
        {
            // 각 플레이어만큼 아이콘 복제
            GameObject enemyIcon = Instantiate(playerIconPrefab);

            enemyIcon.transform.position = new Vector3(player.transform.position.x, 35f, player.transform.position.z);
            enemyIcon.transform.rotation = Quaternion.Euler(90, 0, 0);

            enemyIcon.SetActive(true); // 기본적으로 활성화 상태로 설정
            enemyIcons.Add(enemyIcon); // 리스트에 보관
        }
    }

    void LocalPlayIconUpdata()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            RemotePlayerController localplayer = allPlayers[i].GetComponentInChildren<RemotePlayerController>();
            GameObject localplayerObj = localplayer.gameObject;

            Transform localPlayerIcon = enemyIcons[i].transform;

            localPlayerIcon.position = new Vector3(localplayerObj.transform.position.x, 35f, localplayerObj.transform.position.z);
            localPlayerIcon.rotation = Quaternion.Euler(90, localplayerObj.transform.eulerAngles.y, 0);
        }
    }
}
