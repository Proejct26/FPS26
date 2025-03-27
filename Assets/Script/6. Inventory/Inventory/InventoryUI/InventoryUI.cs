using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
    [기능 - 에디터 전용]
    - 게임 시작 시 동적으로 생성될 슬롯 미리보기(개수, 크기 미리보기 가능)

    [기능 - 유저 인터페이스]
    - 슬롯에 마우스 올리기
      - 사용 가능 슬롯 : 하이라이트 이미지 표시
      - 아이템 존재 슬롯 : 아이템 정보 툴팁 표시

    - 드래그 앤 드롭
      - 아이템 존재 슬롯 -> 아이템 존재 슬롯 : 두 아이템 위치 교환
      - 아이템 존재 슬롯 -> 아이템 미존재 슬롯 : 아이템 위치 변경
        - Shift 또는 Ctrl 누른 상태일 경우 : 셀 수 있는 아이템 수량 나누기
      - 아이템 존재 슬롯 -> UI 바깥 : 아이템 버리기

    - 슬롯 우클릭
      - 사용 가능한 아이템일 경우 : 아이템 사용

    - 기능 버튼(좌측 상단)
      - Trim : 앞에서부터 빈 칸 없이 아이템 채우기
      - Sort : 정해진 가중치대로 아이템 정렬

    - 필터 버튼(우측 상단)
      - [A] : 모든 아이템 필터링
      - [E] : 장비 아이템 필터링
      - [P] : 소비 아이템 필터링

      * 필터링에서 제외된 아이템 슬롯들은 조작 불가

    [기능 - 기타]
    - InvertMouse(bool) : 마우스 좌클릭/우클릭 반전 여부 설정
*/


public class InventoryUI : MonoBehaviour
{
    // 슬롯 개수 및 UI 배치 관련 설정
    [SerializeField] private int _horizontalSlotCount;
    [SerializeField] private int _slotCounts;
    [SerializeField] private float _slotSize;

    // 기능 설정 (툴팁, 하이라이트 등 UI 동작)
    [SerializeField] private bool _showTooltip;
    [SerializeField] private bool _showHighlight;
    [SerializeField] private bool _showRemovingPopup;

    // 연결된 UI 오브젝트들 (슬롯 프리팹, 툴팁, 팝업, 버튼, 토글 등)
    [SerializeField] private RectTransform _contentAreaRT;
    [SerializeField] private GameObject _slotUiPrefab;
    [SerializeField] private ItemTooltipUI _itemTooltip;
    [SerializeField] private InventoryPopupUI _popup;
    [SerializeField] private Button _trimButton;
    [SerializeField] private Button _sortButton;
    [SerializeField] private Toggle _toggleFilterAll;
    [SerializeField] private Toggle _toggleFilterEquipments;
    [SerializeField] private Toggle _toggleFilterPortions;

    // 마우스 클릭 반전 옵션
    [SerializeField] private bool _mouseReversed;

    // 스크롤 관련
    [SerializeField] private ScrollRect _scrollRect; 
    private RectTransform _canvasRootTransform;

    // 마우스 클릭 시 처리 (좌클릭: 드래그 시작 / 우클릭: 아이템 사용)
    private GameObject _dragIconInstance;


    private Inventory _inventory;                     // 실제 인벤토리 객체 참조
    private List<ItemSlotUI> _slotUIList;             // 모든 슬롯 UI 리스트
    private GraphicRaycaster _gr;                     // 그래픽 레이캐스터
    private PointerEventData _ped;                    // 포인터 이벤트 데이터
    private List<RaycastResult> _rrList;              // 레이캐스트 결과 리스트

    private ItemSlotUI _pointerOverSlot;              // 마우스가 올려진 슬롯
    private ItemSlotUI _beginDragSlot;                // 드래그 시작 슬롯
    private Transform _beginDragIconTransform;        // 드래그 아이콘의 트랜스폼
    private Vector3 _beginDragIconPoint;              // 드래그 시작 시 아이콘 위치
    private Vector3 _beginDragCursorPoint;            // 드래그 시작 시 마우스 위치
    private int _beginDragSlotSiblingIndex;           // 드래그 슬롯의 원래 형제 인덱스

    private int _leftClick = 0;                       // 좌클릭 마우스 버튼 번호
    private int _rightClick = 1;                      // 우클릭 마우스 버튼 번호

    private enum FilterOption { All, Equipment, Food, Material }  // 필터 타입
    private FilterOption _currentFilterOption = FilterOption.All;



