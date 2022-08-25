using System;
using Gameplay.Level.LevelGenerator;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.BaseBuilding
{
    public class Placeable : MonoBehaviour
    {
        [SerializeField] [Tooltip("")] private bool beingPlaced;

        [SerializeField] [Tooltip("")] private bool curPositionValid;

        [SerializeField] private float health = 100f;

        private Material prevMaterial;

        private int curCollidingCount = 0;
        private bool baseBoundsValid = true;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }


        #region Damage

        public void Damage(float value, DamageType type)
        {
            health -= value;

            if (health <= 0f && gameObject && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                LevelGenerator.Main.UpdateNavMesh();
            }
        }

        public enum DamageType
        {
            Default,
            Fire,
            Explosion,
            Water
        }

        #endregion
        

        #region Placing

        public void SetBeingPlaced(bool newBeingPlaced)
        {
            beingPlaced = newBeingPlaced;
            if (newBeingPlaced && !(gameObject.GetComponent<Rigidbody>()))
            {
                curPositionValid = true;

                Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                rigidbody.isKinematic = true;

                Collider[] colliders = GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.isTrigger = true;
                }


                Material material = GetComponent<MeshRenderer>().material;
                prevMaterial = material;

                Material newMaterial = BaseManager.Main.transparentDefaultMaterial;
                newMaterial.mainTexture = material.mainTexture;
                newMaterial.color = BaseManager.Main.validMaterialColor;


                MeshRenderer rendererTest = GetComponent<MeshRenderer>();
                rendererTest.material = newMaterial;
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    meshRenderer.material = rendererTest.material;
                }
            }
            else if (!newBeingPlaced && gameObject.GetComponent<Rigidbody>())
            {
                Destroy(gameObject.GetComponent<Rigidbody>());
                // todo reset to original position if there was one / delete otherwise

                Collider[] colliders = GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.isTrigger = false;
                }

                GetComponent<MeshRenderer>().material = prevMaterial;
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    meshRenderer.material = prevMaterial;
                }
            }
        }

        public bool IsCurrentPositionValid()
        {
            return curPositionValid;
        }

        public void UpdateCurrentPositionValid()
        {
            bool newValue = curCollidingCount < 1 && baseBoundsValid;

            if (newValue != curPositionValid)
            {
                if (!newValue)
                    GetComponent<MeshRenderer>().material.color = BaseManager.Main.invalidMaterialColor;

                if (newValue)
                    GetComponent<MeshRenderer>().material.color = BaseManager.Main.validMaterialColor;
            }

            curPositionValid = newValue;
        }

        public void CheckBaseBounds(Texture2D baseMap)
        {
            Bounds bounds = GetComponent<Collider>().bounds;
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

        private void OnTriggerEnter(Collider other)
        {
            if (beingPlaced && other.CompareTag("Placeable"))
            {
                curCollidingCount++;

                if (curCollidingCount < 2)
                    UpdateCurrentPositionValid();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (beingPlaced && other.CompareTag("Placeable"))
            {
                curCollidingCount--;

                if (curCollidingCount < 1)
                    UpdateCurrentPositionValid();
            }
        }
        
        #endregion
    }
}