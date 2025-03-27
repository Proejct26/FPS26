using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// 인벤토리의 각 슬롯 UI를 관리하는 클래스
public class ItemSlotUI : MonoBehaviour
{
    // 인벤토리에서 드래그 시 슬롯을 새로만들어 이미지를 참조
    public Image IconImage => _iconImage;

    // 슬롯 내부 아이콘과 슬롯 경계 사이 여백
    [SerializeField] private float _padding = 1f;

    // 아이템 아이콘 이미지
    [SerializeField] private Image _iconImage;

    // 아이템 개수 텍스트
    [SerializeField] private Text _amountText;

    // 하이라이트 이미지 (선택 시 표시)
    [SerializeField] private Image _highlightImage;

    // 하이라이트 알파값 (투명도)
    [SerializeField] private float _highlightAlpha = 0.5f;

    // 하이라이트 변화 시간
    [SerializeField] private float _highlightFadeDuration = 0.2f;

    // 슬롯의 인덱스
    public int Index { get; private set; }

    // 슬롯에 아이템이 존재하는지 여부
    public bool HasItem => _iconImage.sprite != null;

    // 해당 슬롯이 현재 접근 가능한지 여부
    public bool IsAccessible => _isAccessibleSlot && _isAccessibleItem;

    // 슬롯 및 아이콘의 RectTransform
    public RectTransform SlotRect => _slotRect;
    public RectTransform IconRect => _iconRect;

    private InventoryUI _inventoryUI;

    private RectTransform _slotRect;
    private RectTransform _iconRect;
    private RectTransform _highlightRect;

    private GameObject _iconGo;
    private GameObject _textGo;
    private GameObject _highlightGo;

    private Image _slotImage;

    private float _currentHLAlpha = 0f;             // 현재 하이라이트 투명도
    private bool _isAccessibleSlot = true;          // 슬롯 자체 접근 가능 여부
    private bool _isAccessibleItem = true;          // 아이템 접근 가능 여부

    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    // 초기화 (컴포넌트 설정 및 초기값 적용)
    private void Awake()
    {
        InitComponents();
        InitValues();
    }

    // 슬롯 관련 컴포넌트들 캐싱
    private void InitComponents()
    {
        _inventoryUI = GetComponentInParent<InventoryUI>();

        _slotRect = GetComponent<RectTransform>();
        _iconRect = _iconImage.rectTransform;
        _highlightRect = _highlightImage.rectTransform;

        _iconGo = _iconRect.gameObject;
        _textGo = _amountText.gameObject;
        _highlightGo = _highlightImage.gameObject;

        _slotImage = GetComponent<Image>();
    }

    // 슬롯 설정값 초기화
    private void InitValues()
    {
        _iconRect.pivot = new Vector2(0.5f, 0.5f);
        _iconRect.anchorMin = Vector2.zero;
        _iconRect.anchorMax = Vector2.one;
        _iconRect.offsetMin = Vector2.one * _padding;
        _iconRect.offsetMax = Vector2.one * -_padding;

        _highlightRect.pivot = _iconRect.pivot;
        _highlightRect.anchorMin = _iconRect.anchorMin;
        _highlightRect.anchorMax = _iconRect.anchorMax;
        _highlightRect.offsetMin = _iconRect.offsetMin;
        _highlightRect.offsetMax = _iconRect.offsetMax;

        _iconImage.raycastTarget = false;
        _highlightImage.raycastTarget = false;

        HideIcon();
        _highlightGo.SetActive(false);
    }

    // 아이콘 표시
    private void ShowIcon() => _iconGo.SetActive(true);
    private void HideIcon() => _iconGo.SetActive(false);

    // 텍스트 표시
    private void ShowText() => _textGo.SetActive(true);
    private void HideText() => _textGo.SetActive(false);

    // 슬롯 인덱스 설정
    public void SetSlotIndex(int index) => Index = index;

    // 슬롯 접근 가능 여부 설정
    public void SetSlotAccessibleState(bool value)
    {
        if (_isAccessibleSlot == value) return;

        if (value)
        {
            _slotImage.color = Color.black;
        }
        else
        {
            _slotImage.color = InaccessibleSlotColor;
            HideIcon();
            HideText();
        }

        _isAccessibleSlot = value;
    }

    // 아이템 접근 가능 여부 설정
    public void SetItemAccessibleState(bool value)
    {
        if (_isAccessibleItem == value) return;

        if (value)
        {
            _iconImage.color = Color.white;
            _amountText.color = Color.white;
        }
        else
        {
            _iconImage.color = InaccessibleIconColor;
            _amountText.color = InaccessibleIconColor;
        }

        _isAccessibleItem = value;
    }

    // 다른 슬롯과 아이템 스프라이트 교환 또는 이동
    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null || other == this) return;
        if (!this.IsAccessible || !other.IsAccessible) return;

        var temp = _iconImage.sprite;

        if (other.HasItem) SetItem(other._iconImage.sprite);
        else RemoveItem();

        other.SetItem(temp);
    }

    // 슬롯에 아이템 등록
    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            _iconImage.sprite = itemSprite;
            ShowIcon();
        }
        else
        {
            RemoveItem();
        }
    }

    // 슬롯에서 아이템 제거
    public void RemoveItem()
    {
        _iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    // 아이템 이미지의 투명도 설정
    public void SetIconAlpha(float alpha)
    {
        _iconImage.color = new Color(
            _iconImage.color.r, _iconImage.color.g, _iconImage.color.b, alpha
        );
    }

    // 아이템 수량 텍스트 표시 (1 이하는 숨김)
    public void SetItemAmount(int amount)
    {
        if (HasItem && amount > 1) ShowText();
        else HideText();

        _amountText.text = amount.ToString();
    }

    // 하이라이트 표시/해제
    public void Highlight(bool show)
    {
        if (!this.IsAccessible) return;

        if (show)
            StartCoroutine(nameof(HighlightFadeInRoutine));
        else
            StartCoroutine(nameof(HighlightFadeOutRoutine));
    }

    // 하이라이트 이미지를 위/아래로 배치
    public void SetHighlightOnTop(bool value)
    {
        if (value)
            _highlightRect.SetAsLastSibling();
        else
            _highlightRect.SetAsFirstSibling();
    }

    // 하이라이트 알파값 서서히 증가
    private IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        _highlightGo.SetActive(true);

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha <= _highlightAlpha; _currentHLAlpha += unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentHLAlpha
            );

            yield return null;
        }
    }

    // 하이라이트 알파값 서서히 감소
    private IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha >= 0f; _currentHLAlpha -= unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentHLAlpha
            );

            yield return null;
        }

        _highlightGo.SetActive(false);
    }
}