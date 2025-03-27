using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 아이템 클래스 (장비 아이템의 하위 타입)
// 실제 인벤토리에 들어가는 무기 아이템 인스턴스를 표현함
// 생성 시 WeaponItemData를 받아 해당 데이터를 기반으로 초기화함
public class WeaponItem : EquipmentItem
{
    // 생성자에서 무기 아이템 데이터를 부모 클래스(EquipmentItem)에 전달
    public WeaponItem(WeaponItemData data) : base(data) { }
}
