using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public static class Extension
{

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Util.GetOrAddComponent<T>(go);
    }
    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }

    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type); 
	}

	public static Vector3 RandomUnitVectorInCone(this Vector3 direction, float coneHalfAngleDegrees)
	{
		// 방향 벡터 정규화
        Vector3 dirNormalized = direction.normalized;
        
        // 각도를 라디안으로 변환
        float coneHalfAngleRad = coneHalfAngleDegrees * Mathf.Deg2Rad;
        
        // 랜덤 각도 생성 (0에서 coneHalfAngleRad 사이)
        float angle = Random.Range(0f, coneHalfAngleRad);
        
        // 랜덤 방향 각도 생성 (0에서 2π 사이)
        float rotationAngle = Random.Range(0f, 2f * Mathf.PI);
        
        // 구면 좌표계에서 데카르트 좌표계로 변환
        float sinAngle = Mathf.Sin(angle);
        float x = sinAngle * Mathf.Cos(rotationAngle);
        float y = sinAngle * Mathf.Sin(rotationAngle);
        float z = Mathf.Cos(angle);
        
        // 생성된 벡터 (Z 축을 중심으로 한 콘)
        Vector3 randomVector = new Vector3(x, y, z);
        
        // Z 축을 원하는 방향으로 회전시키기 위한 회전 계산
        if (dirNormalized != Vector3.forward)
        {
            // Z 축에서 원하는 방향으로의 회전을 계산
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, dirNormalized);
            randomVector = rotation * randomVector;
        }
        
        return randomVector;

	}
}
