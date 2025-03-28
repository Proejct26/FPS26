using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float maxHealth = 100;
    public float curHealth = 0;

    public float maxMagazine = 30;
    public float curMagazine = 0;

    private void Awake()
    {


        curHealth = maxHealth;
        curMagazine = maxMagazine;
    }
}
