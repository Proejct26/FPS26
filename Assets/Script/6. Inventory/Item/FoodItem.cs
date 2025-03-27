using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 음식 아이템 클래스
// 수량이 존재하는 소비형 아이템이며, 사용 가능하므로 IUsableItem을 구현
public class FoodItem : CountableItem, IUsableItem
{
    // 생성자: 음식 데이터와 수량을 받아 초기화
    public FoodItem(FoodItemData data, int amount = 1) : base(data, amount) { }

    // 아이템 사용 시 호출됨 - 수량 감소 및 효과 처리
    public bool Use()
    {
        if (Amount <= 0)
            return false;

        Amount--;

        // 실제 회복 효과는 외부에서 구현
        Debug.Log($"{CountableData.Name}을 사용하여 체력을 {(CountableData as FoodItemData).HealAmount} 회복합니다.");

        return true;
    }

    // 수량을 분할하거나 클론할 때 호출되는 복제 메서드
    protected override CountableItem Clone(int amount)
    {
        return new FoodItem(CountableData as FoodItemData, amount);
    }
}
