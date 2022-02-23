using Nutmeg.Runtime.Gameplay.Items;
using Nutmeg.Runtime.Gameplay.Targets;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons.Throwable
{
    public class FragGrenade : global::Nutmeg.Runtime.Gameplay.Weapons.Throwable.Throwable
    {
        [SerializeField] private GameObject impactFX;

        private void Damage()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, Stats[StatType.Range]);

            foreach (Collider cl in colliders)
            {
                if (cl.CompareTag("DamageReceiver") && cl.TryGetComponent(typeof(DamageReceiver), out Component co))
                    co.GetComponent<DamageReceiver>().ReceiveDamage(Stats[StatType.Damage]);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Stats[StatType.Range]);
        }

        protected override void OnImpact()
        {
            Instantiate(impactFX, transform.position, Quaternion.Euler(Vector3.left * 90));
            Damage();
            Destroy(gameObject);
        }
    }
}