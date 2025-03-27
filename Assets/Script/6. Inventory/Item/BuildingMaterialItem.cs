using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 건축 재료 아이템 클래스
// CountableItem을 상속하여 수량 관리 기능을 포함함
public class BuildingMaterialItem : CountableItem
{
    // 생성자: 건축 재료 데이터와 수량을 받아 초기화
    public BuildingMaterialItem(BuildingMaterialItemData data, int amount = 1) : base(data, amount) { }

    // 수량 복제 메서드 - 클론 생성에 사용됨
    protected override CountableItem Clone(int amount)
    {
        return new BuildingMaterialItem(CountableData as BuildingMaterialItemData, amount);
    }
}
