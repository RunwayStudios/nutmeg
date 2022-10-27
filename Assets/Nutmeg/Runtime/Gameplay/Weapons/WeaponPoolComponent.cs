using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class WeaponPoolComponent : WeaponComponent
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int maxPoolSize;
        [SerializeField] private int defaultPoolSize;

        private ObjectPool<GameObject> pool;

        public Action<GameObject> releaseAction;

        protected override void Start()
        {
            base.Start();
            pool = new ObjectPool<GameObject>(CreateObject, GetObject, ReleaseObject, DestroyObject, false,
                defaultPoolSize, maxPoolSize);
        }

        private GameObject CreateObject()
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.GetComponent<IWeaponPoolObject>().SetReleaseAction(pool.Release);
            //bulletPrefab.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.LocalClientId, true);
            return obj;
        }

        private void GetObject(GameObject obj)
        {
            //bullet.SetActive(true);
        }

        private void ReleaseObject(GameObject obj) => releaseAction?.Invoke(obj);

        private void DestroyObject(GameObject obj) => Destroy(obj.gameObject);

        public override bool Get(out object data)
        {
            data = pool.Get();
            
            return true;
        }
    }
}