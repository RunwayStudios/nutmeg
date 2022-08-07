using System;
using System.Collections.Generic;
using Nutmeg.Runtime.Utility.InputSystem;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.BaseBuilding
{
    public class BaseManager : MonoBehaviour
    {
        public static BaseManager Main;

        private InputActions input;

        [SerializeField] [Tooltip("")] private Texture2D baseFlatteningMap;

        [SerializeField] [Tooltip("")] private GameObject debugToPlace;

        [Space]
        [SerializeField] [Tooltip("")] private float baseY = 0.0f;
        
        private bool placingObject = false;
        private bool inBuildingMode = false;
        private GameObject curPlacingGo;
        private GameObject curPlacingOriginalGo;
        private Placeable curPlacingPlaceable;
        private Vector3 previousPosition = Vector3.zero;

        private List<Placeable> placed = new List<Placeable>();

        public Material transparentDefaultMaterial;
        public Color invalidMaterialColor;
        public Color validMaterialColor;

        // Start is called before the first frame update
        void Start()
        {
            Main = this;
            input = InputManager.input;
            input.BaseBuilding.PlaceObject.performed += PlacePlaceable;
            input.Player.Tab.performed += DebugEnterBuildingMode;
        }

        private void OnDestroy()
        {
            input.BaseBuilding.PlaceObject.performed -= PlacePlaceable;
            input.Player.Tab.performed -= DebugEnterBuildingMode;
        }

        // Update is called once per frame
        void Update()
        {
            if (placingObject)
            {
                UpdatePositionOfObjectBeingPlaced();
            }
        }



        private void DebugEnterBuildingMode(InputAction.CallbackContext context)
        {
            EnterBuildingMode();
            StartPlacingObject(debugToPlace);
        }

        private void EnterBuildingMode()
        {
            if (inBuildingMode)
                return;
            
            // todo hendl
            input.Player.Primary.Disable();
            input.BaseBuilding.Enable();

            inBuildingMode = true;
        }

        private void StartPlacingObject(GameObject blueprint)
        {
            if (placingObject)
                return;

            Vector3 placeablePos;
            if (!TryGetNewPlacementPosition(out placeablePos))
                return;
            
            if (!inBuildingMode)
                EnterBuildingMode();
            
            curPlacingOriginalGo = blueprint;
            curPlacingGo = Instantiate(blueprint, placeablePos, Quaternion.Euler(Vector3.zero));

            if (!curPlacingGo.GetComponent<Collider>())
            {
                Debug.LogError("Placeable objects require colliders!");
                return;
            }

            curPlacingPlaceable = curPlacingGo.GetComponent<Placeable>();
            curPlacingPlaceable.SetBeingPlaced(true);
            
            UpdatePositionOfObjectBeingPlaced();
            
            placingObject = true;
        }

        private void ExitBuildingMode()
        {
            if (!inBuildingMode)
                return;

            if (placingObject)
                CancelPlacingPlaceable();

            input.Player.Primary.Enable();
            input.BaseBuilding.Disable();

            inBuildingMode = false;
        }

        private void PlacePlaceable(InputAction.CallbackContext context)
        {
            if (!placingObject || !curPlacingPlaceable.IsCurrentPositionValid())
                return;

            placed.Add(Instantiate(curPlacingOriginalGo, curPlacingGo.transform.position, Quaternion.Euler(Vector3.zero)).GetComponent<Placeable>());

            // todo necessary when moving objects
            // curPlacingPlaceable.SetBeingPlaced(false);

            Destroy(curPlacingGo);
            placingObject = false;

            ExitBuildingMode();
        }

        private void CancelPlacingPlaceable()
        {
            if (!placingObject)
                return;

            Destroy(curPlacingGo);
            placingObject = false;
        }

        private void UpdatePositionOfObjectBeingPlaced()
        {
            Vector3 newPos;
            if (TryGetNewPlacementPosition(out newPos))
            {
                if (previousPosition == newPos)
                    return;

                previousPosition = newPos;
                
                curPlacingGo.transform.position = newPos;

                curPlacingPlaceable.CheckBaseBounds(baseFlatteningMap);
            }
        }

        private bool TryGetNewPlacementPosition(out Vector3 pos)
        {
            RaycastHit? hit = MouseController.ShootRayFromCameraToMouse((1 << 11));
            if (hit.HasValue)
            {
                Vector3 posOut = hit.Value.point;
                posOut.y = baseY;
                pos = posOut;
                return true;
            }
            pos = Vector3.zero;
            return false;
        }
    }
}