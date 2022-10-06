using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class BoxColliderDetectorModule : DetectorModule
    {
        [SerializeField] private CombatGroup targetGroup;
        [SerializeField] private Transform obstacleDetectorCenter;
        [SerializeField] private Vector3 obstacleDetectorHalfExtends = Vector3.one;
        
        

        public override bool TryGetTarget(out CombatEntity target)
        {
            target = null;
            Collider[] colliders = Physics.OverlapBox(obstacleDetectorCenter.position, obstacleDetectorHalfExtends, obstacleDetectorCenter.rotation);
            
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                CombatEntity entity = colliders[i].gameObject.GetComponent<CombatEntity>();
                if (!entity || entity.Group != targetGroup)
                    continue;

                target = entity;
                mostRecentTarget = target;
                return true;
            }

            return false;
        }
        
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(obstacleDetectorCenter.position, obstacleDetectorCenter.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, obstacleDetectorHalfExtends * 2);
        }
    }
}
