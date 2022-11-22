using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class DamageableModule : CombatModule
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private UnityEvent<DamageInfo> OnReceiveDamage;
        [SerializeField] private UnityEvent<DamageInfo> OnDeath;
        private bool dead = false;
        
        
        

        public virtual void Damage(float value, DamageType type, Vector3 sourcePos, Vector3 hitPos)
        {
            DamageServerRpc(new DamageInfo(value, type, sourcePos, hitPos));
        }
        
        public virtual void Damage(float value, DamageType type, Vector3 sourcePos)
        {
            DamageServerRpc(new DamageInfo(value, type, sourcePos));
        }
        
        public virtual void Damage(float value, DamageType type)
        {
            DamageServerRpc(new DamageInfo(value, type));
        }

        public void Damage(float value)
        {
            DamageServerRpc(new DamageInfo(value));
        }
        

        [ServerRpc(RequireOwnership = false)]
        private void DamageServerRpc(DamageInfo info)
        {
            if (dead)
                return;

            health -= info.Value;

            dead = health <= 0f;
            DamageClientRpc(dead, info);
        }

        [ClientRpc]
        private void DamageClientRpc(bool death, DamageInfo info)
        {
            OnReceiveDamage.Invoke(info);

            if (death)
            {
                dead = true;
                Entity.OnDeath();
                OnDeath.Invoke(info);
            }
        }
    }

    [Serializable]
    public struct DamageInfo : INetworkSerializeByMemcpy
    {
        public float Value { get; }
        public DamageType Type { get; }
        public bool SourcePosSpecified { get; }
        public Vector3 SourcePos { get; }
        public bool HitPosSpecified { get; }
        public Vector3 HitPos { get; }
        
        public DamageInfo(float value) : this()
        {
            Value = value;
            Type = DamageType.Default;
        }
        
        public DamageInfo(float value, DamageType type) : this()
        {
            Value = value;
            Type = type;
        }
        
        public DamageInfo(Vector3 sourcePos, Vector3 hitPos) : this()
        {
            SourcePosSpecified = true;
            SourcePos = sourcePos;
            HitPosSpecified = true;
            HitPos = hitPos;
        }
        
        public DamageInfo(float value, DamageType type, Vector3 sourcePos) : this()
        {
            Value = value;
            Type = type;
            SourcePosSpecified = true;
            SourcePos = sourcePos;
        }
        
        public DamageInfo(float value, DamageType type, Vector3 sourcePos, Vector3 hitPos) : this()
        {
            Value = value;
            Type = type;
            SourcePosSpecified = true;
            SourcePos = sourcePos;
            HitPosSpecified = true;
            HitPos = hitPos;
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