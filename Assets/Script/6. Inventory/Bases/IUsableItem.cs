using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템이 사용될 수 있다는 기능을 명시하는 인터페이스
// 예: 포션 사용, 장비 착용 등
public interface IUsableItem
{
    // 아이템을 사용하고, 사용이 성공했는지 여부를 반환
    // true: 사용 성공, false: 사용 실패
    bool Use();
}
