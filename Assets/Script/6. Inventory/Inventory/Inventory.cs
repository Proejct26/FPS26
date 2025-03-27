using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    [Item의 상속구조 설명]
    - Item: 모든 아이템의 베이스 클래스
        - CountableItem: 수량이 있는 아이템
            - PortionItem: 사용 시 수량 1 감소
        - EquipmentItem: 장비 아이템
            - WeaponItem: 무기 아이템
            - ArmorItem: 방어구 아이템

    [ItemData의 상속구조 설명]
    - ItemData: 모든 아이템이 공통으로 가지는 정보
        - CountableItemData: 수량이 있는 아이템 데이터
            - PortionItemData: 수치 기반 효과 (ex. 회복량)
        - EquipmentItemData: 장비 아이템의 기본 데이터 (ex. 내구도)
            - WeaponItemData: 무기 관련 정보 (공격력 등)
            - ArmorItemData: 방어구 관련 정보 (방어력 등)
*/

/*
    [Inventory API 목록]
    - HasItem(int): 슬롯에 아이템이 있는지 확인
    - IsCountableItem(int): 해당 슬롯 아이템이 Countable인지 확인
    - GetCurrentAmount(int): 슬롯의 수량을 반환
    - GetItemData(int): 해당 슬롯의 ItemData 반환
    - GetItemName(int): 해당 슬롯의 아이템 이름 반환

    - Add(ItemData, int): 아이템 추가, 못 넣은 수량 반환
    - Remove(int): 슬롯에서 아이템 제거
    - Swap(int, int): 두 슬롯의 아이템 위치 교체
    - SeparateAmount(int, int, int): 수량 나누기 (A -> B)
    - Use(int): 슬롯의 아이템 사용
    - UpdateSlot(int): 특정 슬롯 UI 갱신
    - UpdateAllSlot(): 모든 슬롯 UI 갱신
    - UpdateAccessibleStatesAll(): 슬롯 접근 가능 여부 갱신
    - TrimAll(): 빈칸 없이 정리
    - SortAll(): 정렬 및 정리
*/

public class Inventory : MonoBehaviour
{
    // 인벤토리 최대 용량 (외부 접근은 읽기 전용)
    public int Capacity { get; private set; }

    // 초기 슬롯 수
    [SerializeField, Range(8, 64)] private int _initalCapacity = 32;
    // 최대 슬롯 수
    [SerializeField, Range(8, 64)] private int _maxCapacity = 64;

    // 연결된 UI 객체
    [SerializeField] private InventoryUI _inventoryUI;

    // 슬롯에 들어갈 아이템 배열
    [SerializeField] private Item[] _items;

    // 갱신이 필요한 슬롯 인덱스 집합
    private readonly HashSet<int> _indexSetForUpdate = new HashSet<int>();

    // 아이템 타입별 정렬 기준값
    private readonly static Dictionary<Type, int> _sortWeightDict = new Dictionary<Type, int>
        {
            { typeof(FoodItemData), 10000 },
            { typeof(WeaponItemData),  20000 },
            { typeof(ArmorItemData),   30000 },
        };

    // 아이템 비교 클래스: ID + 타입 가중치 기반 정렬
    private class ItemComparer : IComparer<Item>
    {
        public int Compare(Item a, Item b)
        {
            return (a.Data.ID + _sortWeightDict[a.Data.GetType()])
                 - (b.Data.ID + _sortWeightDict[b.Data.GetType()]);
        }
    }

    private static readonly ItemComparer _itemComparer = new ItemComparer();

#if UNITY_EDITOR
    // 에디터에서 값 수정 시, 초기 용량이 최대를 초과하지 않도록 제한
    private void OnValidate()
    {
        if (_initalCapacity > _maxCapacity)
            _initalCapacity = _maxCapacity;
    }
#endif

    // 인벤토리 초기화
    private void Awake()
    {
        _items = new Item[_maxCapacity];
        Capacity = _initalCapacity;
        _inventoryUI.SetInventoryReference(this);
    }

