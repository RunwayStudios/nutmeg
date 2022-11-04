using System;
using Nutmeg.Runtime.Utility.GameObjectPooling;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public class AttackEffectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        private void Awake()
        {
            if (!prefab)
            {
                Debug.LogError("AttackEffectSpawner missing prefab! " + gameObject.ToString());
            }
            else if (!prefab.GetComponent<AttackEffect>())
            {
                Debug.LogError("AttackEffect Prefab missing AttackEffect Script! " + prefab.ToString());
            }
        }

        public void Spawn(Vector3 origin, Vector3 target)
        {
            GameObject go = GoPoolingManager.Main.Get(prefab);
            AttackEffect attackEffect = go.GetComponent<AttackEffect>();
            attackEffect.Initialize(origin, target, Finished);
        }

        private void Finished(GameObject go)
        {
            GoPoolingManager.Main.Return(go, prefab);
        }
    }
}
