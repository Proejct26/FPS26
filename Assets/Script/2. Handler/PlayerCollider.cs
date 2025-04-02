using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
    Head,
    Body,
    Arm,
    Hand,
    Leg,
    Foot
}

public class PlayerCollider : MonoBehaviour
{
    public static readonly float headDamage = 2;
    public static readonly float bodyDamage = 1;
    public static readonly float armDamage = 0.5f;
    public static readonly float handDamage = 0.3f;
    public static readonly float legDamage = 0.7f;
    public static readonly float footDamage = 0.4f;  

    public ColliderType colliderType;
    public GameObject Parent {get; set;}

    public float GetDamage()
    {
        switch (colliderType)
        {
            case ColliderType.Head: return headDamage;
            case ColliderType.Body: return bodyDamage;
            case ColliderType.Arm: return armDamage;
            case ColliderType.Hand: return handDamage;
            case ColliderType.Leg: return legDamage;
            case ColliderType.Foot: return footDamage;
        } 
        return 0f;
    }
    
}
 