using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    [상속 구조]

    ItemData(abstract)
        - CountableItemData(abstract)
            - PortionItemData
        - EquipmentItemData(abstract)
            - WeaponItemData
            - ArmorItemData

*/

// 아이템 데이터의 기반이 되는 추상 클래스 (ScriptableObject 기반)
// 인벤토리 시스템 내의 모든 아이템 데이터는 이 클래스를 상속함
public abstract class ItemData : ScriptableObject
{
    // 아이템 고유 ID (예: 데이터베이스 인덱스 또는 구분용)
    public int ID => _id;

    // 아이템 이름 (UI 등에 표시되는 이름)
    public string Name => _name;

    // 아이템 설명 텍스트 (툴팁 등에서 사용)
    public string Tooltip => _tooltip;

    // 아이템 아이콘 스프라이트 (UI에서 표시할 아이콘 이미지)
    public Sprite IconSprite => _iconSprite;

    // 아이템 고유 ID (수동 또는 자동 설정)
    [SerializeField] private int _id;

    // 인스펙터에서 설정하는 아이템 이름
    [SerializeField] private string _name;

    // 인스펙터에서 입력할 수 있는 멀티라인 설명 텍스트
    [Multiline]
    [SerializeField] private string _tooltip;

    // 아이템 UI에 표시할 아이콘 이미지
    [SerializeField] private Sprite _iconSprite;

    // 아이템을 버릴 경우 바닥에 생성될 프리팹 (드롭 아이템 프리팹)
    [SerializeField] private GameObject _dropItemPrefab;

    // 아이템 데이터에 맞는 새로운 인스턴스 생성 (파생 클래스에서 구체적으로 구현)
    public abstract Item CreateItem();
}
