using System.Linq;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.PlayerWeapons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.Combat.PlayerWeapons.Modules
{
    public class HitscanWepModule : AttackWepModule
    {
        [SerializeField] protected int shotCount = 1;
        [SerializeField] protected bool randomizeShotCount;
        [SerializeField] protected int varianceShotCount;
        [SerializeField] private AmmunitionWepModule ammunitionModule;

        [Space] [SerializeField] [Min(0.001f)] protected float maxFireRate = 2f;
        [SerializeField] protected bool continuous = true;

        [Space] [SerializeField] [Tooltip("max angle shots can differ from actual aiming direction")]
        protected float inaccuracy;

        [Space] [SerializeField] protected float damagePerShot;
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected CombatGroup[] damageable;

        [Space] [SerializeField] protected Transform shotSourcePos;

        [SerializeField] protected UnityEvent<DamageInfo> onShotEffect;

        // [SerializeField] protected EffectSpawner shotEffect;
        // protected bool shotEffectNull;
        [SerializeField] protected UnityEvent onMuzzleFlashEffect;
        // [SerializeField] protected EffectSpawner muzzleFlashEffect;
        // protected bool muzzleFlashEffectNull;



        protected bool shooting;
        protected float shotInterval;
        protected float nextShotTime;

        private bool shotted;


        protected virtual void FireShot()
        {
            if (!ammunitionModule.TryUseAmmo())
                return;

            nextShotTime = Time.time + shotInterval;
            Vector3 ogDirection = transform.forward;

            onMuzzleFlashEffect.Invoke();
            // if (!muzzleFlashEffectNull)
            //     muzzleFlashEffect.SpawnEffect();

            int fixedShotCount = randomizeShotCount ? shotCount + Random.Range(-varianceShotCount, varianceShotCount + 1) : shotCount;
            Vector3[] hitPositions = new Vector3[fixedShotCount];

            for (int i = 0; i < fixedShotCount; i++)
            {
                Vector3 direction = (inaccuracy > 0)
                    ? Quaternion.Euler(0f, Random.Range(-inaccuracy / 2f, inaccuracy / 2f + 1f), 0f) * ogDirection
                    : ogDirection;

                Vector3 hitPos;

                if (Physics.Raycast(shotSourcePos.position, direction, out RaycastHit hit, 50f))
                {
                    hitPos = hit.point;

                    DamageableModule entity = hit.transform.GetComponent<DamageableModule>();
                    if (entity && damageable.Contains(entity.Entity.Group))
                        entity.Damage(damagePerShot, damageType, shotSourcePos.position, hit.point);
                }
                else
                    hitPos = shotSourcePos.position + direction * 50f;

                hitPositions[i] = hitPos;
                onShotEffect.Invoke(new DamageInfo(damagePerShot, damageType, shotSourcePos.position, hitPos));
                // if (!shotEffectNull)
                //     shotEffect.SpawnEffect(new DamageInfo(damagePerShot, damageType, shotSourcePos.position, hitPos));
            }

            shotted = true;
            FireShotsClientRpc(hitPositions);
        }

        [ClientRpc]
        private void FireShotsClientRpc(Vector3[] hitPositions)
        {
            if (shotted)
            {
                shotted = false;
                return;
            }

            onMuzzleFlashEffect.Invoke();
            // if (!muzzleFlashEffectNull)
            //     muzzleFlashEffect.SpawnEffect();

            for (int i = 0; i < hitPositions.Length; i++)
                onShotEffect.Invoke(new DamageInfo(damagePerShot, damageType, shotSourcePos.position, hitPositions[i]));
            // if (!shotEffectNull)
            // {
            //     for (int i = 0; i < hitPositions.Length; i++)
            //     {
            //         shotEffect.SpawnEffect(new DamageInfo(damagePerShot, damageType, shotSourcePos.position, hitPositions[i]));
            //     }
            // }
        }


        public override void UpdateModule()
        {
            if (shooting && nextShotTime < Time.time)
                FireShot();
        }

        public override void Attack(InputAction.CallbackContext context)
        {
            shooting = continuous;

            if (nextShotTime < Time.time)
                FireShot();
        }

        public override void AttackCancelled(InputAction.CallbackContext context)
        {
            shooting = false;
        }

        public override void InitializeModule(Weapon weapon, WeaponParent weaponParent)
        {
            base.InitializeModule(weapon, weaponParent);

            shotInterval = 1f / maxFireRate;

            // shotEffectNull = shotEffect == null;
            // muzzleFlashEffectNull = muzzleFlashEffect == null;
        }
    }
}