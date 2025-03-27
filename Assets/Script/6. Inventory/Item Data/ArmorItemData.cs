using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// 장비 아이템 중 방어구에 해당하는 데이터를 정의하는 ScriptableObject 클래스
// EquipmentItemData를 상속하여 장착 가능한 아이템으로 처리됨
[CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
public class ArmorItemData : EquipmentItemData
{
    // 방어력 수치를 반환하는 프로퍼티 (읽기 전용)
    public int Defence => _defence;

    // 인스펙터에서 설정 가능한 방어력 수치 (기본값 1)
    [SerializeField] private int _defence = 1;

    // 이 데이터를 기반으로 실제 장비 아이템 인스턴스를 생성하여 반환
    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }
}
