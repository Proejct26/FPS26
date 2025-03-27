using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 음식 제작 재료 아이템 클래스
// CountableItem을 상속하여 수량이 존재함
public class FoodMaterialItem : CountableItem
{
    // 생성자: 음식 재료 데이터와 수량을 받아 초기화
    public FoodMaterialItem(FoodMaterialItemData data, int amount = 1) : base(data, amount) { }

    // 수량 복제 메서드 - 분할 또는 클론 시 호출
    protected override CountableItem Clone(int amount)
    {
        return new FoodMaterialItem(CountableData as FoodMaterialItemData, amount);
    }
}
