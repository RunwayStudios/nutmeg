using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class DamageableModule : CombatModule
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private UnityEvent<Vector3, DamageType> OnReceiveDamage;
        [SerializeField] private UnityEvent OnDeath;
        private bool dead = false;
        

        public virtual void Damage(float value, Vector3 hitPos, DamageType type = DamageType.Default)
        {
            DamageServerRpc(value, hitPos, type);
        }
        
        public virtual void Damage(float value, DamageType type)
        {
            DamageServerRpc(value, Entity.transform.position, type);
        }

        public void Damage(float value)
        {
            Damage(value, DamageType.Default);
        }
        

        [ServerRpc(RequireOwnership = false)]
        private void DamageServerRpc(float value, Vector3 hitPos, DamageType type)
        {
            if (dead)
                return;

            health -= value;

            dead = health <= 0f;
            DamageClientRpc(dead, hitPos, type);
        }

        [ClientRpc]
        private void DamageClientRpc(bool death, Vector3 hitPos, DamageType type)
        {
            OnReceiveDamage.Invoke(hitPos, type);

            if (death)
            {
                Entity.OnDeath();
                OnDeath.Invoke();
            }
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