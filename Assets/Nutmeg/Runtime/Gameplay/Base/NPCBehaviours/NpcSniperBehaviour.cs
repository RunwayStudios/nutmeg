using Nutmeg.Runtime.Gameplay.Combat;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Utility.Effects;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Base.NPCBehaviours
{
    public class NpcSniperBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject placeable;

        [SerializeField] private Transform AttackEffectOrigin;
        
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

            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        }

        public void SpawnAttackEffect()
        {
            if (!placeable.GetComponent<CombatEntity>().TryGetModule(typeof(RadiusDetectorModule), out CombatModule module))
                return;

            CombatEntity target = ((DetectorModule)module).MostRecentTarget;
            if (!target)
                return;

            GetComponent<AttackEffectSpawner>().Spawn(AttackEffectOrigin.position, target.transform.position);
        }
    }
}