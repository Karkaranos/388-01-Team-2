using Cinemachine;
using System;
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
    [SerializeField] private RoomGenerator roomGenerator;
    public UIAimArrowBehavior aimingArrow;
    private PlayerControls playerControls;
    private Rigidbody2D rb2D;
    


    [Header("Movement Settings:")]
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float controllerDeadzone = 0.1f;
    [SerializeField] private float controllerRotateSmoothing = 1000f;
    [SerializeField] private bool canMoveWhileLassoing;

    [Header("Debug Information:")]
    public GameObject currentlyLassoed;
    
    [SerializeField] private Vector2 movementVector;
    public Vector2 aimingVector;
    [SerializeField] private bool lassoThrown;
    public bool Throwing;

    //Input refrences
    private PlayerInput pInput;
    private InputAction move;
    private InputAction aim;
    private InputAction throwLasso;

    [Header("Player Information:")]
    [SerializeField] private CharacterStats stats;
    private bool _invincible;

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
                currentlyLassoed.GetComponent<Throwable>().pickedUp = false;
                aimingArrow.HideArrow();
                currentlyLassoed = null;
                aimingArrow = GetComponentInChildren<UIAimArrowBehavior>();
                aimingArrow.ShowArrow();
                
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
        if (canMoveWhileLassoing || !Lasso.enabled)
        {
            rb2D.velocity = movementVector * MovementSpeed;
        }
        else
        {
            rb2D.velocity = Vector2.zero;
        }
        
    }

    private void HandleRotation()
    {
        aimingArrow.Aim(aimingVector, controllerDeadzone, controllerRotateSmoothing);
        
            /*if ((Mathf.Abs(aimingVector.x) > controllerDeadzone || Mathf.Abs(aimingVector.y) > controllerDeadzone))
            {
                Vector2 playerDirection = new Vector2(aimingVector.x, aimingVector.y);
                if (playerDirection.sqrMagnitude > 0.0f)
                {
                    Quaternion newRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, controllerRotateSmoothing * Time.deltaTime);
                }
            }*/
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Door")
        {
            RoomBehavior roomBehav = collision.GetComponentInParent<RoomBehavior>();
            roomIAmIn = roomBehav.gridPosition;
            transform.SetParent(collision.transform);
            cameraBehav.UpdateLocation(roomIAmIn);
            if (!roomBehav.hasBeenVisited)
            {
                roomBehav.SpawnEnemies();
            }
            if (roomIAmIn == roomGenerator.bottomRightRoom)
            {
                roomGenerator.ReachedTheEnd();
            }
        }
    }

    /// <summary>
    /// Occurs when the player collides with another object. 
    /// Handles damage taken.
    /// </summary>
    /// <param name="collision">The object collided with</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the object collided with is a throwable
        if (collision.gameObject.GetComponent<Throwable>() != null)
        {
            Throwable collidedWith = collision.gameObject.GetComponent<EnemyBehavior>();

            //If the enemy was not thrown
            if (!collidedWith.thrown && !_invincible)
            {
                print("Player attacked by Enemy");
                stats.TakeDamage(collidedWith.Damage(ObjectStats.DamageTypes.TO_PLAYER));
                print("New health: " + stats.Health);
                StartCoroutine(Invincible());
            }
        }
    }

    /// <summary>
    /// Temporarily makes the player invincible
    /// </summary>
    /// <returns>The amount of time the player is invincible for</returns>
    IEnumerator Invincible()
    {
        _invincible = true;
        yield return new WaitForSeconds(stats.InvincibilityTime);
        _invincible = false;
    }
}
