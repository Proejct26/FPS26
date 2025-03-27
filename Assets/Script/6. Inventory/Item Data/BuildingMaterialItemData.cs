using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 건축 제작에 사용되는 재료 아이템의 데이터를 정의하는 ScriptableObject 클래스
// CountableItemData를 상속하여 수량이 존재함
[CreateAssetMenu(fileName = "Item_BuildingMaterial_", menuName = "Inventory System/Item Data/Building Material", order = 12)]
public class BuildingMaterialItemData : CountableItemData
{
    // 이 데이터를 기반으로 건축 재료 아이템 인스턴스를 생성하여 반환
    public override Item CreateItem()
    {
        return new BuildingMaterialItem(this);
    }
}
