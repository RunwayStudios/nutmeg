using System.Linq;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class BoxColliderDetectorModule : DetectorModule
    {
        [SerializeField] private CombatGroup[] targetGroups;
        [SerializeField] private Transform obstacleDetectorCenter;
        [SerializeField] private Vector3 obstacleDetectorHalfExtends = Vector3.one;

        private Collider[] colliderBuffer = new Collider[5];
        

        public override bool TryGetTarget(out CombatEntity target)
        {
            target = null;
            int size = Physics.OverlapBoxNonAlloc(obstacleDetectorCenter.position, obstacleDetectorHalfExtends, colliderBuffer, obstacleDetectorCenter.rotation);
            
            for (int i = size - 1; i >= 0; i--)
            {
                CombatEntity entity = colliderBuffer[i].gameObject.GetComponent<CombatEntity>();
                if (!entity || !targetGroups.Contains(entity.Group))
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
