using UnityEngine;
using UnityEngine.EventSystems;

// UI 창의 상단(헤더)을 드래그하여 창을 이동시키는 기능을 담당하는 클래스
public class MovableHeaderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // 드래그하여 이동시킬 대상 Transform (설정하지 않으면 부모 객체로 자동 설정됨)
    [SerializeField] private Transform _targetTr;

    // 드래그 시작 시의 대상 위치
    private Vector2 _beginPoint;
    // 드래그 시작 시의 마우스 위치
    private Vector2 _moveBegin;

    // 초기화: 이동 대상이 설정되지 않았으면 부모 객체를 기본 대상으로 지정
    private void Awake()
    {
        if (_targetTr == null)
            _targetTr = transform.parent;
    }

    // 마우스를 누른 순간의 정보 저장
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _beginPoint = _targetTr.position;         // 시작 시점의 UI 위치 저장
        _moveBegin = eventData.position;          // 시작 시점의 마우스 위치 저장
    }

    // 드래그 중일 때 호출됨: UI를 마우스 이동에 따라 따라가도록 위치 조정
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        // 현재 마우스 위치와 시작 위치 차이만큼 UI 이동
        _targetTr.position = _beginPoint + (eventData.position - _moveBegin);
    }
}
