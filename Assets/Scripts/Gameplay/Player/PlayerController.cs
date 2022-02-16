using System;
using Runway.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityTemplateProjects.Core.InputSystem;
using UnityTemplateProjects.Gameplay.Player.StateConditions;

namespace UnityTemplateProjects.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputEventChannel input;
        [SerializeField] private CharacterController cc;
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private Camera camera;
        [SerializeField] private LayerMask mask;

        [SerializeField] private Item equippedItem;

        private Action stateAction;

        private Vector2 moveVector;
        private Vector3 lookTarget;

        private bool useItem;

        public bool IsWalking { get; private set; }

        private void OnEnable()
        {
            stateMachine.onStateEnter += OnStateEnter;
            stateMachine.onStateExit += OnStateExit;

            input.onMoveActionPerformed += OnMoveActionPerformed;
            input.onMoveActionCanceled += OnMoveActionCanceled;
            input.onFireActionPerformed += OnFireActionPerformed;
            input.onFireActionCanceled += OnFireActionCanceled;
        }

        private void OnStateEnter(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    stateAction += OnIdle;
                    break;
                case PlayerState.Walking:
                    stateAction += OnWalking;
                    break;
            }
        }

        private void OnStateExit(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    stateAction -= OnIdle;
                    break;
                case PlayerState.Walking:
                    stateAction -= OnWalking;
                    break;
            }
        }

        private void OnWalking()
        {
            Move();
            Rotate();
        }

        private void OnIdle()
        {
            Rotate();
        }

        private void Update()
        {
            stateAction?.Invoke();
        }

        private void OnMoveActionPerformed(Vector2 value)
        {
            IsWalking = true;
            moveVector = value;
        }

        private void OnMoveActionCanceled(Vector2 value)
        {
            IsWalking = false;
            moveVector = value;
        }

        private void OnFireActionPerformed()
        {
            if (equippedItem != null)
                stateAction += equippedItem.Use;
        }

        private void OnFireActionCanceled()
        {
            if (equippedItem != null)
                stateAction -= equippedItem.Use;
        }

        private RaycastHit? ShootRayFromCameraToMouse()
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, mask))
            {
                Debug.DrawRay(camera.transform.position,
                    camera.transform.position + (hit.point - camera.transform.position) * 100f);
                return hit;
            }

            return null;
        }

        private void Move()
        {
            cc.Move((Vector3.left * moveVector.x + Vector3.back * moveVector.y) * Time.deltaTime);
        }

        private void UpdateLookTarget()
        {
            lookTarget = ShootRayFromCameraToMouse()?.point ?? lookTarget;
        }

        private void Rotate()
        {
            UpdateLookTarget();
            transform.LookAt(lookTarget);
            //transform.LookAt(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        private void OnDrawGizmos()
        {
        }
    }
}