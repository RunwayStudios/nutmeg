using Nutmeg.Runtime.Utility.GameObjectPooling;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public class EffectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private Transform effectPosition;
        [SerializeField] private Transform effectParent;
        [SerializeField] private bool stickToParent;

        public void SpawnEffect()
        {
            if (stickToParent)
            {
                GameObject go = GoPoolingManager.Main.Get(effectPrefab);
                go.transform.position = effectPosition.position;
                go.transform.SetParent(effectParent);
                go.GetComponent<ParticleEffect>().Initialize(Finished);
            }
            else
            {
                GameObject go = GoPoolingManager.Main.Get(effectPrefab);
                go.transform.position = effectPosition.position;
                go.GetComponent<ParticleEffect>().Initialize(Finished);
            }
        }

        public void SpawnEffect(Vector3 pos)
        {
            GameObject go = GoPoolingManager.Main.Get(effectPrefab);
            go.transform.position = pos;
            go.GetComponent<ParticleEffect>().Initialize(Finished);
        }

        public void SpawnEffect(Transform parent)
        {
            GameObject go = GoPoolingManager.Main.Get(effectPrefab);
            go.transform.position = parent.position;
            go.transform.SetParent(parent);
            go.GetComponent<ParticleEffect>().Initialize(Finished);
        }
        
        public void SpawnEffect(Vector3 pos, Transform parent)
        {
            GameObject go = GoPoolingManager.Main.Get(effectPrefab);
            go.transform.position = pos;
            go.transform.SetParent(parent);
            go.GetComponent<ParticleEffect>().Initialize(Finished);
        }
        
        private void Finished(GameObject go)
        {
            GoPoolingManager.Main.Return(go, effectPrefab);
        }
    }
}