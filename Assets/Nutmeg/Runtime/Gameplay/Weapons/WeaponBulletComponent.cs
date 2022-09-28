using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class WeaponBulletComponent : WeaponComponent
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int maxPoolSize;
        [SerializeField] private int defaultPoolSize;

        private ObjectPool<GameObject> pool;

        protected override void Start()
        {
            base.Start();

            pool = new ObjectPool<GameObject>(CreateBullet, GetBullet, ReleaseBullet, DestroyBullet, false,
                defaultPoolSize, maxPoolSize);
        }

        private GameObject CreateBullet()
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullet.GetComponent<Bullet>().SetReleaseAction(pool.Release);
            return bullet;
        }

        private void GetBullet(GameObject bullet) => bullet.SetActive(true);

        private void ReleaseBullet(GameObject bullet) => bullet.SetActive(false);

        private void DestroyBullet(GameObject bullet) => Destroy(bullet);


        public override bool Get(out object data)
        {
            data = pool.Get().GetComponent<Bullet>();
            
            return true;
        }
    }
}