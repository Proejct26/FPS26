using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 음식 제작에 사용되는 재료 아이템의 데이터를 정의하는 ScriptableObject 클래스
// CountableItemData를 상속하여 수량 관리가 가능함
[CreateAssetMenu(fileName = "Item_FoodMaterial_", menuName = "Inventory System/Item Data/Food Material", order = 11)]
public class FoodMaterialItemData : CountableItemData
{
    // 이 데이터를 기반으로 음식 재료 아이템 인스턴스를 생성하여 반환
    public override Item CreateItem()
    {
        return new FoodMaterialItem(this);
    }
}
