using UnityEngine;
using UnityEngine.UI;

public class Crosshair : UI_Scene
{
    /*ui 만들때 주의점
    ui 기능 스크립트들은 UI_Scene 상속 받아야 한다
    MainUI 에 해당기능에 접근할수잇게 gameObject.GetComponentInChildren 로 길을 만들어줘야한다
    */

    public Image crosshairUp; //크로스 헤어 이미지
    public Image crosshairDown;
    public Image crosshairRight;
    public Image crosshairLeft;

    public float SpreadAmount => currentWeapon != null && currentWeapon is GunController ? (currentWeapon as GunController).Spread: 0f; //크로스 헤어 벌어지는 정도 (반동)
 
    private RectTransform crosshairUpRect; // 크로스헤어 UI의 RectTransform 변수
    private RectTransform crosshairDownRect;
    private RectTransform crosshairRightRect;
    private RectTransform crosshairLeftRect;

    private Vector2 center;//크로스 헤어의 기본위치 값 저장 겸 스프레이를 위한 중앙값

    //크로스 헤어 퍼짐 값
    private Vector2 directionY;
    private Vector2 directionX;

    private PlayerWeaponHandler playerWeaponHandler;
    private WeaponBaseController currentWeapon => playerWeaponHandler.CurrentWeapon;

    private void Awake()
    {
        playerWeaponHandler = FindFirstObjectByType<PlayerWeaponHandler>(); 
    }

    private void Start()
    {
        crosshairUpRect = crosshairUp.GetComponent<RectTransform>();//컴포넌트 가져오기
        crosshairDownRect = crosshairDown.GetComponent<RectTransform>();//컴포넌트 가져오기
        crosshairRightRect = crosshairRight.GetComponent<RectTransform>();//컴포넌트 가져오기
        crosshairLeftRect = crosshairLeft.GetComponent<RectTransform>();//컴포넌트 가져오기

        //크로스 헤어의 중앙값
        center = (crosshairUpRect.anchoredPosition + crosshairDownRect.anchoredPosition +
                  crosshairRightRect.anchoredPosition + crosshairLeftRect.anchoredPosition) / 4;

        //크로스 헤어 퍼짐을위한 기본값
        //2개의 포지션값을 빼면 0 .normalized 써서 1을 만들고 그값으로 반동이랑 연동
        directionY = (crosshairUpRect.anchoredPosition - crosshairDownRect.anchoredPosition).normalized;
        directionX = (crosshairRightRect.anchoredPosition - crosshairLeftRect.anchoredPosition).normalized;
    }

    void Update()
    {
        Spread(center, directionY, directionX);
    }


    /// <summary>
    /// 플레이어의 움직임과 총기 발사 상태에 따라 크로스헤어 확대 연산 값 지정
    /// </summary>
    /// <returns></returns>
    float GetSpreadFactor()
    {
        if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼(총기 발사)
            return 1f;


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))//이동키중 아무 입력
            return 0.5f; // 이동 시 약간 확대

        return 0f; // 기본 크기
    }


    /// <summary>
    /// 크로스 헤어 퍼짐 연출 메서드
    /// </summary>
    /// <param name="center"></param>
    /// <param name="directionY"></param>
    /// <param name="directionX"></param>
    /// <param name="spread"></param>
    void Spread(Vector2 center, Vector2 directionY, Vector2 directionX) 
    {
      //  float spread = Mathf.Lerp(0, SpreadAmount, GetSpreadFactor());//크로스 헤어 퍼짐 계산 
        float spread = SpreadAmount / 4; 
        Debug.Log(spread);
        crosshairUpRect.anchoredPosition = center + directionY * spread;   // 위로 퍼짐
        crosshairDownRect.anchoredPosition = center - directionY * spread; // 아래로 퍼짐
        crosshairRightRect.anchoredPosition = center + directionX * spread; // 오른쪽으로 퍼짐
        crosshairLeftRect.anchoredPosition = center - directionX * spread;  // 왼쪽으로 퍼짐
    }
}
