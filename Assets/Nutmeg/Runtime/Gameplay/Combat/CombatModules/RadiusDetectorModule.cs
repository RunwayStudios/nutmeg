using UnityEditor;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class RadiusDetectorModule : DetectorModule
    {
        [SerializeField] private CombatGroup targetGroup;
        [SerializeField] private float fromRadius = 1f;
        [SerializeField] private float toRadius = 5f;


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
                if (distance > fromRadius && distance < toRadius && distance < lowestDistance)
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
            Handles.color = new Color(0f, 0.5f, 0.5f);
            Handles.DrawWireDisc(transform.position, Vector3.up, fromRadius);
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(transform.position, Vector3.up, toRadius);
        }
    }
}