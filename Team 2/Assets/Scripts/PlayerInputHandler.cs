using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;

    private PlayerInput pInput;
    private InputAction move;
    private InputAction aim;
    public InputAction throwLasso;

    private PlayerControls playerControls;

    private PlayerBehavior playerBehavior;

    private Vector2 movement;
    private Vector2 aiming;

    private Vector3 playerVelocity;


    public bool lassoThrown;
    private GameObject currentObject;


    public void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        playerControls = new PlayerControls();
        pInput = GetComponent<PlayerInput>();

        move.performed += Move_performed;

        aim.performed += Aim_performed;

        throwLasso.performed += ThrowLasso_performed;
        throwLasso.canceled += ThrowLasso_canceled;
    }

    private void Aim_performed(InputAction.CallbackContext obj)
    {
        movement = obj.ReadValue<Vector2>();
        aiming = obj.ReadValue<Vector2>();
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        movement = obj.ReadValue<Vector2>();
    }

    private void ThrowLasso_canceled(InputAction.CallbackContext obj)
    {
        lassoThrown = false;
    }

    private void ThrowLasso_performed(InputAction.CallbackContext obj)
    {
        if (!lassoThrown)
        {
            lassoThrown = true;
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    private void HandleInput()
    {


    }

    private void HandleMovement()
    {

    }

    private void HandleRotation()
    {

    }
}
