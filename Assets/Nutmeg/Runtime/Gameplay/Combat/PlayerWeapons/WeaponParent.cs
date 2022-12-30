using JetBrains.Annotations;
using Nutmeg.Runtime.Gameplay.Combat.PlayerWeapons;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons
{
    public class WeaponParent : NetworkBehaviour
    {
        [SerializeField] private Weapon weapon;
        private Transform weaponTransform;
        private bool weaponSelected;


        private void Start()
        {
            SetWeapon(weapon);
        }

        private void Update()
        {
            if (weaponSelected)
            {
                weaponTransform.position = transform.position;
                weaponTransform.rotation = transform.rotation;
            }
        }


        public void SetWeapon([CanBeNull] Weapon newWeapon)
        {
            SetWeaponClientRpc(newWeapon ? new NetworkBehaviourReference(newWeapon) : new NetworkBehaviourReference());
        }

        [ClientRpc]
        private void SetWeaponClientRpc(NetworkBehaviourReference newWeaponReference)
        {   
            if (!newWeaponReference.TryGet(out Weapon newWeapon))
            {
                weapon = null;
                weaponTransform = null;
                weaponSelected = false;
                return;
            }
            
            weapon = newWeapon;
            weaponTransform = weapon.transform;
            weaponSelected = true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetWeapon(weapon);
        }
#endif
    }
}