using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float controllerDeadzone = 0.1f;
    [SerializeField] private float controllerRotateSmoothing = 1000f;

    private PlayerInput pInput;
    private InputAction move;
    private InputAction aim;
    private InputAction throwLasso;

    private PlayerControls playerControls;

    private Rigidbody2D rb2D;

    [SerializeField] private bool isAiming;
    [SerializeField] private bool isMoving;
    [SerializeField] private Vector2 movement;
    public Vector2 aiming;

    private Vector3 playerVelocity;


    public bool lassoThrown;
    private GameObject currentObject;
    [SerializeField] private LassoBehavior Lasso;


    public void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
        pInput = GetComponent<PlayerInput>();

        move = pInput.actions.FindAction("Movement");
        aim = pInput.actions.FindAction("AimLasso");
        throwLasso = pInput.actions.FindAction("Throw");


        move.performed += Move_performed;


        aim.performed += Aim_performed;


        throwLasso.started += ThrowLasso_started;
        throwLasso.canceled += ThrowLasso_canceled;
    }



    private void Aim_performed(InputAction.CallbackContext obj)
    {
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

    private void ThrowLasso_started(InputAction.CallbackContext obj)
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
        
            HandleMovement();
        
        
            HandleRotation();
        
        
    }

    private void HandleMovement()
    {
        rb2D.velocity = movement * MovementSpeed;
    }

    private void HandleRotation()
    {
        //&& !Lasso.isGrappling
        if ((Mathf.Abs(aiming.x) > controllerDeadzone || Mathf.Abs(aiming.y) > controllerDeadzone) )
        {
            Vector2 playerDirection = new Vector2(aiming.x, aiming.y);
            if (playerDirection.sqrMagnitude > 0.0f)
            {
                Quaternion newRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, controllerRotateSmoothing * Time.deltaTime);
            }
        }
    }

}
