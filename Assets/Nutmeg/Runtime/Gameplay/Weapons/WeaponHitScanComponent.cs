using System.Collections.Generic;
using System.Linq;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Weapons.Misc;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponHitScanComponent : WeaponDamageComponent
    {
        protected override void Start()
        {
            base.Start();
            root.poolComponent.releaseAction = obj => obj.SetActive(false);
        }

        public override bool Get(out object data)
        {
            bool success = HitScan(out var r);
            data =  new[] { r };
            return success;
        }
        
        protected virtual bool HitScan(out DamageableModule hit)
        {
            Debug.DrawRay(Origin, OffsetDirection * root.stats.range, Color.red,
                1f / (root.stats.fireRate / 60f));

            if (Physics.Raycast(Origin, OffsetDirection, out RaycastHit h, root.stats.range))
            {
                SpawnBullet(Origin, h.point);

                if (h.transform.TryGetComponent(out DamageableModule m))
                {
                    hit = m;
                    return true;
                }
            }
            else
            {
                SpawnBullet(Origin, Origin + OffsetDirection * root.stats.range);
            }

            hit = null;
            return false;
        }

        private void SpawnBullet(Vector3 origin, Vector3 target)
        {
            root.poolComponent.Get(out var b);
            Projectile projectile = ((GameObject) b).GetComponent<Projectile>();
            projectile.Initialize(origin, target);

            if (root.playerNetworkObject.IsLocalPlayer)
                SpawnBulletServerRpc(origin, target);
        }

        [ServerRpc]
        private void SpawnBulletServerRpc(Vector3 origin, Vector3 target, ServerRpcParams serverRpcParams = default)
        {
            List<ulong> ids = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            ids.Remove(serverRpcParams.Receive.SenderClientId);

            SpawnBulletClientRpc(origin, target,
                new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = ids}});
        }

        [ClientRpc]
        private void SpawnBulletClientRpc(Vector3 origin, Vector3 target, ClientRpcParams clientRpcParams)
        {
            SpawnBullet(origin, target);
        }
    }
}