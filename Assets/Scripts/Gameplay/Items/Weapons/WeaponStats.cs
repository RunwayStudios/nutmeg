using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new WeaponStats", menuName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public float accuracy;
    public float recoil;
    public float damage;
    public float fireRate;
    public int magazineSize;
    public float reloadTime;
    public float range;

    public enum fireType
    {
        single,
        fullAuto
    }
}