using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : WeaponBaseController
{
    protected override void Fire()
    {
        _weaponAnimationHandler.Fire();
    }


}
