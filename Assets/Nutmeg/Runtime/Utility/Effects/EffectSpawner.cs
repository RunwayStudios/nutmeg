using System.Linq;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Utility.GameObjectPooling;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Utility.Effects
{
    public class EffectSpawner : MonoBehaviour
    {
        [SerializeField] protected GameObject effectPrefab;
        [SerializeField] protected Transform effectPosition;
        [SerializeField] protected Transform effectParent;
        [SerializeField] protected bool stickToParent;
        [Space] [Header("Using TrySpawnEffect()")]
        [SerializeField] protected DamageType[] validTypes;
        [SerializeField] protected float damageCap;
        [SerializeField] protected UnityEvent OnFinished;
        

        public virtual void SpawnEffect()
        {
            if (stickToParent)
            {
                GameObject go = GoPoolingManager.Main.Get(effectPrefab);
                go.transform.position = effectPosition.position;
                go.transform.rotation = effectPosition.rotation;
                go.transform.SetParent(effectParent);
                go.GetComponent<Effect>().Initialize(Finished);
            }
            else
            {
                GameObject go = GoPoolingManager.Main.Get(effectPrefab);
                go.transform.position = effectPosition.position;
                go.transform.rotation = effectPosition.rotation;
                go.GetComponent<Effect>().Initialize(Finished);
            }
        }

        public virtual void SpawnEffect(DamageInfo info)
        {
            GameObject go = GoPoolingManager.Main.Get(effectPrefab);
            go.transform.position = info.HitPosSpecified ? info.HitPos : transform.position;
            go.transform.rotation = effectPosition ? effectPosition.rotation : transform.rotation;
            go.GetComponent<Effect>().Initialize(info, Finished);
        }

        public virtual void TrySpawnEffect(DamageInfo info)
        {
            if (damageCap > info.Value)
                return;

            if (!validTypes.Contains(info.Type))
                return;

            SpawnEffect(info);
        }

        protected virtual void Finished(GameObject go)
        {
            GoPoolingManager.Main.Return(go, effectPrefab);
            OnFinished.Invoke();
        }


        // private void OnValidate()
        // {
        //     if (testEffect)
        //     {
        //         testEffect = false;
        //         SpawnEffect();
        //     }
        // }
    }
}