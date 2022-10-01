using Gameplay.Level.LevelGenerator;
using Nutmeg.Runtime.Gameplay.Combat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Base
{
    public class Placeable : NetworkBehaviour
    {
        [SerializeField] public int price = 0;
        [Space] [SerializeField] private Transform boundsCenter;
        [SerializeField] private Vector3 boundsHalfExtends = Vector3.one;

        [SerializeField] private UnityEvent OnStartPlacing;
        [SerializeField] private UnityEvent OnStopPlacing;

        private bool beingPlaced;
        private bool curPositionValid;
        private bool purchasable;

        private Material prevMaterial;
        private Material placingMaterial;

        private bool intersectingOtherPlaceable = true;
        private bool baseBoundsValid = false;


        public void StartPlacing()
        {
            beingPlaced = true;

            curPositionValid = true;

            OnStartPlacing.Invoke();


            prevMaterial = GetComponent<MeshRenderer>().material;

            placingMaterial = BaseManager.Main.transparentDefaultMaterial;
            placingMaterial.mainTexture = prevMaterial.mainTexture;
            SetMaterialColor(BaseManager.Main.validMaterialColor);

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

        private void SetMaterialColor(Color color)
        {
            if (beingPlaced)
                placingMaterial.color = color;
        }

        public bool IsCurrentPositionValid()
        {
            return curPositionValid;
        }

        public bool CanPurchase()
        {
            return purchasable;
        }

        private void UpdateCurrentPositionValid()
        {
            bool newValue = !intersectingOtherPlaceable && baseBoundsValid;

            if (newValue && curPositionValid)
                return;

            curPositionValid = newValue;

            if (!newValue)
                SetMaterialColor(BaseManager.Main.invalidMaterialColor);

            if (newValue)
                SetMaterialColor(purchasable ? BaseManager.Main.validMaterialColor : BaseManager.Main.noMoneyMaterialColor);
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

        public void UpdatePurchasable()
        {
            bool newPurchasable = MoneyManager.Main.CanAfford(price);
            if (purchasable && newPurchasable)
                return;

            purchasable = newPurchasable;

            if (!curPositionValid)
                return;

            SetMaterialColor(purchasable ? BaseManager.Main.validMaterialColor : BaseManager.Main.noMoneyMaterialColor);
        }

        public void Deactivate()
        {
            enabled = false;
            CombatEntity entity = GetComponent<CombatEntity>();
            entity.enabled = false;
        }

        public void OnDeath()
        {
            DestroyImmediate(gameObject);
            LevelGenerator.Main.UpdateNavMesh();
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(boundsCenter.position, boundsCenter.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, boundsHalfExtends * 2);
        }
    }
}