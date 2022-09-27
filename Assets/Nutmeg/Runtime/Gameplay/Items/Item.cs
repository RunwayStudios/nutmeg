using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] [CanBeNull] private Rigidbody rb;
        [SerializeField] private float timeBeforeFreeze;
        [SerializeField] private float maxMagnitudeForFreeze;

        private void Start()
        {
            rb ??= GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            FreezeItem();
        }

        public virtual void Use()
        {
            
        }

        private void Initialize()
        {
            
        } 

        private float timer;
        private void FreezeItem()
        {
            if(gameObject.isStatic)
                return;

            if (rb.velocity.magnitude > maxMagnitudeForFreeze)
            {
                timer = 0f;
                return;
            }
            
            if ((timer += Time.deltaTime) >= timeBeforeFreeze)
            {
                gameObject.isStatic = true;
                rb.isKinematic = true;
            }
        }
    }
}