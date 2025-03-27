using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

// 슬롯 위에 마우스를 올렸을 때 표시되는 아이템 툴팁 UI
public class ItemTooltipUI : MonoBehaviour
{
    // 아이템 이름을 표시할 텍스트
    [SerializeField] private Text _titleText;

    // 아이템 설명을 표시할 텍스트
    [SerializeField] private Text _contentText;

    // 툴팁의 RectTransform
    private RectTransform _rt;

    // 툴팁이 속한 캔버스의 CanvasScaler
    private CanvasScaler _canvasScaler;

    // 위치 보정을 위한 기준 포인트들 (피벗 변경용)
    private static readonly Vector2 LeftTop = new Vector2(0f, 1f);
    private static readonly Vector2 LeftBottom = new Vector2(0f, 0f);
    private static readonly Vector2 RightTop = new Vector2(1f, 1f);
    private static readonly Vector2 RightBottom = new Vector2(1f, 0f);

    // 초기화 및 시작 시 숨김 처리
    private void Awake()
    {
        Init();
        Hide();
    }

    // 툴팁 초기 설정 함수
    private void Init()
    {
        TryGetComponent(out _rt);                            // RectTransform 캐싱
        _rt.pivot = LeftTop;                                 // 기본 피벗 설정
        _canvasScaler = GetComponentInParent<CanvasScaler>();
        DisableAllChildrenRaycastTarget(transform);          // 자식 오브젝트들 레이캐스트 비활성화
    }

    // 모든 자식 Graphic 컴포넌트의 Raycast Target을 비활성화
    private void DisableAllChildrenRaycastTarget(Transform tr)
    {
        tr.TryGetComponent(out Graphic gr);
        if (gr != null)
            gr.raycastTarget = false;

        int childCount = tr.childCount;
        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            DisableAllChildrenRaycastTarget(tr.GetChild(i));
        }
    }

    // 툴팁 텍스트 내용 설정
    public void SetItemInfo(ItemData data)
    {
        _titleText.text = data.Name;
        _contentText.text = data.Tooltip;
    }

    // 툴팁의 월드 좌표 위치를 설정 (슬롯 기준)
    public void SetRectPosition(RectTransform slotRect)
    {
        // 해상도에 따라 보정 비율 계산
        float wRatio = Screen.width / _canvasScaler.referenceResolution.x;
        float hRatio = Screen.height / _canvasScaler.referenceResolution.y;
        float ratio =
            wRatio * (1f - _canvasScaler.matchWidthOrHeight) +
            hRatio * (_canvasScaler.matchWidthOrHeight);

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        // 기본 툴팁 위치 = 슬롯의 오른쪽 아래
        _rt.position = slotRect.position + new Vector3(slotWidth, -slotHeight);
        Vector2 pos = _rt.position;

        // 툴팁의 실제 크기 계산
        float width = _rt.rect.width * ratio;
        float height = _rt.rect.height * ratio;

        // 툴팁이 화면을 벗어나는지 확인
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        // 화면 경계에 따라 위치 조정
        if (rightTruncated && !bottomTruncated)
        {
            // 오른쪽만 잘린 경우 => 왼쪽 아래에 표시
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        else if (!rightTruncated && bottomTruncated)
        {
            // 아래만 잘린 경우 => 오른쪽 위에 표시
            _rt.position = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        else if (rightTruncated && bottomTruncated)
        {
            // 모두 잘린 경우 => 왼쪽 위에 표시
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
        // 잘리지 않은 경우는 기본 위치 유지
    }

    // 툴팁 표시
    public void Show() => gameObject.SetActive(true);

    // 툴팁 숨김
    public void Hide() => gameObject.SetActive(false);
}