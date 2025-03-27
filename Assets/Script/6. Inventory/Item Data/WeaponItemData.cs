using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// 무기 아이템 데이터를 정의하는 ScriptableObject 클래스
// 장비 아이템 데이터(EquipmentItemData)를 상속받아 무기 고유의 속성을 추가로 가짐
[CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weaopn", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    // 공격력 수치를 외부에서 읽을 수 있도록 제공
    public int Damage => _damage;

    // 인스펙터에서 설정할 수 있는 공격력 수치 (기본값 1)
    [SerializeField] private int _damage = 1;

    // 무기 아이템을 생성할 때 호출되는 팩토리 메서드
    // 이 데이터 객체를 기반으로 WeaponItem 인스턴스를 생성함
    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}
