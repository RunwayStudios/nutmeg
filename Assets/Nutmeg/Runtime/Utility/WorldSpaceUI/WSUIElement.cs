using System;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.WorldSpaceUI
{
    // ReSharper disable once InconsistentNaming, IdentifierTypo
    public class WSUIElement
    {
        public int Id { get; internal set; }
        public float ElapsedTime { get; private set; }
        public float LifeTime { get; internal set; }

        public bool IsPaused { get; internal set; }
        
        public GameObject GameObject { get; internal set; }

        public Action onDestroy;
        
        public Action onComplete;
        
        internal bool destroyOnComplete;
        
        public WSUIElement DestroyOnComplete()
        {
            destroyOnComplete = true;
            return this;
        }

        public void Reset()
        {
            ElapsedTime = 0f;
        }

        internal void DestroySelf()
        {
            onDestroy?.Invoke();
            UnityEngine.Object.Destroy(GameObject);
        }
        
        internal void UpdateInternal(float time)
        {
            ElapsedTime += time;

            if (ElapsedTime >= LifeTime)
            {
                onComplete?.Invoke();
            }
        }
    }
}