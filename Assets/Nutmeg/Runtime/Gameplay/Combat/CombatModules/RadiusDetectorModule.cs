using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class RadiusDetectorModule : DetectorModule
    {
        [SerializeField] private CombatGroup[] targetGroups;
        [SerializeField] private bool setPriority = false;
        [SerializeField] private CombatGroup priorityTarget;
        [Space] [SerializeField] private float fromRadius = 1f;
        [SerializeField] private float toRadius = 5f;

        private float lastFoundDistance;


        public override bool TryGetTarget(out CombatEntity target)
        {
            target = null;
            bool found = false;
            bool foundPrioritised = !setPriority;
            float lowestDistance = float.MaxValue;

            for (int j = 0; j < targetGroups.Length; j++)
            {
                // only one prioritised group - if we're switching groups but already found a priority target we can skip the others
                if (foundPrioritised && setPriority)
                    continue;

                if (!CombatEntityManager.Main.activeGroups.TryGetValue(targetGroups[j], out var targets))
                    continue;

                for (int i = 0; i < targets.Count; i++)
                {
                    float distance = new Vector2(transform.position.x - targets[i].Transform.position.x,
                        transform.position.z - targets[i].Transform.position.z).magnitude;

                    if (distance > fromRadius && distance < toRadius &&
                        (distance < lowestDistance || (targetGroups[j] == priorityTarget && !foundPrioritised)))
                    {
                        lowestDistance = distance;
                        found = true;
                        foundPrioritised = !setPriority || targetGroups[j] == priorityTarget;
                        target = targets[i].Entity;

                        mostRecentTarget = target;
                        lastFoundDistance = lowestDistance;
                    }
                }
            }

            return found;
        }

        public override List<CombatEntity> GetTargets()
        {
            List<CombatEntity> targetsOut = new List<CombatEntity>();

            for (int j = 0; j < targetGroups.Length; j++)
            {
                if (!CombatEntityManager.Main.activeGroups.TryGetValue(targetGroups[j], out var targets))
                    continue;

                for (int i = 0; i < targets.Count; i++)
                {
                    float distance = new Vector2(transform.position.x - targets[i].Transform.position.x,
                        transform.position.z - targets[i].Transform.position.z).magnitude;

                    if (distance > fromRadius && distance < toRadius)
                    {
                        targetsOut.Add(targets[i].Entity);

                        mostRecentTarget = targets[i].Entity;
                        lastFoundDistance = distance;
                    }
                }
            }

            return targetsOut;
        }

        public override int GetTargetsNonAlloc(CombatEntity[] targetBuffer)
        {
            int targetBufferSize = targetBuffer.Length;
            int targetBufferIndex = 0;

            for (int j = 0; j < targetGroups.Length; j++)
            {
                if (!CombatEntityManager.Main.activeGroups.TryGetValue(targetGroups[j], out var targets))
                    continue;

                for (int i = 0; i < targets.Count && targetBufferIndex < targetBufferSize; i++)
                {
                    float distance = new Vector2(transform.position.x - targets[i].Transform.position.x,
                        transform.position.z - targets[i].Transform.position.z).magnitude;

                    if (distance > fromRadius && distance < toRadius)
                    {
                        targetBuffer[targetBufferIndex] = targets[i].Entity;
                        targetBufferIndex++;
                        
                        mostRecentTarget = targets[i].Entity;
                        lastFoundDistance = distance;
                    }
                }
            }

            return targetBufferIndex;
        }

        public float LastFoundDistance => lastFoundDistance;
        
        

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Handles.color = new Color(0f, 0.5f, 0.5f);
            Handles.DrawWireDisc(transform.position, Vector3.up, fromRadius);
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(transform.position, Vector3.up, toRadius);
#endif
        }
    }
}