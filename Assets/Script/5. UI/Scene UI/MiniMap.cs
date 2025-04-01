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

        InvokeRepeating("UpdatePlayerList", 0.5f, 0.5f);
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


    void UpdatePlayerList()
    {
        GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("LocalPlayer");
        if (currentPlayers.Length != allPlayers?.Length)
        {
            allPlayers = currentPlayers;
            Debug.Log(allPlayers.Length);
            SpawnEnemyIcons();
        }
    }

    void SpawnEnemyIcons()
    {
        foreach (var icon in enemyIcons)
        {
            Destroy(icon);
        }
        enemyIcons.Clear();

        foreach (var player in allPlayers)
        {
            GameObject enemyIcon = Instantiate(playerIconPrefab);
            enemyIcon.transform.position = new Vector3(player.transform.position.x, 35f, player.transform.position.z);
            enemyIcon.transform.rotation = Quaternion.Euler(90, 0, 0);
            enemyIcon.SetActive(true);
            enemyIcons.Add(enemyIcon);
        }
    }

    void LocalPlayIconUpdata()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            RemotePlayerController localplayer = allPlayers[i].GetComponentInChildren<RemotePlayerController>(true);

            Transform localPlayerIcon = enemyIcons[i].transform;

            localPlayerIcon.position = new Vector3(localplayer.transform.position.x, 35f, localplayer.transform.position.z);
            localPlayerIcon.rotation = Quaternion.Euler(90, localplayer.transform.eulerAngles.y, 0);
        }
    }
}
