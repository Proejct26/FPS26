using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 소비 가능한 음식 아이템 데이터를 정의하는 ScriptableObject 클래스
// CountableItemData를 상속하여 수량을 가질 수 있으며, 사용 시 효과를 발동함
[CreateAssetMenu(fileName = "Item_Food_", menuName = "Inventory System/Item Data/Food", order = 4)]
public class FoodItemData : CountableItemData
{
    // 음식 사용 시 회복되는 수치를 반환 (읽기 전용)
    public int HealAmount => _healAmount;

    // 인스펙터에서 설정 가능한 회복량 수치 (기본값 20)
    [SerializeField] private int _healAmount = 20;

    // 이 데이터를 기반으로 음식 아이템 인스턴스를 생성하여 반환
    public override Item CreateItem()
    {
        return new FoodItem(this);
    }
}
