using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 제작에 사용되는 재료 아이템의 데이터를 정의하는 ScriptableObject 클래스
// 수량을 가질 수 있으므로 CountableItemData를 상속함
[CreateAssetMenu(fileName = "Item_WeaponMaterial_", menuName = "Inventory System/Item Data/Weapon Material", order = 10)]
public class WeaponMaterialItemData : CountableItemData
{
    // 이 데이터를 기반으로 무기 재료 아이템 인스턴스를 생성하여 반환
    public override Item CreateItem()
    {
        return new WeaponMaterialItem(this);
    }
}
