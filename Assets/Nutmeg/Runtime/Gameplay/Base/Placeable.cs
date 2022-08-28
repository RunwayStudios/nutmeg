using Gameplay.Level.LevelGenerator;
using UnityEngine;
using UnityEngine.AI;

namespace Nutmeg.Runtime.Gameplay.BaseBuilding
{
    public class Placeable : MonoBehaviour
    {
        [SerializeField] private Transform boundsCenter;
        [SerializeField] private Vector3 boundsHalfExtends = Vector3.one;

        [Space]
        [SerializeField] private float health = 100f;
        
        private bool beingPlaced;
        private bool curPositionValid;

        private Material prevMaterial;

        private bool intersectingOtherPlaceable = true;
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

        public virtual void Damage(float value, DamageType type)
        {
            health -= value;

            if (health <= 0f && gameObject && gameObject.activeSelf)
            {
                DestroyImmediate(gameObject);
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

        public void StartPlacing()
        {
            beingPlaced = true;

            curPositionValid = true;

            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            NavMeshObstacle navMeshObstacle = GetComponent<NavMeshObstacle>();
            if (navMeshObstacle)
                navMeshObstacle.enabled = false;


            Material material = GetComponent<MeshRenderer>().material;
            prevMaterial = material;

            Material newMaterial = BaseManager.Main.transparentDefaultMaterial;
            newMaterial.mainTexture = material.mainTexture;
            newMaterial.color = BaseManager.Main.validMaterialColor;


            MeshRenderer rendererTest = GetComponent<MeshRenderer>();
            if (rendererTest)
                rendererTest.material = newMaterial;
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = rendererTest.material;
            }
        }

        public void StopPlacing()
        {
            beingPlaced = false;

            Destroy(gameObject.GetComponent<Rigidbody>());
            // todo reset to original position if there was one / delete otherwise

            Collider[] colliders = GetComponents<Collider>();

            //todo safe which controllers were activated?
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }

            GetComponent<MeshRenderer>().material = prevMaterial;
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = prevMaterial;
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
                    GetComponent<MeshRenderer>().material.color = BaseManager.Main.invalidMaterialColor;

                if (newValue)
                    GetComponent<MeshRenderer>().material.color = BaseManager.Main.validMaterialColor;
            }

            curPositionValid = newValue;
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
            foreach (Collider collider1 in colliders)
            {
                Placeable placeable = collider1.GetComponent<Placeable>();
                if (placeable != null && placeable != this)
                {
                    intersectingOtherPlaceable = true;
                    UpdateCurrentPositionValid();
                    return;
                }
            }
            
            intersectingOtherPlaceable = false;
            UpdateCurrentPositionValid();
        }
        
        /*private void OnTriggerEnter(Collider other)
        {
            if (beingPlaced && other.CompareTag("Placeable"))
            {
                intersectingOtherPlaceable++;

                if (intersectingOtherPlaceable < 2)
                    UpdateCurrentPositionValid();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (beingPlaced && other.CompareTag("Placeable"))
            {
                intersectingOtherPlaceable--;

                if (intersectingOtherPlaceable < 1)
                    UpdateCurrentPositionValid();
            }
        }*/
        
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(boundsCenter.position, boundsCenter.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, boundsHalfExtends * 2);
        }
    }
}