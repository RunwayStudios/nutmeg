using System;
using System.Collections;
using Gameplay.Items;
using UnityEngine;

public abstract class Weapon : Item
{
    [SerializeField] protected WeaponStats stats;

    protected float fireRateCoolDown;
    protected int currentAmmunitionAmount;

    public float ReloadProgress { get; private set; }

    public override void Use()
    {
        if (ReloadProgress == 0f)
            Fire();
    }

    public void ReloadWeapon()
    {
        if (ReloadProgress == 0f && currentAmmunitionAmount != stats.magazineSize)
            StartCoroutine(ReloadEnumerator());
    }

    private float reloadTimer;
    private IEnumerator ReloadEnumerator()
    {
        while ((reloadTimer += Time.deltaTime) <= stats.reloadTime)
        {
            ReloadProgress = reloadTimer / stats.reloadTime;
            yield return null;
        }

        ReloadProgress = reloadTimer = 0f;
        currentAmmunitionAmount = stats.magazineSize;
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
        return true;
    }
}