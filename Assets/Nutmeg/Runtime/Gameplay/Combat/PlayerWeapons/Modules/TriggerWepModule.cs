using System.Collections.Generic;
using System.Linq;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.PlayerWeapons;
using Nutmeg.Runtime.Utility.GameObjectPooling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.Combat.PlayerWeapons.Modules
{
    public class TriggerWepModule : AttackWepModule
    {
        [Space] [SerializeField] private float damage;
        [SerializeField] private float damageInterval;
        [SerializeField] private DamageType type;
        [SerializeField] protected CombatGroup[] damageable;

        [Space] [SerializeField] private GameObject triggerPrefab;
        [SerializeField] private Transform spawnPos;
        [SerializeField] private bool continuous;
        [SerializeField] private float spawningInterval;

        [Space] [SerializeField] private UnityEvent onAttack;
        [SerializeField] private UnityEvent onAttackCancelled;

        private List<TriggerHitDetector> activeTriggers = new List<TriggerHitDetector>();
        private bool attacking;
        private float nextSpawn;
        private float nextDamage;
        private List<Transform> collisions = new List<Transform>();


        private void SpawnTrigger()
        {
            GameObject triggerGo = GoPoolingManager.Main.Get(triggerPrefab);
            TriggerHitDetector trigger = triggerGo.GetComponent<TriggerHitDetector>();
            activeTriggers.Add(trigger);
            trigger.Initialize(spawnPos, go =>
            {
                activeTriggers.Remove(trigger);
                GoPoolingManager.Main.Return(go, triggerPrefab);
            });
        }

        private void DealDamage()
        {
            for (int i = 0; i < activeTriggers.Count; i++)
            {
                activeTriggers[i].AddNewCollisions(collisions);
            }

            for (int i = 0; i < collisions.Count; i++)
            {
                DamageableModule entity = collisions[i].GetComponent<DamageableModule>();
                if (entity && damageable.Contains(entity.Entity.Group))
                    entity.Damage(damage, type, spawnPos.position);
            }

            collisions.Clear();
        }

        public override void Attack(InputAction.CallbackContext context)
        {
            onAttack.Invoke();
            attacking = true;
            if (!continuous)
                SpawnTrigger();

            base.Attack(context);
        }

        public override void AttackCancelled(InputAction.CallbackContext context)
        {
            base.AttackCancelled(context);

            attacking = false;
            onAttackCancelled.Invoke();
        }

        public override void DestroyModule()
        {
            base.DestroyModule();
        }

        public override void UpdateModule()
        {
            if (continuous && attacking && Time.time > nextSpawn)
            {
                SpawnTrigger();
                nextSpawn = Time.time + spawningInterval;
            }

            if (Time.time > nextDamage && activeTriggers.Count > 0)
            {
                DealDamage();
                nextDamage = Time.time + damageInterval;
            }
        }
    }
}