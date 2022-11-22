using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects.EffectTypes
{
    public class BodySplatterEffectSpawner : EffectSpawner
    {
        public override void SpawnEffect(DamageInfo info)
        {
            // MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>(false);
            // Debug.Log(meshRenderers.Length);
            // for (int i = 0; i < meshRenderers.Length; i++)
            // {
            //     meshRenderers[i].enabled = false;
            // }

            GetComponentInChildren<SkinnedMeshRenderer>(false).enabled = false;
            
            base.SpawnEffect(info);
        }
    }
}