    // 초기 설정: 필드 초기화 및 슬롯, 버튼, 토글 이벤트 설정
    private void Awake()
    {
        Init();
        InitSlots();
        InitButtonEvents();
        InitToggleEvents();
    }

    // 프레임마다 호출: 마우스 입력 및 슬롯 UI 상호작용 처리
    private void Update()
    {
        _ped.position = Input.mousePosition;

        OnPointerEnterAndExit();        // 슬롯 포인터 감지
        if (_showTooltip) ShowOrHideItemTooltip();
        OnPointerDown();                // 마우스 클릭 감지
        OnPointerDrag();                // 마우스 드래그 감지
        OnPointerUp();                  // 마우스 버튼 해제 감지
        
        // 드래그 중이면 마우스를 따라 아이콘 이동
        if (_dragIconInstance != null)
        {
            _beginDragIconTransform.position = Input.mousePosition;
        }
    }

    // UI 컴포넌트 및 기본 필드 초기화
    private void Init()
    {
        // GraphicRaycaster 컴포넌트 가져오기 (없으면 자동 추가)
        TryGetComponent(out _gr);
        if (_gr == null)
            _gr = gameObject.AddComponent<GraphicRaycaster>();

        // 마우스 이벤트 처리를 위한 PointerEventData 초기화
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10); // 레이캐스트 결과 저장 리스트

        // 아이템 툴팁 UI가 연결되지 않았으면 자식 객체에서 자동으로 찾음
        if (_itemTooltip == null)
        {
            _itemTooltip = GetComponentInChildren<ItemTooltipUI>();
            Debug.Log("인스펙터에서 아이템 툴팁 UI를 직접 지정하지 않아 자식에서 발견하여 초기화하였습니다.");
        }

