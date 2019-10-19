using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MySword : MyWeapon
{
    public override void WeaponAttacking(bool turner)
    {
        _successAttack = new List<Body>();
        _noSuccessAttack = new List<Body>();
        if (_weaponTrailRenderer) _weaponTrailRenderer.enabled = turner;
        if (_weaponMesh) _weaponMesh.enabled = turner;
        _weaponAttacking = turner;
        _atkIsBlocked = false;
    }
}