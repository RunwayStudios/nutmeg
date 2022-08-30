using UnityEditor;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class RadiusDetectorModule : DetectorModule
    {
        [SerializeField] private CombatGroup targetGroup;
        [SerializeField] private float radius = 5f;


        public override bool TryGetTarget(out CombatEntity target)
        {
            target = null;
            if (!CombatEntityManager.Main.activeGroups.TryGetValue(targetGroup, out var targets))
                return false;

            float lowestDistance = float.MaxValue;
            bool found = false;
            int closestIndex = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                float distance = new Vector2(transform.position.x - targets[i].Transform.position.x, transform.position.z - targets[i].Transform.position.z).magnitude;
                if (distance < radius && distance < lowestDistance)
                {
                    lowestDistance = distance;
                    closestIndex = i;
                    found = true;
                }
            }

            if (found)
                target = targets[closestIndex].Entity;
            
            return found;
        }

        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(transform.position, Vector3.up, radius);
        }
    }
}