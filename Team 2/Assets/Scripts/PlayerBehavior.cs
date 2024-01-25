using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Refrences:")]
    [SerializeField] private LassoBehavior Lasso;
    [SerializeField] private ThrowingArmBehavior ThrowingArm;
    [SerializeField] private CameraBehavior cameraBehav;
    private PlayerControls playerControls;
    private Rigidbody2D rb2D;

    [Header("Movement Settings:")]
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float controllerDeadzone = 0.1f;
    [SerializeField] private float controllerRotateSmoothing = 1000f;

    [Header("Debug Information:")]
    public GameObject currentlyLassoed;
    [SerializeField] private Vector2 movementVector;
    public Vector2 aimingVector;
    [SerializeField] private bool lassoThrown;
    [SerializeField] private bool Throwing;

    //Input refrences
    private PlayerInput pInput;
    private InputAction move;
    private InputAction aim;
    private InputAction throwLasso;

    //storing location
    [HideInInspector] public Vector2 roomIAmIn;

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
        aimingVector = obj.ReadValue<Vector2>();
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        movementVector = obj.ReadValue<Vector2>();
    }

    private void ThrowLasso_canceled(InputAction.CallbackContext obj)
    {
        
    }

    private void ThrowLasso_started(InputAction.CallbackContext obj)
    {
        if (!lassoThrown)
        {
            lassoThrown = true;
            if (ThrowingArm.offCooldown)
            {
                ThrowingArm.SetLassoPoint();
            }
            
        }
        else
        {
            if (!Throwing)
            {
                lassoThrown = false;
                Lasso.enabled = false;
                currentlyLassoed = null;
            }
            
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
        rb2D.velocity = movementVector * MovementSpeed;
    }

    private void HandleRotation()
    {
        
        if ((Mathf.Abs(aimingVector.x) > controllerDeadzone || Mathf.Abs(aimingVector.y) > controllerDeadzone) )
        {
            Vector2 playerDirection = new Vector2(aimingVector.x, aimingVector.y);
            if (playerDirection.sqrMagnitude > 0.0f)
            {
                Quaternion newRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, controllerRotateSmoothing * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Door")
        {
            RoomBehavior roomBehav = collision.GetComponentInParent<RoomBehavior>();
            roomIAmIn = roomBehav.gridPosition;
            cameraBehav.UpdateLocation(roomIAmIn);
        }
    }
}
