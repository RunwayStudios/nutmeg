using System;
using UnityEngine;

public abstract class Weapon : Item
{
    [SerializeField] protected WeaponStats stats;

    protected float fireRateCoolDown;

    public override void Use()
    {
        Fire();
    }

    private void Update()
    {
        UpdateFireRateTimer();
    }

    private void UpdateFireRateTimer()
    {
        fireRateCoolDown = Mathf.Clamp(fireRateCoolDown -= Time.deltaTime, 0f, float.MaxValue);
    }

    protected virtual bool Fire()
    {
        if (fireRateCoolDown != 0) return false;

        fireRateCoolDown = 1f / (stats.fireRate / 60f);

        Debug.DrawRay(transform.position, transform.forward * 100f, Color.red, 1f / (stats.fireRate / 60f));
        return true;
    }
}