using Nutmeg.Runtime.Gameplay.Combat;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Utility.Effects;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Base.NPCBehaviours
{
    public class NpcSniperBehaviour : MonoBehaviour
    {
        [SerializeField] private CombatEntity placeable;
        [SerializeField] private Transform weapon;
        [SerializeField] private Transform attackEffectOrigin;
        [SerializeField] private EffectSpawner effectSpawner;
        

        public void RotateToLastTarget()
        {
            if (!placeable.TryGetModule(typeof(RadiusDetectorModule), out CombatModule module))
                return;

            CombatEntity target = ((DetectorModule)module).MostRecentTarget;
            if (!target)
                return;

            Vector3 targetPos = target.transform.position;
            transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
            weapon.LookAt(targetPos);
        }

        public void SpawnAttackEffect()
        {
            if (!placeable.TryGetModule(typeof(RadiusDetectorModule), out CombatModule module))
                return;

            CombatEntity target = ((DetectorModule)module).MostRecentTarget;
            if (!target)
                return;

            effectSpawner.SpawnEffect(new DamageInfo(attackEffectOrigin.position, target.transform.position));
        }
    }
}