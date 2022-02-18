using UnityEngine;

namespace Gameplay.Items.Throwable
{
    public class FragGranade : Throwable
    {
        [SerializeField] private GameObject impactFX;
        
        private void Damage()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, data.stats.range);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("DamageReceiver") &&
                    collider.TryGetComponent(typeof(DamageReceiver), out Component c))
                    c.GetComponent<DamageReceiver>().ReceiveDamage(data.stats.damage);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, data.stats.range);
        }

        protected override void OnImpact()
        {
            Instantiate(impactFX, transform.position, Quaternion.Euler(Vector3.left * 90));
            Damage();
            Destroy(gameObject);
        }
    }
}