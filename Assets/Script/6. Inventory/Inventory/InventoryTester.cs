using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 인벤토리 시스템 테스트를 위한 MonoBehaviour
// 버튼 클릭 시 다양한 아이템을 인벤토리에 추가하거나 제거함
public class InventoryTester : MonoBehaviour
{
    // 연결된 인벤토리 인스턴스
    public Inventory _inventory;

    // 테스트용 아이템 데이터 배열 (에디터에서 설정)
    public ItemData[] _itemDataArray;

    [Space(12)]
    // 모든 아이템 제거 버튼
    public Button _removeAllButton;

    [Space(8)]
    // 장비 아이템 추가 버튼들
    public Button _AddArmorA1;
    public Button _AddArmorB1;
    public Button _AddSwordA1;
    public Button _AddSwordB1;

    // 포션 아이템 추가 버튼들 (1개 / 50개)
    public Button _AddPortionA1;
    public Button _AddPortionA50;
    public Button _AddPortionB1;
    public Button _AddPortionB50;

    private void Start()
    {
        // 시작 시 아이템 배열이 존재하면 각 아이템을 3개 추가
        if (_itemDataArray?.Length > 0)
        {
            for (int i = 0; i < _itemDataArray.Length; i++)
            {
                _inventory.Add(_itemDataArray[i], 3);

                // 수량형 아이템인 경우, 255개 추가 (최대 테스트용)
                if (_itemDataArray[i] is CountableItemData)
                    _inventory.Add(_itemDataArray[i], 255);
            }
        }

        // 모든 아이템 제거 버튼에 리스너 등록
        _removeAllButton.onClick.AddListener(() =>
        {
            int capacity = _inventory.Capacity;
            for (int i = 0; i < capacity; i++)
                _inventory.Remove(i);
        });

        // 각 버튼 클릭 시 해당 인덱스의 아이템을 인벤토리에 추가
        _AddArmorA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[0]));
        _AddArmorB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[1]));

        _AddSwordA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[2]));
        _AddSwordB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[3]));

        _AddPortionA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[4]));
        _AddPortionA50.onClick.AddListener(() => _inventory.Add(_itemDataArray[4], 50));
        _AddPortionB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[5]));
        _AddPortionB50.onClick.AddListener(() => _inventory.Add(_itemDataArray[5], 50));
    }
}
