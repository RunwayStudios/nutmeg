using Nutmeg.Runtime.Utility.MouseController;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nutmeg.Runtime.Core.WorldSpaceUIManager
{
    public class HitNumber : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private TMP_Text text;
        [SerializeField] private float minPositionVariance;
        [SerializeField] private float maxPositionVariance;

        private float internalLifeTime;
    
        private void OnEnable()
        {
            internalLifeTime = lifeTime;
        }

        private void Update()
        {
            transform.LookAt(MouseController.camera.transform.position);
        
            internalLifeTime -= Time.deltaTime;
            if(internalLifeTime <= 0f)
                Destroy(gameObject);
        }

        public void ResetLifeTime() => internalLifeTime = lifeTime;
    
        public void SetNumberAndPosition(float value, Vector3 hitPosition)
        {
            transform.position = new Vector3(hitPosition.x + Random.Range(minPositionVariance, maxPositionVariance),
                hitPosition.y + Random.Range(minPositionVariance, maxPositionVariance),
                hitPosition.z + Random.Range(minPositionVariance, maxPositionVariance));
        
            text.SetText(value.ToString());
        }
    }
}