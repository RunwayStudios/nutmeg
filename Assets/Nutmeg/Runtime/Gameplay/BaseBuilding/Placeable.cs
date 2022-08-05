using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.BaseBuilding
{
    public class Placeable : MonoBehaviour
    {
        [SerializeField] [Tooltip("")] private bool beingPlaced;

        [SerializeField] [Tooltip("")] private bool curPositionValid;

        private Material prevMaterial;

        private int curCollidingCount = 0;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void SetBeingPlaced(bool newBeingPlaced)
        {
            beingPlaced = newBeingPlaced;
            if (newBeingPlaced && !(gameObject.GetComponent<Rigidbody>()))
            {
                curPositionValid = true;

                Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;

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

        public bool CurrentPositionValid()
        {
            return curPositionValid;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (beingPlaced && other.CompareTag("Placeable"))
            {
                if (curCollidingCount < 1)
                {
                    curPositionValid = false;
                    GetComponent<MeshRenderer>().material.color = BaseManager.Main.invalidMaterialColor;
                }

                curCollidingCount++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (beingPlaced && other.CompareTag("Placeable"))
            {
                curCollidingCount--;

                if (curCollidingCount < 1)
                {
                    curPositionValid = true;
                    GetComponent<MeshRenderer>().material.color = BaseManager.Main.validMaterialColor;
                }
            }
        }
    }
}