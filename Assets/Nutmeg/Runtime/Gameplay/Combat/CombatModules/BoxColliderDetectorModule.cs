using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class BoxColliderDetectorModule : DetectorModule
    {
        [SerializeField] private CombatGroup[] targetGroups;
        [SerializeField] private Transform obstacleDetectorCenter;
        [SerializeField] private Vector3 obstacleDetectorHalfExtends = Vector3.one;
        [SerializeField] private int colliderBufferSize = 3;
        private Collider[] colliderBuffer;
        

        public override void InitializeModule(CombatEntity entity)
        {
            base.InitializeModule(entity);

            colliderBuffer = new Collider[colliderBufferSize];
        }
        
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

        public override List<CombatEntity> GetTargets()
        {
            List<CombatEntity> targets = new List<CombatEntity>();
            Collider[] colliders = Physics.OverlapBox(obstacleDetectorCenter.position, obstacleDetectorHalfExtends, obstacleDetectorCenter.rotation);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                CombatEntity entity = colliderBuffer[i].gameObject.GetComponent<CombatEntity>();
                if (!entity || !targetGroups.Contains(entity.Group))
                    continue;

                targets.Add(entity);
                mostRecentTarget = entity;
            }

            return targets;
        }

        public override int GetTargetsNonAlloc(CombatEntity[] targetBuffer)
        {
            int targetBufferSize = targetBuffer.Length;
            int targetBufferIndex = 0;
            int size = Physics.OverlapBoxNonAlloc(obstacleDetectorCenter.position, obstacleDetectorHalfExtends, colliderBuffer, obstacleDetectorCenter.rotation);
            
            for (int i = 0; i < size && targetBufferIndex < targetBufferSize; i++)
            {
                CombatEntity entity = colliderBuffer[i].gameObject.GetComponent<CombatEntity>();
                if (!entity || !targetGroups.Contains(entity.Group))
                    continue;

                targetBuffer[targetBufferIndex] = entity;
                mostRecentTarget = entity;
                targetBufferIndex++;
            }

            return targetBufferIndex;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(obstacleDetectorCenter.position, obstacleDetectorCenter.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, obstacleDetectorHalfExtends * 2);
        }
    }
}
