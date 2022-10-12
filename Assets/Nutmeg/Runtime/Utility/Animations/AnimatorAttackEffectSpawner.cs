using Nutmeg.Runtime.Gameplay.Combat;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Zombies;
using Nutmeg.Runtime.Utility.Effects;
using Nutmeg.Runtime.Utility.GameObjectPooling;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Animations
{
    public class AnimatorAttackEffectSpawner : StateMachineBehaviour
    {
        [SerializeField] private GameObject prefab;
        private Transform origin;
        

        public void Finished(GameObject go)
        {
            GoPoolingManager.Main.Return(go, prefab);
        }
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        { 
            if (!animator.GetComponent<CombatEntity>().TryGetModule(typeof(RadiusDetectorModule), out CombatModule module))
                return;

            CombatEntity targetEntity = ((DetectorModule)module).MostRecentTarget;
            if (!targetEntity)
                return;
            
            if (!origin && !(origin = animator.GetComponent<Zombie>().bulletSource))
                return;
            
            GameObject go = GoPoolingManager.Main.Get(prefab);
            AttackEffect attackEffect = go.GetComponent<AttackEffect>();
            attackEffect.Initialize(origin.position, targetEntity.transform.position, Finished);
        }
    }
}
