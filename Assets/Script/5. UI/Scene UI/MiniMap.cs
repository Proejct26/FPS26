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

    public Image playerIcon;//플레이어 아이콘

    public Vector3 playerPosition;//플레이어 위치값 저장
    float playerRotation;//플레이어의 시야 각도 값



    void Start()
    {
        
    }

    void Update()
    {
        playerPosition = Managers.Player.gameObject.transform.position;//플레이어 위치값 넣기
        playerRotation = Managers.Player.gameObject.transform.eulerAngles.y;//플레이어의 y축 회전값 넣기

        playerIcon.rectTransform.anchoredPosition = playerPosition;//아이콘 위치를 플레이어 위치랑 연동
        playerIcon.rectTransform.rotation = Quaternion.Euler(0, 0, playerRotation);//플레이어 각도에 따라 아이콘 각도 변경
    }
}
