using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class PathfindFirstAttackerModule : AttackerModule
    {
        [SerializeField] private float attackDistance = 1f;
        [SerializeField] private NavMeshAgent agent;


        public override void UpdateModule()
        {
            if (!ShouldTryAttack()) return;

            if (detector.TryGetTarget(out CombatEntity target))
            {
                // attacking
                if (((RadiusDetectorModule)detector).LastFoundDistance <= attackDistance)
                {
                    if (!attacking)
                    {
                        OnStartAttacking.Invoke();
                    }

                    Attack(target);
                    if (!attacking)
                    {
                        OnStartedAttacking.Invoke();
                        attacking = true;
                    }

                    OnAttack.Invoke();
                }
                else
                {
                    if (agent.destination != target.transform.position)
                        agent.SetDestination(target.transform.position);

                    if (attacking)
                    {
                        attacking = false;
                        OnStoppedAttacking.Invoke();
                    }
                }
            }
            else if (attacking)
            {
                attacking = false;
                OnStoppedAttacking.Invoke();
            }
        }


        private void OnValidate()
        {
            if (detector && detector.GetType() != typeof(RadiusDetectorModule))
            {
                detector = null;
                Debug.LogError("PathfindFirstAttackerModule requires a RadiusDetectorModule");
            }
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Handles.color = new Color(0.5f, 0.1f, 0.1f);
            Handles.DrawWireDisc(transform.position, Vector3.up, attackDistance);
#endif
        }
    }
}