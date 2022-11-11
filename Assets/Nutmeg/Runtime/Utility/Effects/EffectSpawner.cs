using System;
using Nutmeg.Runtime.Utility.GameObjectPooling;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Utility.Effects
{
    public class EffectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private Transform effectPosition;
        [SerializeField] private Transform effectParent;
        [SerializeField] private bool stickToParent;

        [Space] [SerializeField] private bool testEffect;

        [SerializeField] private UnityEvent OnFinished;

        public void SpawnEffect()
        {
            if (stickToParent)
            {
                GameObject go = GoPoolingManager.Main.Get(effectPrefab);
                go.transform.position = effectPosition.position;
                go.transform.rotation = effectPosition.rotation;
                go.transform.SetParent(effectParent);
                go.GetComponent<ParticleEffect>().Initialize(Finished);
            }
            else
            {
                GameObject go = GoPoolingManager.Main.Get(effectPrefab);
                go.transform.position = effectPosition.position;
                go.transform.rotation = effectPosition.rotation;
                go.GetComponent<ParticleEffect>().Initialize(Finished);
            }
        }

        public void SpawnEffect(Vector3 pos)
        {
            GameObject go = GoPoolingManager.Main.Get(effectPrefab);
            go.transform.position = pos;
            go.transform.rotation = effectPosition.rotation;
            go.GetComponent<ParticleEffect>().Initialize(Finished);
        }

        public void SpawnEffect(Transform parent)
        {
            GameObject go = GoPoolingManager.Main.Get(effectPrefab);
            go.transform.position = parent.position;
            go.transform.rotation = effectPosition.rotation;
            go.transform.SetParent(parent);
            go.GetComponent<ParticleEffect>().Initialize(Finished);
        }

        public void SpawnEffect(Vector3 pos, Transform parent)
        {
            GameObject go = GoPoolingManager.Main.Get(effectPrefab);
            go.transform.position = pos;
            go.transform.rotation = effectPosition.rotation;
            go.transform.SetParent(parent);
            go.GetComponent<ParticleEffect>().Initialize(Finished);
        }

        private void Finished(GameObject go)
        {
            GoPoolingManager.Main.Return(go, effectPrefab);
            OnFinished.Invoke();
        }

        
        private void OnValidate()
        {
            if (testEffect)
            {
                testEffect = false;
                SpawnEffect();
            }
        }
    }
}