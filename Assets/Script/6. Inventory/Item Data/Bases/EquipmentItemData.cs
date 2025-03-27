using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 장비 아이템의 공통 데이터를 정의하는 추상 클래스
// 무기, 방어구 등 장비 아이템들이 상속받아 사용함
public abstract class EquipmentItemData : ItemData
{
    // 장비 아이템의 최대 내구도
    // 이 값은 내구도 시스템에서 장비의 수명을 판단할 때 사용됨
    public int MaxDurability => _maxDurability;

    // 인스펙터에서 설정할 수 있는 최대 내구도 값 (기본값은 100)
    [SerializeField] private int _maxDurability = 100;
}
