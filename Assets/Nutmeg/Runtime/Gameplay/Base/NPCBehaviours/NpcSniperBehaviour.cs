using Nutmeg.Runtime.Gameplay.Combat;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Utility.Effects;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Base.NPCBehaviours
{
    public class NpcSniperBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject placeable;
        [SerializeField] private Transform weapon;
        [SerializeField] private Transform attackEffectOrigin;
        
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void RotateToLastTarget()
        {
            if (!placeable.GetComponent<CombatEntity>().TryGetModule(typeof(RadiusDetectorModule), out CombatModule module))
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
            if (!placeable.GetComponent<CombatEntity>().TryGetModule(typeof(RadiusDetectorModule), out CombatModule module))
                return;

            CombatEntity target = ((DetectorModule)module).MostRecentTarget;
            if (!target)
                return;

            GetComponent<AttackEffectSpawner>().Spawn(attackEffectOrigin.position, target.transform.position);
        }
    }
}