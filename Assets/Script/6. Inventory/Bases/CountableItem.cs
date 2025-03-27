using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수량이 존재하는 아이템의 추상 클래스
// 포션, 재료 등 여러 개를 가질 수 있는 아이템을 나타냄
public abstract class CountableItem : Item
{
    // 이 아이템이 참조하는 ScriptableObject 기반의 데이터
    public CountableItemData CountableData { get; private set; }

    // 현재 아이템의 수량
    public int Amount { get; protected set; }

    // 하나의 슬롯이 가질 수 있는 최대 수량
    public int MaxAmount => CountableData.MaxAmount;

    // 최대 수량에 도달했는지 여부
    public bool IsMax => Amount >= CountableData.MaxAmount;

    // 수량이 0인지 여부
    public bool IsEmpty => Amount <= 0;

    // 생성자: 아이템 데이터와 초기 수량을 설정
    public CountableItem(CountableItemData data, int amount = 1) : base(data)
    {
        CountableData = data;
        SetAmount(amount);
    }

    // 수량을 설정하고, 0 ~ MaxAmount 사이로 제한
    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    // 수량을 추가하고, 초과한 수량을 반환 (초과 없으면 0)
    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        return (nextAmount > MaxAmount) ? (nextAmount - MaxAmount) : 0;
    }

    // 수량을 분리하여 새로운 아이템 복제 (최소 1개 남기고 분리 가능)
    public CountableItem SeperateAndClone(int amount)
    {
        // 수량이 1 이하이면 복제 불가
        if (Amount <= 1) return null;

        // 최소 1개는 남기기 위해 최대 분리 가능량을 제한
        if (amount > Amount - 1)
            amount = Amount - 1;

        Amount -= amount;
        return Clone(amount);
    }

    // 수량을 지정한 복제본을 만드는 메서드 (자식 클래스에서 구현 필요)
    protected abstract CountableItem Clone(int amount);
}
