﻿using System;
using System.Collections;
using Nutmeg.Runtime.Gameplay.Items;
using Nutmeg.Runtime.Utility.InputSystem;
using Nutmeg.Runtime.Utility.MouseController;
using Nutmeg.Runtime.Utility.StateMachine;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private InputEventChannel input;
        [SerializeField] private CharacterController cc;
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private Transform body;

        [Header("Movement")] [SerializeField] private float moveSpeed;
        [SerializeField] private float acceleration = .01f;
        [SerializeField] private float dashDistance = 10f;
        [SerializeField] private float dashSpeed = 1f;
        [SerializeField] private AnimationCurve dashAcceleration;


        [SerializeField] private GameObject equippedThrowable;
        [SerializeField] private Item weapon;
        [SerializeField] private Transform hand;

        public static PlayerController c_player;

        private Action stateAction;

        private Vector2 moveVector;
        private Vector3 lookTarget;

        private PlayerAnimationController animationController;

        private bool useItem;

        public bool IsWalking { get; private set; }
        public bool IsDashing { get; private set; }


        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                c_player = this;
                stateMachine.onStateEnter += OnStateEnter;
                stateMachine.onStateExit += OnStateExit;

                animationController = GetComponent<PlayerAnimationController>();

                input.onMoveActionPerformed += OnMoveActionPerformed;
                input.onMoveActionCanceled += OnMoveActionCanceled;
                input.onPrimaryActionPerformed += OnPrimaryActionPerformed;
                input.onPrimaryActionCanceled += OnPrimaryActionCanceled;
                input.onReloadActionPerformed += OnReloadActionPerformed;
                input.onSecondaryActionPerformed += OnSecondaryActionPerformed;
            }
        }

        public void Start()
        {
           
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
                case PlayerState.Dashing:
                    StartCoroutine(Dash());
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

        private Vector2 animationVelocity = Vector2.zero;

        private void OnWalking()
        {
            UpdateMoveVectorServerRpc();

            //Back or Forth
            /*if(cross.y > 0)
                animationController.UpdateFloatParam("Strafe_Velocity", .75f);
            else if(cross.y < 0)
                animationController.UpdateFloatParam("Strafe_Velocity", -.75f);
            
            
            if(cross.y < 0 && dot > 0)
                animationController.PlayCrossAnimation("Player_Rifle_Walk_Forward_Right", 0);
            else if(cross.y < 0 && dot < 0)
                animationController.PlayCrossAnimation("Player_Rifle_Walk_Backward_Right", 0);
            else if(cross.y > 0 && dot > 0)
                animationController.PlayCrossAnimation("Player_Rifle_Walk_Forward_Left", 0);
            else if(cross.y > 0 && dot < 0)
                animationController.PlayCrossAnimation("Player_Rifle_Walk_Backward_Left", 0);
                

            if(cross.y < 0 && isPerpendicular)
                animationController.PlayAnimation("Player_Rifle_Walk_Right");
            else if(cross.y > 0 && isPerpendicular)
                animationController.PlayAnimation("Player_Rifle_Walk_Left");
            else if(dot < 0.5)
                animationController.PlayAnimation("Player_Rifle_Walk_Forward");
            else if(dot > -0.5)
                animationController.PlayAnimation("Player_Rifle_Walk_Backward");
            */
            //Left or right

            Move();
            Rotate();
        }

        private void OnIdle()
        {
            animationVelocity = new Vector2(
                animationVelocity.x - (animationVelocity.x > .03f || animationVelocity.x < -.03f
                    ? animationVelocity.x > 0f ? acceleration : -acceleration
                    : animationVelocity.x),
                animationVelocity.y - (animationVelocity.y > .03f || animationVelocity.y < -.03f
                    ? animationVelocity.y > 0f ? acceleration : -acceleration
                    : animationVelocity.y));

            animationController.UpdateFloatParam("Forward_Velocity", Mathf.Clamp(animationVelocity.y, -.75f, .75f));
            animationController.UpdateFloatParam("Strafe_Velocity", Mathf.Clamp(animationVelocity.x, -.75f, .75f));

            Rotate();
        }

        private void Update()
        {
            stateAction?.Invoke();

            //Debug.Log(animationVelocity);
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

        private void OnPrimaryActionPerformed()
        {
            //animationController.SetNewAnimationParamBool("rifle", true);

            //TODO doesnt make sense to add to stateAction
            //Feels like a cheap work around
            stateAction += weapon.Use;
        }

        private void OnPrimaryActionCanceled()
        {
            //TODO doesnt make sense to add to stateAction
            stateAction -= weapon.Use;
        }

        private void OnReloadActionPerformed()
        {
            //TODO check tag
        }

        private void OnSecondaryActionPerformed()
        {
            if (!IsDashing)
                IsDashing = true;
            //TODO make more modular. Mybe the secondary wont be a nade
            //Instantiate(equippedThrowable, hand.position, hand.rotation);
        }

        private float elapsedDashTime;

        private IEnumerator Dash()
        {
            float velocity = 0f;
            while (velocity < 1f)
            {
                Debug.Log(velocity);

                //velocity = dashAcceleration.Evaluate(elapsedDashTime / dashSpeed);
                velocity = elapsedDashTime / dashSpeed;
                elapsedDashTime += Time.deltaTime;

                cc.Move((Vector3.left * moveVector.x + Vector3.back * moveVector.y) * (velocity) * dashDistance *
                        Time.deltaTime);
                yield return null;
            }

            IsDashing = false;
        }


        [ServerRpc]
        private void UpdateMoveVectorServerRpc()
        {
            float dot = Vector3.Dot(new Vector3(-moveVector.x, 0f, moveVector.y), transform.forward);
            Vector3 cross = Vector3.Cross(new Vector3(-moveVector.x, 0f, moveVector.y), transform.forward);

            //not working correctly
            //velocity gets added without needed ???

            animationVelocity = new Vector2(
                Mathf.Clamp(
                    animationVelocity.x + (cross.y != 0f
                        ? -cross.y * acceleration
                        : animationVelocity.x > .03f || animationVelocity.x < -.03f
                            ? animationVelocity.x > 0f ? -acceleration : acceleration
                            : animationVelocity.x), -Math.Abs(cross.y),
                    Math.Abs(cross.y)),
                Mathf.Clamp(
                    animationVelocity.y + (dot != 0f
                        ? -dot * acceleration
                        : animationVelocity.y > .03f || animationVelocity.y < -.03f
                            ? animationVelocity.y > 0f ? -acceleration : acceleration
                            : animationVelocity.y), -Math.Abs(dot),
                    Math.Abs(dot))
            );

            animationController.UpdateFloatParam("Forward_Velocity", Mathf.Clamp(animationVelocity.y, -.75f, .75f));
            animationController.UpdateFloatParam("Strafe_Velocity", Mathf.Clamp(animationVelocity.x, -.75f, .75f));
        }

        private void Move()
        {
            /*
            velocity = new Vector2(
                Mathf.Clamp(
                    velocity.x + (moveVector.x != 0 ? moveVector.normalized.x * acceleration :
                        velocity.x != 0 ? velocity.x > 0 ? -acceleration : acceleration : 0f), -1f, 1f),
                Mathf.Clamp(
                    velocity.y + (moveVector.y != 0 ? moveVector.normalized.y * acceleration :
                        velocity.y != 0 ? velocity.y > 0 ? -acceleration : acceleration : 0f), -1f, 1f)
            );
            */

            cc.Move((Vector3.left * moveVector.x + Vector3.back * moveVector.y) * moveSpeed *
                    Time.deltaTime);
        }


        private void Rotate()
        {
            //MouseController.UpdateMouseLookTarget();
            body.LookAt(MouseController.GetLastMouseLookTargetPoint());
            //transform.LookAt(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            body.rotation = Quaternion.Euler(0f, body.rotation.eulerAngles.y, 0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, new Vector3(-moveVector.x, 0, moveVector.y) * 2f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + new Vector3(animationVelocity.x, 0f, animationVelocity.y) * 4f, .2f);
        }
    }
}