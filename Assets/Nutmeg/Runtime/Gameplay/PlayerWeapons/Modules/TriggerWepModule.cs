using System.Collections.Generic;
using Nutmeg.Runtime.Utility.GameObjectPooling;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons.Modules
{
    public class TriggerWepModule : AttackWepModule
    {
        [SerializeField] private GameObject triggerPrefab;
        [SerializeField] private Transform spawnPos;
        [SerializeField] private float lifetime;
        [SerializeField] private Vector3 velocity;
        [SerializeField] private bool moveInLocalSpace;

        [Space] [SerializeField] private bool continuous;
        [SerializeField] private float spawningInterval;
        


        private List<TriggerHitDetector> activeTriggers = new List<TriggerHitDetector>();


        private void SpawnTrigger()
        {
            GameObject triggerGo = GoPoolingManager.Main.Get(triggerPrefab);
            TriggerHitDetector trigger = triggerGo.GetComponent<TriggerHitDetector>();
            trigger.Initialize(transform, go => GoPoolingManager.Main.Return(go, triggerPrefab));
        }
        
        protected override void Attack(InputAction.CallbackContext context)
        {
            base.Attack(context);
        }

        protected override void AttackCancelled(InputAction.CallbackContext context)
        {
            base.AttackCancelled(context);
        }

        public override void DestroyModule()
        {
            base.DestroyModule();
        }

        public override void UpdateModule()
        {
            base.UpdateModule();
        }
    }
}