using System.Collections.Generic;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.BaseBuilding
{
    public class BaseManager : MonoBehaviour
    {
        public static BaseManager Main;
        
        [SerializeField] [Tooltip("")] private Texture2D baseFlatteningMap;

        [SerializeField] [Tooltip("")] private bool startPlacingObject = false;
        [SerializeField] [Tooltip("")] private bool placeObject = false;
        
        
        [SerializeField] [Tooltip("")] private bool placingObject = false;
        [SerializeField] [Tooltip("")] private GameObject toPlace;
        [SerializeField] [Tooltip("")] private GameObject curPlacingGo;
        [SerializeField] [Tooltip("")] private Placeable curPlacingPlaceable;
        [SerializeField] [Tooltip("")] private Vector3 previousPosition = Vector3.zero;

        [SerializeField] [Tooltip("")] private float baseY = 0.0f;

        private List<Placeable> placed = new List<Placeable>();

        public Material transparentDefaultMaterial;
        public Color invalidMaterialColor;
        public Color validMaterialColor;

        // Start is called before the first frame update
        void Start()
        {
            Main = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (startPlacingObject)
            {
                startPlacingObject = false;
                
                curPlacingGo = Instantiate(toPlace, new Vector3(0f, -100f, 0f), Quaternion.Euler(Vector3.zero));
                Collider collider = curPlacingGo.GetComponent<Collider>();
                if (!collider)
                    Debug.LogError("Placeable objects require colliders!");
                
                collider.isTrigger = true;
                curPlacingPlaceable = curPlacingGo.GetComponent<Placeable>();
                curPlacingPlaceable.SetBeingPlaced(true);
                
                

                placingObject = true;
            }

            if (placeObject)
            {
                placeObject = false;
                if (placingObject && curPlacingPlaceable.CurrentPositionValid())
                {
                    placed.Add(Instantiate(toPlace, curPlacingGo.transform.position, Quaternion.Euler(Vector3.zero)).GetComponent<Placeable>());
                        
                    curPlacingPlaceable.SetBeingPlaced(false);
                    Destroy(curPlacingGo);
                    
                    
                    placingObject = false;
                }
            }
            
            if (placingObject)
            {
                RaycastHit? hit = MouseController.ShootRayFromCameraToMouse((1 << 11));
                if (hit != null)
                {
                    Vector3 hitPosition = hit.Value.point;
                    if (previousPosition.x != hitPosition.x ||
                        previousPosition.z != hitPosition.z)
                    {
                        previousPosition.x = hitPosition.x;
                        previousPosition.y = baseY;
                        previousPosition.z = hitPosition.z;

                        int textureX = Mathf.FloorToInt(previousPosition.x) + baseFlatteningMap.width / 2;
                        int textureY = Mathf.FloorToInt(previousPosition.z) + baseFlatteningMap.height / 2;
                        
                        float color = baseFlatteningMap.GetPixel(textureX, textureY).r;
                        if (color == 0)
                        {
                            curPlacingGo.transform.position = previousPosition;

                        }
                    }
                }
            }
        }
    }
}