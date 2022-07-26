using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nutmeg.Runtime.Utility.WorldSpaceUI
{
    public class HitNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float minPositionVariance;
        [SerializeField] private float maxPositionVariance;
        public float Number { get; private set; }

        private void Update()
        {
            transform.LookAt(MouseController.MouseController.camera.transform.position);
        }

        public void SetNumberAndPosition(float value, Vector3 hitPosition)
        {
            transform.position = new Vector3(hitPosition.x + Random.Range(minPositionVariance, maxPositionVariance),
                hitPosition.y + Random.Range(minPositionVariance, maxPositionVariance),
                hitPosition.z + Random.Range(minPositionVariance, maxPositionVariance));
            Number = value;
            text.SetText(value.ToString());
        }
    }
}