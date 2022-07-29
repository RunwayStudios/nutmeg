﻿using System;
using Mirror;
using Nutmeg.Runtime.Gameplay.Items;
using Nutmeg.Runtime.Gameplay.Weapons;
using Nutmeg.Runtime.Utility.InputSystem;
using Nutmeg.Runtime.Utility.MouseController;
using Nutmeg.Runtime.Utility.StateMachine;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private InputEventChannel input;
        [SerializeField] private CharacterController cc;
        [SerializeField] private StateMachine stateMachine;

        [Header("Movement")] [SerializeField] private float moveSpeed;

        [SerializeField] private Item equippedWeapon;
        [SerializeField] private GameObject equippedThrowable;
        [SerializeField] private Transform hand;

        public static PlayerController c_player;

        private Action stateAction;

        private Vector2 moveVector;
        private Vector3 lookTarget;

        private PlayerAnimationController animationController;

        private bool useItem;

        public bool IsWalking { get; private set; }

        private void Start()
        {
            stateMachine.onStateEnter += OnStateEnter;
            stateMachine.onStateExit += OnStateExit;

            animationController = GetComponent<PlayerAnimationController>();
            
            if (isLocalPlayer)
            {
                c_player = this;

                input.onMoveActionPerformed += OnMoveActionPerformed;
                input.onMoveActionCanceled += OnMoveActionCanceled;
                input.onPrimaryActionPerformed += OnPrimaryActionPerformed;
                input.onPrimaryActionCanceled += OnPrimaryActionCanceled;
                input.onReloadActionPerformed += OnReloadActionPerformed;
                input.onSecondaryActionPerformed += OnSecondaryActionPerformed;
            }
        }

        private void OnStateEnter(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    stateAction += OnIdle;
                    animationController.PlayAnimation("Player_Rifle_Idle_1");
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

            float dot = Vector3.Dot(new Vector3(-moveVector.x, 0, moveVector.y), transform.forward); 
            Vector3 cross = Vector3.Cross(new Vector3(-moveVector.x, 0, moveVector.y), transform.forward); 
            Debug.Log(dot);

            bool isPerpendicular = dot > -0.5 && dot < 0.5;
            
            animationController.UpdateFloatParam("Player_Rifle_Walk", Math.Abs(dot));
            
            //Back or Forth
            if(cross.y < 0 && dot > 0.5)
                animationController.PlayBlendAnimation("Player_Rifle_Walk_Forward_Right");
            else if(cross.y < 0 && dot < -0.5)
                animationController.PlayBlendAnimation("Player_Rifle_Walk_Backward_Right");
            else if(cross.y > 0 && dot > 0.5)
                animationController.PlayBlendAnimation("Player_Rifle_Walk_Forward_Left");
            else if(cross.y > 0 && dot < -0.5)
                animationController.PlayBlendAnimation("Player_Rifle_Walk_Backward_Left");
                

            /*if(cross.y < 0 && isPerpendicular)
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

        private void OnPrimaryActionPerformed()
        {
            //animationController.SetNewAnimationParamBool("rifle", true);
            animationController.PlayAnimation("equip rifle");
            
            //TODO doesnt make sense to add to stateAction
            //Feels like a cheap work around
            if (equippedWeapon != null)
                stateAction += equippedWeapon.Use;
        }

        private void OnPrimaryActionCanceled()
        {
            //TODO doesnt make sense to add to stateAction
            if (equippedWeapon != null)
                stateAction -= equippedWeapon.Use;
        }

        private void OnReloadActionPerformed()
        {
            //TODO check tag
            equippedWeapon.GetComponent<Weapon>().ReloadWeapon();
        }

        private void OnSecondaryActionPerformed()
        {
            //TODO make more modular. Mybe the secondary wont be a nade
            Instantiate(equippedThrowable, hand.position, hand.rotation);
        }

        private void Move()
        {
            cc.Move((Vector3.left * moveVector.x + Vector3.back * moveVector.y) * moveSpeed * Time.deltaTime);
        }


        private void Rotate()
        {
            MouseController.UpdateMouseLookTarget();
            transform.LookAt(MouseController.GetLastMouseLookTargetPoint());
            //transform.LookAt(camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, new Vector3(-moveVector.x, 0, moveVector.y) * 2f);
        }
    }
}