using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : UI_Scene
{
    /* 스크립트 적용법
    1.카메라 생성후 로테이션 x값을 90설정후 수직으로 만든다
    2.ui 캔버스안에서 Raw Image 생성 (이게 미니맵 창이 될 예정)
    3.Raw Image 텍스쳐 옵션 에서 만들어둔 MiniMapRen 텍스쳐 지정
    4.Raw Image 하위 오브젝트로 이미지 생성후 플레이어 커서 아이콘 추가
    5.만든 카메라에  Target Texture 에 만든 MiniMapRen 추가

    이후에 플레이어 만들어질때 test 해보기
    */


    //카메라의 사이즈 값으로 확대기능이 구현이 가능

    public Image playerIcon;  // 미니맵에서 플레이어 아이콘
    public RectTransform minimapRect; // 미니맵 UI의 RectTransform
    public Transform playerTransform; // 플레이어 Transform (Managers.Player 사용 가능)
    public Transform minimapCenter; // 미니맵의 기준점 (예: 미니맵 카메라 위치)
    public float minimapScale = 0.1f; // 월드 좌표와 미니맵 크기 비율



    void Start()
    {
        
    }

    void Update()
    {
        if (playerTransform == null || minimapCenter == null) return;

        // 플레이어의 월드 위치 가져오기
        Vector3 playerPos = playerTransform.position;

        // 미니맵 기준점(중앙)과의 상대 위치 계산 (X, Z만 사용)
        Vector2 relativePos = new Vector2(playerPos.x - minimapCenter.position.x,
                                          playerPos.z - minimapCenter.position.z);

        // 미니맵 크기 비율 적용하여 UI 위치 조정
        Vector2 minimapPos = relativePos * minimapScale;

        // 미니맵 UI에서 아이콘 위치 설정
        playerIcon.rectTransform.anchoredPosition = minimapPos;

        // 플레이어의 회전값을 미니맵 아이콘 회전에 적용
        float playerRotation = playerTransform.eulerAngles.y;
        playerIcon.rectTransform.rotation = Quaternion.Euler(0, 0, -playerRotation);
    }
}
