using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new WeaponStats", menuName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public float accuracy;
    public float recoil;
    public float damage;
    public float fireRate;
    public float magazineSize;
    public float reloadTime;

    public enum fireType
    {
        single,
        fullAuto
    }
}