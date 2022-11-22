using System;
using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nutmeg.Runtime.Utility.Effects.EffectTypes
{
    public class BodySplatterEffect : Effect
    {
        [SerializeField] private Transform[] bodyParts;
        [SerializeField] private int minParts = 0;
        [SerializeField] private int maxParts = 0;
        [SerializeField] private float force = 0f;
        [SerializeField] private float forceMaxVariance = 0f;
        
        [Space] [SerializeField] private float timeActive = 3f;
        [SerializeField] private float timeDecay = 1f;
        [SerializeField] private float decayDisplacement = -.5f;

        private float timeStart;
        private bool decaying;
 

        private List<Transform> activeParts = new List<Transform>();
        private List<Vector3> activePartsPositions = new List<Vector3>();
        private List<Quaternion> activePartsRotations = new List<Quaternion>();


        public override void Initialize(Action<GameObject> finishedAction)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(DamageInfo info, Action<GameObject> finishedAction)
        {
            timeStart = Time.time;
            
            for (int i = 0; i < Random.Range(minParts, maxParts + 1); i++)
            {
                Transform child = bodyParts[Random.Range(0, bodyParts.Length)];
                if (activeParts.Contains(child))
                    continue;

                activeParts.Add(child);
                activePartsPositions.Add(child.localPosition);
                activePartsRotations.Add(child.localRotation);
                child.gameObject.SetActive(true);

                child.GetComponent<Collider>().enabled = true;
                Rigidbody rigidbody = child.GetComponent<Rigidbody>();
                rigidbody.constraints = RigidbodyConstraints.None;
                rigidbody.velocity = (child.transform.position - info.SourcePos + Vector3.up).normalized *
                                                           (force + Random.Range(-forceMaxVariance, +forceMaxVariance));
            }

            base.Initialize(info, finishedAction);
        }

        private void Update()
        {
            if (timeStart + timeActive + timeDecay < Time.time)
            {
                Finished();
                decaying = false;
            }
            else if (timeStart + timeActive < Time.time)
            {
                if (!decaying)
                {
                    decaying = true;
                    
                    for (int i = 0; i < activeParts.Count; i++)
                    {
                        activeParts[i].GetComponent<Collider>().enabled = true;
                        Rigidbody rigidbody = activeParts[i].GetComponent<Rigidbody>();
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    }
                }
                
                transform.position = new Vector3(transform.position.x, 
                    transform.position.y + decayDisplacement * (Time.deltaTime / timeDecay), transform.position.z);
            }
        }

        protected override void Finished()
        {
            for (int i = 0; i < activeParts.Count; i++)
            {
                // activeParts[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                activeParts[i].localPosition = activePartsPositions[i];
                activeParts[i].localRotation = activePartsRotations[i];
                activeParts[i].gameObject.SetActive(false);
            }

            activeParts.Clear();
            activePartsPositions.Clear();
            activePartsRotations.Clear();

            base.Finished();
        }
    }
}