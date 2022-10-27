using System.Collections.Generic;
using System.Linq;
using Nutmeg.Runtime.Gameplay.Weapons.Misc;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class WeaponProjectileComponent : WeaponDamageComponent
    {
        protected override void Start()
        {
            base.Start();

            root.poolComponent.releaseAction = obj => obj.GetComponent<Weapon>().Use();
        }
        
        public override bool Get(out object data)
        {
            data = default;
            
            SpawnProjectile(Origin, Origin + OffsetDirection * root.stats.range);
            return false;
        }
        
       private void SpawnProjectile(Vector3 origin, Vector3 target)
        {
            root.poolComponent.Get(out var b);
            Projectile projectile = ((GameObject) b).GetComponent<Projectile>();
            projectile.Initialize(origin, target);
            
            if (root.playerNetworkObject.IsLocalPlayer)
                SpawnProjectileServerRpc(origin, target);
        }

        [ServerRpc]
        private void SpawnProjectileServerRpc(Vector3 origin, Vector3 target, ServerRpcParams serverRpcParams = default)
        {
            List<ulong> ids = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            ids.Remove(serverRpcParams.Receive.SenderClientId);

            SpawnProjectileClientRpc(origin, target,
                new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = ids}});
        }

        [ClientRpc]
        private void SpawnProjectileClientRpc(Vector3 origin, Vector3 target, ClientRpcParams clientRpcParams)
        {
            SpawnProjectile(origin, target);
        }
    }
}