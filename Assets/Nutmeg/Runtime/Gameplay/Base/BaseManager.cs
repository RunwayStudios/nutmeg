using System.Collections.Generic;
using Gameplay.Level.LevelGenerator;
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

        [Space] [SerializeField] [Tooltip("")] private float baseY = 0.0f;

        [SerializeField] private float rotateSpeed = 90f;
        private float currentRotation = 0f;

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


        private void Awake()
        {
            Main = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            input = InputManager.Input;
            input.BaseBuilding.PlaceObject.performed += PlacePlaceable;
            input.BaseBuilding.RotateClockwise.performed += StartRotatingPlaceableClockwise;
            input.BaseBuilding.RotateClockwise.canceled += StopRotatingPlaceableClockwise;
            input.BaseBuilding.RotateCounterclockwise.performed += StartRotatingPlaceableCounterclockwise;
            input.BaseBuilding.RotateCounterclockwise.canceled += StopRotatingPlaceableCounterclockwise;
            input.Player.Tab.performed += SwapBuildingMode;
        }

        private void OnDestroy()
        {
            input.BaseBuilding.PlaceObject.performed -= PlacePlaceable;
            input.BaseBuilding.RotateClockwise.performed -= StartRotatingPlaceableClockwise;
            input.BaseBuilding.RotateClockwise.canceled -= StopRotatingPlaceableClockwise;
            input.BaseBuilding.RotateCounterclockwise.performed -= StartRotatingPlaceableCounterclockwise;
            input.BaseBuilding.RotateCounterclockwise.canceled -= StopRotatingPlaceableCounterclockwise;
            input.Player.Tab.performed -= SwapBuildingMode;
        }

        // Update is called once per frame
        void Update()
        {
            if (placingObject)
            {
                UpdatePositionOfObjectBeingPlaced();
                RotatePlaceable();
            }
        }


        private void SwapBuildingMode(InputAction.CallbackContext context)
        {
            if (!inBuildingMode)
            {
                EnterBuildingMode();
                StartPlacingObject(debugToPlace);
            }
            else
            {
                ExitBuildingMode();
            }
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

            if (!TryGetNewPlacementPosition(out Vector3 placeablePos))
                return;

            if (!inBuildingMode)
                EnterBuildingMode();

            curPlacingOriginalGo = blueprint;
            curPlacingGo = Instantiate(blueprint, placeablePos, Quaternion.Euler(Vector3.zero));

            curPlacingPlaceable = curPlacingGo.GetComponent<Placeable>();
            curPlacingPlaceable.StartPlacing();

            UpdatePositionOfObjectBeingPlaced(true);

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

            Placeable placeable = Instantiate(curPlacingOriginalGo, curPlacingGo.transform.position, curPlacingGo.transform.rotation).GetComponent<Placeable>();
            placed.Add(placeable);

            // todo necessary when moving objects
            // curPlacingPlaceable.SetBeingPlaced(false);

            // todo separate single place mode?
            // Destroy(curPlacingGo);
            // placingObject = false;
            
            LevelGenerator.Main.UpdateNavMesh();

            // ExitBuildingMode();
        }

        private void CancelPlacingPlaceable()
        {
            if (!placingObject)
                return;

            Destroy(curPlacingGo);
            placingObject = false;
        }

        private void UpdatePositionOfObjectBeingPlaced(bool forceUpdate = false)
        {
            if (TryGetNewPlacementPosition(out var newPos))
            {
                if (previousPosition == newPos && !forceUpdate)
                    return;

                previousPosition = newPos;

                curPlacingGo.transform.position = newPos;

                curPlacingPlaceable.CheckBaseBounds(baseFlatteningMap);
                curPlacingPlaceable.CheckIntersecting();
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

        private void StartRotatingPlaceableClockwise(InputAction.CallbackContext context)
        {
            currentRotation += rotateSpeed;
        }
        
        private void StopRotatingPlaceableClockwise(InputAction.CallbackContext context)
        {
            currentRotation -= rotateSpeed;
        }
        
        private void StartRotatingPlaceableCounterclockwise(InputAction.CallbackContext context)
        {
            currentRotation -= rotateSpeed;
        }

        private void StopRotatingPlaceableCounterclockwise(InputAction.CallbackContext context)
        {
            currentRotation += rotateSpeed;
        }

        private void RotatePlaceable()
        {
            if (currentRotation == 0f)
                return;
            
            curPlacingGo.transform.Rotate(Vector3.up, currentRotation * Time.deltaTime);
        }
    }
}