using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    [상속 구조]
    Item : 기본 아이템
        - EquipmentItem : 장비 아이템
        - CountableItem : 수량이 존재하는 아이템
*/

// 아이템 인스턴스의 최상위 추상 클래스
// 모든 인게임 아이템 객체는 이 클래스를 기반으로 생성됨
public abstract class Item
{
    // 아이템이 참조하는 ScriptableObject 기반의 아이템 데이터
    public ItemData Data { get; private set; }

    // 생성자에서 아이템 데이터 참조를 설정
    public Item(ItemData data) => Data = data;
}
