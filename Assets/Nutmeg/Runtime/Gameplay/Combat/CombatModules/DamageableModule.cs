using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class DamageableModule : CombatModule
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private UnityEvent OnReceiveDamage;
        [SerializeField] private UnityEvent OnDeath;
        private bool dead = false;


        public virtual void Damage(float value, DamageType type)
        {
                DamageServerRpc(value, type);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DamageServerRpc(float value, DamageType type)
        {
            if (dead)
                return;

            health -= value;

            DamageClientRpc();

            if (health <= 0f)
            {
                dead = true;
                DeathClientRpc();
            }
        }

        [ClientRpc]
        private void DamageClientRpc()
        {
            OnReceiveDamage.Invoke();
        }

        [ClientRpc]
        private void DeathClientRpc()
        {
            Entity.OnDeath();
            OnDeath.Invoke();
        }
    }

    public enum DamageType
    {
        Default,
        Fire,
        Explosion,
        Water
    }
}