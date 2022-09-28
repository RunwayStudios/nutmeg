using Nutmeg.Runtime.Gameplay.Combat;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.BaseBuilding
{
    public class Placeable : MonoBehaviour
    {
        [SerializeField] private Transform boundsCenter;
        [SerializeField] private Vector3 boundsHalfExtends = Vector3.one;

        [SerializeField] private UnityEvent OnStartPlacing;
        [SerializeField] private UnityEvent OnStopPlacing;
        
        private bool beingPlaced;
        private bool curPositionValid;

        private Material prevMaterial;
        private Material placingMaterial;

        private bool intersectingOtherPlaceable = true;
        private bool baseBoundsValid = true;


        public void StartPlacing()
        {
            beingPlaced = true;

            curPositionValid = true;
            
            OnStartPlacing.Invoke();


            prevMaterial = GetComponent<MeshRenderer>().material;

            placingMaterial = BaseManager.Main.transparentDefaultMaterial;
            placingMaterial.mainTexture = prevMaterial.mainTexture;
            placingMaterial.color = BaseManager.Main.validMaterialColor;

            SetCollidersEnabled(false);
            SetMaterial(placingMaterial);
        }

        public void StopPlacing()
        {
            beingPlaced = false;
            
            OnStopPlacing.Invoke();

            // todo reset to original position if there was one / delete otherwise

            SetCollidersEnabled(true);
            SetMaterial(prevMaterial);
            prevMaterial = null;
            placingMaterial = null;
        }

        private void SetCollidersEnabled(bool enable)
        {
            //todo maybe safe which colliders were activated before
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = enable;
            }
        }
        
        private void SetMaterial(Material newMaterial)
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = newMaterial;
            }
        }

        public bool IsCurrentPositionValid()
        {
            return curPositionValid;
        }

        private void UpdateCurrentPositionValid()
        {
            bool newValue = !intersectingOtherPlaceable && baseBoundsValid;

            if (newValue != curPositionValid)
            {
                if (!newValue)
                    placingMaterial.color = BaseManager.Main.invalidMaterialColor;

                if (newValue)
                    placingMaterial.color = BaseManager.Main.validMaterialColor;
                
                curPositionValid = newValue;
            }
        }

        public void CheckBaseBounds(Texture2D baseMap)
        {
            Bounds bounds = new Bounds(boundsCenter.position, boundsHalfExtends * 2);
            int textureX = Mathf.FloorToInt(bounds.min.x) + baseMap.width / 2;
            int textureY = Mathf.FloorToInt(bounds.min.z) + baseMap.width / 2;
            float color = baseMap.GetPixel(textureX, textureY).r;

            if (color != 0)
            {
                baseBoundsValid = false;
                UpdateCurrentPositionValid();
                return;
            }

            textureY = Mathf.FloorToInt(bounds.max.z) + baseMap.width / 2;
            color = baseMap.GetPixel(textureX, textureY).r;
            if (color != 0)
            {
                baseBoundsValid = false;
                UpdateCurrentPositionValid();
                return;
            }

            textureX = Mathf.FloorToInt(bounds.max.x) + baseMap.width / 2;
            color = baseMap.GetPixel(textureX, textureY).r;
            if (color != 0)
            {
                baseBoundsValid = false;
                UpdateCurrentPositionValid();
                return;
            }

            textureY = Mathf.FloorToInt(bounds.min.z) + baseMap.width / 2;
            color = baseMap.GetPixel(textureX, textureY).r;
            if (color != 0)
            {
                baseBoundsValid = false;
                UpdateCurrentPositionValid();
                return;
            }

            baseBoundsValid = true;
            UpdateCurrentPositionValid();
        }

        public void CheckIntersecting()
        {
            Collider[] colliders = Physics.OverlapBox(boundsCenter.position, boundsHalfExtends, boundsCenter.rotation);
            foreach (Collider c in colliders)
            {
                CombatEntity entity = c.GetComponent<CombatEntity>();
                if (entity != null && entity != GetComponent<CombatEntity>())
                {
                    intersectingOtherPlaceable = true;
                    UpdateCurrentPositionValid();
                    return;
                }
            }
            
            intersectingOtherPlaceable = false;
            UpdateCurrentPositionValid();
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(boundsCenter.position, boundsCenter.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, boundsHalfExtends * 2);
        }
    }
}