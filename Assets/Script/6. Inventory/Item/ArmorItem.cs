using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 장비 아이템 중 '방어구'를 나타내는 클래스
// EquipmentItem을 상속받아 기본 장비 기능을 제공
public class ArmorItem : EquipmentItem
{
    // 생성자: ArmorItemData를 받아 부모 클래스에 전달
    public ArmorItem(ArmorItemData data) : base(data) { }
}
