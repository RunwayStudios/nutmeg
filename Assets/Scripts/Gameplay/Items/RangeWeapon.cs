using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class RangeWeapon : Weapon
{
    protected override bool Fire()
    {
        if(!base.Fire()) return false; 
        Debug.Log("PEW");
        return true;
    }
}