using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 제작 재료 아이템 클래스
// 수량을 가지며, CountableItem을 상속함
public class WeaponMaterialItem : CountableItem
{
    // 생성자: 무기 재료 데이터와 수량을 받아 초기화
    public WeaponMaterialItem(WeaponMaterialItemData data, int amount = 1) : base(data, amount) { }

    // 수량 분리 또는 복제 시 사용되는 복제 메서드
    protected override CountableItem Clone(int amount)
    {
        return new WeaponMaterialItem(CountableData as WeaponMaterialItemData, amount);
    }
}