    // 시작 시 슬롯 접근 상태 업데이트
    private void Start()
    {
        UpdateAccessibleStatesAll();
    }

    // 인덱스 유효성 검사
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    // 빈 슬롯 탐색 (앞에서부터)
    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
            if (_items[i] == null)
                return i;
        return -1;
    }

    // 동일 아이템이 있고, 수량 여유 있는 슬롯 탐색
    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
        {
            var current = _items[i];
            if (current == null) continue;

            if (current.Data == target && current is CountableItem ci)
            {
                if (!ci.IsMax)
                    return i;
            }
        }
        return -1;
    }

    // 단일 슬롯 UI 갱신
    private void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;
        Item item = _items[index];

        if (item != null)
        {
            _inventoryUI.SetItemIcon(index, item.Data.IconSprite);

            if (item is CountableItem ci)
            {
                if (ci.IsEmpty)
                {
                    _items[index] = null;
                    RemoveIcon();
                    return;
                }
                else
                {
                    _inventoryUI.SetItemAmountText(index, ci.Amount);
                }
            }
            else
            {
                _inventoryUI.HideItemAmountText(index);
            }
            _inventoryUI.UpdateSlotFilterState(index, item.Data);
        }
        else
        {
            RemoveIcon();
        }

        void RemoveIcon()
        {
            _inventoryUI.RemoveItem(index);
            _inventoryUI.HideItemAmountText(index);
        }
    }

    // 복수 슬롯 UI 갱신
    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
            UpdateSlot(i);
    }

    // 전체 슬롯 UI 갱신
    private void UpdateAllSlot()
    {
        for (int i = 0; i < Capacity; i++)
            UpdateSlot(i);
    }

    // 해당 슬롯에 아이템이 있는지 확인
    public bool HasItem(int index) => IsValidIndex(index) && _items[index] != null;

    // 해당 슬롯의 아이템이 수량형인지 확인
    public bool IsCountableItem(int index) => HasItem(index) && _items[index] is CountableItem;

    // 현재 수량 반환
    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (_items[index] == null) return 0;

        if (!(_items[index] is CountableItem ci)) return 1;
        return ci.Amount;
    }

    // 아이템 데이터 반환
    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        return _items[index]?.Data;
    }

    // 아이템 이름 반환
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        return _items[index]?.Data.Name ?? "";
    }

    // 외부에서 UI 연결
    public void ConnectUI(InventoryUI inventoryUI)
    {
        _inventoryUI = inventoryUI;
        _inventoryUI.SetInventoryReference(this);
    }

    // 아이템 추가
    public int Add(ItemData itemData, int amount = 1)
    {
        int index;

        // 수량형 아이템
        if (itemData is CountableItemData ciData)
        {
            bool findNextCountable = true;
            index = -1;

            while (amount > 0)
            {
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);
                    if (index == -1) findNextCountable = false;
                    else
                    {
                        var ci = _items[index] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);
                        UpdateSlot(index);
                    }
                }
                else
                {
                    index = FindEmptySlotIndex(index + 1);
                    if (index == -1) break;

                    var ci = ciData.CreateItem() as CountableItem;
                    ci.SetAmount(amount);
                    _items[index] = ci;
                    amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;
                    UpdateSlot(index);
                }
            }
        }
        else // 비수량형 아이템
        {
            if (amount == 1)
            {
                index = FindEmptySlotIndex();
                if (index != -1)
                {
                    _items[index] = itemData.CreateItem();
                    amount = 0;
                    UpdateSlot(index);
                }
            }
            else
            {
                index = -1;
                for (; amount > 0; amount--)
                {
                    index = FindEmptySlotIndex(index + 1);
                    if (index == -1) break;

                    _items[index] = itemData.CreateItem();
                    UpdateSlot(index);
                }
            }
        }

        return amount;
    }

    // 슬롯 아이템 제거
    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;
        _items[index] = null;
        _inventoryUI.RemoveItem(index);
    }

    // 두 슬롯 간 아이템 교체
    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

        var itemA = _items[indexA];
        var itemB = _items[indexB];

        if (itemA != null && itemB != null && itemA.Data == itemB.Data &&
            itemA is CountableItem ciA && itemB is CountableItem ciB)
        {
            int sum = ciA.Amount + ciB.Amount;
            if (sum <= ciB.MaxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - ciB.MaxAmount);
                ciB.SetAmount(ciB.MaxAmount);
            }
        }
        else
        {
            _items[indexA] = itemB;
            _items[indexB] = itemA;
        }

        UpdateSlot(indexA, indexB);
    }

    // 수량 분리 (A -> B 슬롯)
    public void SeparateAmount(int indexA, int indexB, int amount)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;
        if (!(_items[indexA] is CountableItem ciA)) return;
        if (_items[indexB] != null) return;

        _items[indexB] = ciA.SeperateAndClone(amount);
        UpdateSlot(indexA, indexB);
    }

    // 아이템 사용
    public void Use(int index)
    {
        if (!IsValidIndex(index) || _items[index] == null) return;

        if (_items[index] is IUsableItem uItem && uItem.Use())
            UpdateSlot(index);
    }

    // 인벤토리에서 동일한 CountableItemData를 가진 아이템을 지정 수량만큼 소비
    public bool TryConsumeItem(CountableItemData targetData, int amount)
    {
        if (targetData == null || amount <= 0) return false;

        int total = 0;
        List<(int index, CountableItem item)> slotList = new();

        // 1단계: 전체 슬롯에서 동일한 아이템 찾기 + 수량 합산
        for (int i = 0; i < Capacity; i++)
        {
            if (_items[i] is CountableItem ci && ci.CountableData == targetData)
            {
                total += ci.Amount;
                slotList.Add((i, ci));

                if (total >= amount)
                    break;
            }
        }

        // 수량 부족
        if (total < amount)
            return false;

        // 2단계: 슬롯 순서대로 수량 소비
        int remaining = amount;
        foreach (var (index, item) in slotList)
        {
            if (remaining <= 0) break;

            int consume = Mathf.Min(item.Amount, remaining);
            item.SetAmount(item.Amount - consume);
            remaining -= consume;

            // 비었으면 제거
            if (item.IsEmpty)
                _items[index] = null;

            UpdateSlot(index);
        }

        return true;
    }

    // 인벤토리 내에서 특정 CountableItemData 타입의 총 수량을 반환, 장비는 셀 수 없으니 처리 안함
    public int GetTotalAmount(CountableItemData targetData)
    {
        if (targetData == null) return 0;

        int total = 0;

        for (int i = 0; i < Capacity; i++)
        {
            if (_items[i] is CountableItem ci && ci.CountableData == targetData)
            {
                total += ci.Amount;
            }
        }

        return total;
    }

    // 슬롯 접근 가능 여부 갱신
    public void UpdateAccessibleStatesAll()
    {
        _inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    // 빈칸 없이 앞으로 정렬
    public void TrimAll()
    {
        _indexSetForUpdate.Clear();

        int i = -1;
        while (++i < Capacity && _items[i] != null) ;
        int j = i;

        while (++j < Capacity)
        {
            if (_items[j] == null) continue;

            _indexSetForUpdate.Add(i);
            _indexSetForUpdate.Add(j);

            _items[i++] = _items[j];
            _items[j] = null;
        }

        foreach (var index in _indexSetForUpdate)
            UpdateSlot(index);

        _inventoryUI.UpdateAllSlotFilters();
    }

    // 아이템 정렬 + 빈칸 정리
    public void SortAll()
    {
        TrimAll();

        int count = 0;
        while (count < Capacity && _items[count] != null) count++;

        Array.Sort(_items, 0, count, _itemComparer);

        UpdateAllSlot();
        _inventoryUI.UpdateAllSlotFilters();
    }

    // 특정 인덱스의 Item 인스턴스를 반환
    public Item GetItem(int index)
    {
        if (!IsValidIndex(index)) return null;
        return _items[index];
    }



}