        // Canvas 루트 캐싱
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            _canvasRootTransform = canvas.transform as RectTransform;
    }

    // 슬롯 UI를 지정된 개수만큼 동적으로 생성하고 초기화
    private void InitSlots()
    {
        // 슬롯 프리팹의 RectTransform 가져오기 및 크기 설정
        _slotUiPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(_slotSize, _slotSize);

        // 슬롯 프리팹에 ItemSlotUI 컴포넌트가 없으면 추가
        _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            _slotUiPrefab.AddComponent<ItemSlotUI>();

        // 슬롯 프리팹은 비활성화된 상태로 유지 (복제용)
        _slotUiPrefab.SetActive(false);

        // 슬롯 리스트 초기화
        _slotUIList = new List<ItemSlotUI>(_slotCounts);

        // 슬롯 행렬 생성
        for (int i = 0; i < _slotCounts; i++)
        {
            int slotIndex = i;

            // 슬롯 생성 및 초기 설정
            var slotRT = CloneSlot();
            slotRT.gameObject.SetActive(true);                      // 활성화
            slotRT.gameObject.name = $"Item Slot [{slotIndex}]";    // 슬롯 이름 설정

            // 슬롯 UI 구성 및 리스트 등록
            var slotUI = slotRT.GetComponent<ItemSlotUI>();
            slotUI.SetSlotIndex(slotIndex);
            _slotUIList.Add(slotUI);
        }

        // 직접 위치를 계산하는 대신 슬롯 프리팹에 LayoutElement 컴포넌트 붙이기
        _slotUiPrefab.AddComponent<LayoutElement>().preferredHeight = _slotSize;

        // 슬롯 프리팹이 씬에 존재하는 임시 객체일 경우 제거
        if (_slotUiPrefab.scene.rootCount != 0)
            Destroy(_slotUiPrefab);

        // 슬롯 프리팹을 복제하고 부모에 연결하는 로컬 메서드
        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUiPrefab);         // 프리팹 복제
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(_contentAreaRT);                           // 부모 지정
            return rt;
        }

        // 슬롯 생성 완료 후 ContentArea 크기 조정
        AdjustContentArea(_slotUIList.Count);
        //AdjustContentAreaPosition();
        StartCoroutine(ScrollToTopNextFrame());
    }

    // Trim, Sort 버튼 클릭 이벤트 리스너 등록
    private void InitButtonEvents()
    {
        _trimButton.onClick.AddListener(() => _inventory.TrimAll()); // 빈 칸 채우기
        _sortButton.onClick.AddListener(() => _inventory.SortAll()); // 정렬
    }

    // 필터 토글 이벤트 리스너 등록
    private void InitToggleEvents()
    {
        // [A] 전체 보기 필터
        _toggleFilterAll.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.All));

        // [E] 장비 아이템 필터
        _toggleFilterEquipments.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Equipment));

        // [P] 소비 아이템 필터
        _toggleFilterPortions.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Food));

        // 내부 필터 갱신 함수
        void UpdateFilter(bool flag, FilterOption option)
        {
            if (flag)
            {
                _currentFilterOption = option;     // 현재 필터 타입 갱신
                UpdateAllSlotFilters();            // 슬롯 접근 가능 상태 갱신
            }
        }
    }


    // 현재 마우스 커서가 UI 요소 위에 있는지 여부를 확인
    private bool IsOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    // 레이캐스트로 UI를 감지하고, 첫 번째 감지된 객체에서 특정 컴포넌트를 찾아 리턴
    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear(); // 기존 결과 초기화

        _gr.Raycast(_ped, _rrList); // 현재 마우스 위치에서 레이캐스트 실행

        if (_rrList.Count == 0)
            return null;

        return _rrList[0].gameObject.GetComponent<T>();
    }

    // 포인터가 슬롯 위에 올라갔거나 빠져나갔을 때의 처리를 담당
    private void OnPointerEnterAndExit()
    {
        var prevSlot = _pointerOverSlot; // 이전 프레임 슬롯
        var curSlot = _pointerOverSlot = RaycastAndGetFirstComponent<ItemSlotUI>(); // 현재 프레임 슬롯

        if (prevSlot == null)
        {
            // 슬롯에 처음 진입
            if (curSlot != null)
                OnCurrentEnter();
        }
        else
        {
            // 슬롯에서 벗어남
            if (curSlot == null)
                OnPrevExit();

            // 슬롯이 변경됨
            else if (prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        // 현재 슬롯에 하이라이트 적용
        void OnCurrentEnter()
        {
            if (_showHighlight)
                curSlot.Highlight(true);
        }

        // 이전 슬롯에서 하이라이트 제거
        void OnPrevExit()
        {
            prevSlot.Highlight(false);
        }
    }

    // 마우스 커서가 아이템 슬롯 위에 있고, 조건을 만족하면 툴팁을 보여줌
    private void ShowOrHideItemTooltip()
    {
        bool isValid =
            _pointerOverSlot != null &&
            _pointerOverSlot.HasItem &&
            _pointerOverSlot.IsAccessible &&
            _pointerOverSlot != _beginDragSlot; // 드래그 중일 땐 툴팁 비활성화

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot); // 툴팁 정보 갱신
            _itemTooltip.Show();               // 툴팁 표시
        }
        else
        {
            _itemTooltip.Hide();               // 툴팁 숨김
        }
    }

    private void OnPointerDown()
    {
        // 좌클릭 → 드래그 시작
        if (Input.GetMouseButtonDown(_leftClick))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (_beginDragSlot != null && _beginDragSlot.HasItem && _beginDragSlot.IsAccessible)
            {
                Debug.Log($"Drag Begin : Slot [{_beginDragSlot.Index}]");

                // 드래그용 아이콘 복사
                var originalIcon = _beginDragSlot.IconImage;
                if (originalIcon == null || originalIcon.sprite == null)
                {
                    Debug.LogWarning("드래그할 아이콘이 없습니다.");
                    _beginDragSlot = null;
                    return;
                }

                _dragIconInstance = Instantiate(originalIcon.gameObject, transform.root);
                _dragIconInstance.name = "DragIcon";

                // 크기 조정 - 원본 스프라이트 크기에 맞게
                var dragIconImage = _dragIconInstance.GetComponent<Image>();
                if (dragIconImage != null)
                    dragIconImage.SetNativeSize();

                // Raycast 방해 방지
                var cg = _dragIconInstance.AddComponent<CanvasGroup>();
                cg.blocksRaycasts = false;

                // 드래그 정보 저장
                _beginDragIconTransform = _dragIconInstance.transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;
                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();

                // 원본 슬롯의 아이콘 숨김
                _beginDragSlot.IconImage.enabled = false;

                // 스크롤 막기
                if (_scrollRect != null)
                    _scrollRect.enabled = false;

                _beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                _beginDragSlot = null;
            }
        }
        // 우클릭 → 아이템 사용
        else if (Input.GetMouseButtonDown(_rightClick))
        {
            ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();
            if (slot != null && slot.HasItem && slot.IsAccessible)
            {
                TryUseItem(slot.Index);
            }
        }
    }



    // 마우스를 드래그하는 동안 아이콘 위치를 마우스에 따라 이동
    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(_leftClick))
        {
            // 드래그 시작 위치 + 마우스 이동 거리만큼 아이콘 이동
            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }

    // 마우스 클릭을 놓을 경우 드래그 종료 처리
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(_leftClick))
        {
            if (_beginDragSlot != null)
            {
                // 드래그용 아이콘 제거
                if (_dragIconInstance != null)
                {
                    Destroy(_dragIconInstance);
                    _dragIconInstance = null;
                }

                // 원본 슬롯의 아이콘 다시 보이게 설정
                _beginDragSlot.IconImage.enabled = true;

                // 슬롯 UI 순서 복원
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                // Raycast 다시 활성화
                var cg = _beginDragSlot.GetComponent<CanvasGroup>();
                if (cg != null)
                    cg.blocksRaycasts = true;

                // 스크롤 다시 활성화
                if (_scrollRect != null)
                    _scrollRect.enabled = true;

                // 드래그 종료 처리
                EndDrag();

                // 하이라이트 이미지를 아이콘 위로 이동
                _beginDragSlot.SetHighlightOnTop(true);

                // 참조 정리
                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }


    // 드래그 종료 시 처리 (아이템 이동, 수량 나누기, 버리기 등)
    private void EndDrag()
    {
        ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        // 다른 슬롯에 아이템을 놓은 경우
        if (endDragSlot != null && endDragSlot.IsAccessible)
        {
            // 셀 수 있는 아이템을 수량 나누는 조건
            bool isSeparatable =
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                (_inventory.IsCountableItem(_beginDragSlot.Index) && !_inventory.HasItem(endDragSlot.Index));

            bool isSeparation = false;
            int currentAmount = 0;

            // 수량이 2 이상일 경우 수량 나누기 활성화
            if (isSeparatable)
            {
                currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.Index);
                if (currentAmount > 1)
                    isSeparation = true;
            }

            // 1. 수량 나누기
            if (isSeparation)
                TrySeparateAmount(_beginDragSlot.Index, endDragSlot.Index, currentAmount);
            // 2. 위치 교환 또는 이동
            else
                TrySwapItems(_beginDragSlot, endDragSlot);

            UpdateTooltipUI(endDragSlot); // 툴팁 정보 갱신
            return;
        }

        // UI 바깥에 놓은 경우 → 아이템 버리기
        if (!IsOverUI())
        {
            int index = _beginDragSlot.Index;
            string itemName = _inventory.GetItemName(index);
            int amount = _inventory.GetCurrentAmount(index);

            // 수량이 있는 아이템이라면 표시
            if (amount > 1)
                itemName += $" x{amount}";

            if (_showRemovingPopup)
            {
                // 확인 팝업을 띄운 뒤 콜백으로 실제 제거
                _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
            }
            else
            {
                // 바로 제거
                TryRemoveItem(index);
            }
        }
        // 슬롯 외 UI 위에 놓은 경우 → 아무 작업도 하지 않음
        else
        {
            Debug.Log($"Drag End(Do Nothing)");
        }
    }



    // UI 및 인벤토리에서 아이템 제거
    private void TryRemoveItem(int index)
    {
        Debug.Log($"UI - Try Remove Item : Slot [{index}]");
        _inventory.Remove(index);
    }

    // 아이템 사용
    private void TryUseItem(int index)
    {
        Debug.Log($"UI - Try Use Item : Slot [{index}]");
        _inventory.Use(index);
    }

    // 두 슬롯의 아이템 교환
    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        if (from == to)
        {
            Debug.Log($"UI - Try Swap Items: Same Slot [{from.Index}]");
            return;
        }

        Debug.Log($"UI - Try Swap Items: Slot [{from.Index} -> {to.Index}]");
        from.SwapOrMoveIcon(to);
        _inventory.Swap(from.Index, to.Index);
    }

    // 셀 수 있는 아이템 개수 나누기
    private void TrySeparateAmount(int indexA, int indexB, int amount)
    {
        if (indexA == indexB)
        {
            Debug.Log($"UI - Try Separate Amount: Same Slot [{indexA}]");
            return;
        }

        Debug.Log($"UI - Try Separate Amount: Slot [{indexA} -> {indexB}]");

        string itemName = $"{_inventory.GetItemName(indexA)} x{amount}";

        _popup.OpenAmountInputPopup(
            amt => _inventory.SeparateAmount(indexA, indexB, amt),
            amount, itemName
        );
    }

    // 툴팁 UI의 슬롯 데이터 갱신
    private void UpdateTooltipUI(ItemSlotUI slot)
    {
        if (!slot.IsAccessible || !slot.HasItem)
            return;

        _itemTooltip.SetItemInfo(_inventory.GetItemData(slot.Index));
        _itemTooltip.SetRectPosition(slot.SlotRect);
    }





    // 인벤토리 참조 등록 (인벤토리에서 직접 호출)
    public void SetInventoryReference(Inventory inventory)
    {
        _inventory = inventory;
    }

    // 마우스 클릭 좌우 반전시키기 (true : 반전), 안쓸 예정
    public void InvertMouse(bool value)
    {
        _leftClick = value ? 1 : 0;
        _rightClick = value ? 0 : 1;
        _mouseReversed = value;
    }

    // 슬롯에 아이템 아이콘 등록
    public void SetItemIcon(int index, Sprite icon)
    {
        Debug.Log($"Set Item Icon : Slot [{index}]");
        _slotUIList[index].SetItem(icon);
    }

    // 해당 슬롯의 아이템 개수 텍스트 지정
    public void SetItemAmountText(int index, int amount)
    {
        Debug.Log($"Set Item Amount Text : Slot [{index}], Amount [{amount}]");
        _slotUIList[index].SetItemAmount(amount);
    }

    // 해당 슬롯의 아이템 개수 텍스트 숨기기
    public void HideItemAmountText(int index)
    {
        Debug.Log($"Hide Item Amount Text : Slot [{index}]");
        _slotUIList[index].SetItemAmount(1);
    }

    // 슬롯에서 아이템 아이콘 제거, 개수 텍스트 숨기기
    public void RemoveItem(int index)
    {
        Debug.Log($"Remove Item : Slot [{index}]");
        _slotUIList[index].RemoveItem();
    }

    // 접근 가능한 슬롯 범위 설정
    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            _slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    // 특정 슬롯의 필터 상태 업데이트
    public void UpdateSlotFilterState(int index, ItemData itemData)
    {
        bool isFiltered = true;

        if (itemData != null)
            switch (_currentFilterOption)
            {
                case FilterOption.Equipment:
                    isFiltered = (itemData is EquipmentItemData);
                    break;
                case FilterOption.Food:
                    isFiltered = (itemData is FoodItemData);
                    break;
            }

        _slotUIList[index].SetItemAccessibleState(isFiltered);
    }

    // 모든 슬롯 필터 상태 업데이트
    public void UpdateAllSlotFilters()
    {
        int capacity = _inventory.Capacity;
        for (int i = 0; i < capacity; i++)
        {
            ItemData data = _inventory.GetItemData(i);
            UpdateSlotFilterState(i, data);
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
    public void OpenUI()
    {
        gameObject.SetActive(true);
    }
    private void AdjustContentArea(int totalSlotCount)
    {
        int columnCount = _horizontalSlotCount;
        int rowCount = Mathf.CeilToInt((float)totalSlotCount / columnCount);

        float cellSize = 64f; // 슬롯 한 칸 크기
        float spacing = 5f;
        float padding = 0f; // GridLayoutGroup의 Padding이 0이라면

        float height = (cellSize * rowCount) + (spacing * (rowCount - 1)) + (padding * 2);
        _contentAreaRT.sizeDelta = new Vector2(_contentAreaRT.sizeDelta.x, height);
    }
    private RectTransform FindScrollViewRectTransform()
    {
        // 하위 오브젝트 중에서 ScrollRect를 찾고, 해당 RectTransform 반환
        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
            return scrollRect.GetComponent<RectTransform>();

        Debug.LogError("하위에서 ScrollRect를 찾을 수 없습니다.");
        return null;
    }

    private void AdjustContentAreaPosition()
    {
        RectTransform scrollRT = FindScrollViewRectTransform();
        if (scrollRT == null) return;

        float scrollWidth = scrollRT.rect.width;
        _contentAreaRT.anchoredPosition = new Vector2(-scrollWidth / 2f, _contentAreaRT.anchoredPosition.y);
    }
    private IEnumerator ScrollToTopNextFrame()
    {
        yield return null; // 1프레임 대기 → Layout 계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentAreaRT); // 강제 반영
        yield return null; // 2프레임 대기 후
        if (_scrollRect != null)
            _scrollRect.verticalNormalizedPosition = 1f;
    }
}