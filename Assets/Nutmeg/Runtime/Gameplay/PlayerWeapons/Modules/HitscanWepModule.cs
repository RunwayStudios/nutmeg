using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Utility.Effects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons.Modules
{
    public class HitscanWepModule : AttackWepModule
    {
        [SerializeField] protected int shotCount = 1;
        [SerializeField] protected bool randomizeShotCount;
        [SerializeField] protected int varianceShotCount;

        [Space] [SerializeField] [Min(0.001f)] protected float maxFireRate = 2f;
        [SerializeField] protected bool continuous = true;

        [Space] [SerializeField] [Tooltip("max angle shots can differ from actual aiming direction")]
        protected float inaccuracy;

        [Space] [SerializeField] protected float damagePerShot;
        [SerializeField] protected DamageType damageType;

        [Space] [SerializeField] protected Transform shotSourcePos;
        [SerializeField] protected EffectSpawner shotEffect;
        protected bool shotEffectNull;
        [SerializeField] protected EffectSpawner muzzleFlashEffect;
        protected bool muzzleFlashEffectNull;


        protected bool shooting;
        protected float shotInterval;
        protected float nextShotTime;

        private bool shotted;


        protected virtual void FireShot()
        {
            nextShotTime = Time.time + shotInterval;
            Vector3 ogDirection = transform.forward;
            
            if (!muzzleFlashEffectNull)
                muzzleFlashEffect.SpawnEffect();

            int fixedShotCount = randomizeShotCount ? shotCount + Random.Range(-varianceShotCount, varianceShotCount + 1) : shotCount;

            for (int i = 0; i < fixedShotCount; i++)
            {
                Vector3 direction = (inaccuracy > 0)
                    ? Quaternion.Euler(0f, Random.Range(-inaccuracy / 2f, inaccuracy / 2f + 1f), 0f) * ogDirection
                    : ogDirection;

                Vector3 hitPos;

                if (Physics.Raycast(shotSourcePos.position, direction, out RaycastHit hit, 100f))
                {
                    hitPos = hit.point;

                    DamageableModule entity = hit.transform.GetComponent<DamageableModule>();
                    if (!entity)
                        continue;

                    entity.Damage(damagePerShot, damageType, shotSourcePos.position, hit.point);
                }
                else
                    hitPos = shotSourcePos.position + direction * 50;
                
                if (!shotEffectNull)
                    shotEffect.SpawnEffect(new DamageInfo(damagePerShot, damageType, shotSourcePos.position, hitPos));
                
                
                shotted = true;
                FireShotClientRpc(hitPos);
            }
        }

        [ClientRpc]
        private void FireShotClientRpc(Vector3 hitPos)
        {
            if (shotted)
            {
                shotted = false;
                return;
            }
            
            if (!muzzleFlashEffectNull)
                muzzleFlashEffect.SpawnEffect();
            
            if (!shotEffectNull)
                shotEffect.SpawnEffect(new DamageInfo(damagePerShot, damageType, shotSourcePos.position, hitPos));
        }


        public override void UpdateModule()
        {
            if (shooting && nextShotTime < Time.time)
                FireShot();
        }

        protected override void Attack(InputAction.CallbackContext context)
        {
            shooting = continuous;

            if (shooting && nextShotTime < Time.time)
                FireShot();
        }

        protected override void AttackCancelled(InputAction.CallbackContext context)
        {
            shooting = false;
        }

        public override void InitializeModule(Weapon weapon, WeaponParent weaponParent)
        {
            base.InitializeModule(weapon, weaponParent);

            shotInterval = 1f / maxFireRate;

            shotEffectNull = shotEffect == null;
            muzzleFlashEffectNull = muzzleFlashEffect == null;
        }
    }
}