using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 장비 아이템의 공통 로직을 담당하는 추상 클래스
// 무기, 방어구 등은 이 클래스를 상속받는다
public abstract class EquipmentItem : Item
{
    // 이 장비 아이템이 참조하는 데이터 (스크립터블 오브젝트 기반)
    public EquipmentItemData EquipmentData { get; private set; }

    // 현재 내구도
    public int Durability
    {
        get => _durability;
        set
        {
            // 0 미만이면 0으로 고정
            if (value < 0) value = 0;

            // 최대 내구도 초과 시 최대값으로 고정
            if (value > EquipmentData.MaxDurability)
                value = EquipmentData.MaxDurability;

            _durability = value;
        }
    }

    private int _durability;

    // 생성자: 데이터 참조 및 초기 내구도 설정
    public EquipmentItem(EquipmentItemData data) : base(data)
    {
        EquipmentData = data;
        Durability = data.MaxDurability;
    }

    // ItemData 외의 필드를 받는 생성자는 제공하지 않음
    // 자식 클래스마다 동일 로직 반복되기 때문에 유지보수에 불리
}
