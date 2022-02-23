﻿using System.Collections;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons.HitScan
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        private Vector3 origin;
        private Vector3 target;

        private float elapsedTime;
        private TrailRenderer renderer;

        private void Update()
        {
            transform.position = Vector3.Lerp(origin, target,
                elapsedTime += Time.deltaTime / (Vector3.Distance(origin, target) / lifeTime));

            if (transform.position == target)
                StartCoroutine(DestroySelf());
        }

        private float destroyTimer;

        private IEnumerator DestroySelf()
        {
            while ((destroyTimer += Time.deltaTime) <= 1f)
                yield return null;

            Destroy(gameObject);
        }

        public void Initialize(Vector3 target)
        {
            origin = transform.position;
            this.target = target;
            renderer = GetComponent<TrailRenderer>();
        }
    }
}