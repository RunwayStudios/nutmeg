using System;
using Gameplay.Items;
using Gameplay.Items.Weapons;
using Nutmeg.Runtime.Utility.MouseController;
using Runway.Core;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputEventChannel input;
    [SerializeField] private CharacterController cc;
    [SerializeField] private StateMachine stateMachine;

    [Header("Movment")] [SerializeField] private float moveSpeed;

    [SerializeField] private Item equippedWeapon;
    [SerializeField] private GameObject equippedThrowable;
    [SerializeField] private Transform hand;

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
        input.onReloadActionPerformed += OnReloadActionPerformed;
        input.onThrowActionPerformed += OnThrowActionPerformed;
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
        if (equippedWeapon != null)
            stateAction += equippedWeapon.Use;
    }

    private void OnFireActionCanceled()
    {
        if (equippedWeapon != null)
            stateAction -= equippedWeapon.Use;
    }

    private void OnReloadActionPerformed()
    {
        //TODO check tag
        equippedWeapon.GetComponent<Weapon>().ReloadWeapon();
    }

    private void OnThrowActionPerformed()
    {
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
    }
}