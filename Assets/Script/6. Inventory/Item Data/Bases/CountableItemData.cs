using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수량을 가질 수 있는 아이템 데이터의 추상 클래스
// 포션, 재료 등 여러 개를 가질 수 있는 아이템이 이 클래스를 상속함
public abstract class CountableItemData : ItemData
{
    // 인벤토리에서 이 아이템이 가질 수 있는 최대 수량
    public int MaxAmount => _maxAmount;

    // 인스펙터에서 설정 가능한 최대 수량 값 (기본값은 99)
    [SerializeField] private int _maxAmount = 99;
}
