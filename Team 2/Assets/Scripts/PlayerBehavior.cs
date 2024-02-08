using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Refrences:")]
    [SerializeField] private LassoBehavior Lasso;
    [SerializeField] private ThrowingArmBehavior ThrowingArm;
    [SerializeField] private CameraBehavior cameraBehav;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private GameManager GM;
    public UIAimArrowBehavior aimingArrow;
    private PlayerControls playerControls;
    private Rigidbody2D rb2D;
    


    [Header("Movement Settings:")]
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float controllerDeadzone = 0.1f;
    [SerializeField] private float controllerRotateSmoothing = 1000f;
    [SerializeField] public static bool canMoveWhileLassoing;

    [Header("Debug Information:")]
    public GameObject currentlyLassoed;
    
    [SerializeField] private Vector2 movementVector;
    public Vector2 aimingVector;
    [SerializeField] public bool lassoThrown;
    public bool Throwing;

    //Input refrences
    private PlayerInput pInput;
    private InputAction move;
    private InputAction aim;
    private InputAction throwLasso;
    private InputAction quit;

    [Header("Player Information:")]
    [SerializeField] private CharacterStats stats;
    [SerializeField]
    private bool _invincible;
    private float timer;

    public Slider HealthBar;

    //storing location
    [HideInInspector] public Vector2 roomIAmIn;
    private bool coroutineStarted = false;

    public void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
        pInput = GetComponent<PlayerInput>();

        move = pInput.actions.FindAction("Movement");
        aim = pInput.actions.FindAction("AimLasso");
        throwLasso = pInput.actions.FindAction("Throw");
        quit = pInput.actions.FindAction("Quit");

        quit.started += Quit_started;
        move.performed += Move_performed;


        aim.performed += Aim_performed;


        throwLasso.started += ThrowLasso_started;
        throwLasso.canceled += ThrowLasso_canceled;
        PlayerPrefs.SetInt("CurrentScore", 0);
    }

    private void Quit_started(InputAction.CallbackContext obj)
    {
        GM.Quit();
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
        if (timer >= 0.1 && !Throwing)
        {
            timer = 0;
            if (currentlyLassoed != null && !currentlyLassoed.tag.Equals("Temp"))
            {
                print("error");
                currentlyLassoed.GetComponent<Throwable>().GetThrown(aimingVector);
                
            }
            else if (currentlyLassoed != null && currentlyLassoed.tag.Equals("Temp")&&!coroutineStarted)
            {
                print("should run");
                StartCoroutine(ResetMissedLasso(currentlyLassoed));
            }
            else
            {
                if (!lassoThrown)
                {

                    if (ThrowingArm.offCooldown)
                    {
                        ThrowingArm.SetLassoPoint();
                    }

                }
                else
                {



                    print("Error 2");
                    if(currentlyLassoed != null && currentlyLassoed.tag.Equals("Temp"))
                    {
                        StartCoroutine(ResetMissedLasso(currentlyLassoed));
                    }
                    else
                    {
                        ResetLasso();
                    }


                    

                }
            }
            
            
        }
        
    }
    public void ResetLasso()
    {
        print("Reset caught lasso");
        if (currentlyLassoed != null)
        {
            if(currentlyLassoed.GetComponent<Throwable>()!=null)
            {
                currentlyLassoed.GetComponent<Throwable>().pickedUp = false;
            }
        }
        lassoThrown = false;
        Lasso.enabled = false;
        Throwing = false;
        aimingArrow.HideArrow();
        currentlyLassoed = null;
        aimingArrow = GetComponentInChildren<UIAimArrowBehavior>();
        aimingArrow.ShowArrow();
    }

    IEnumerator ResetMissedLasso(GameObject temp)
    {
        coroutineStarted = true;
        print("Reset missed lasso");
        yield return new WaitForSeconds(.3f);
        coroutineStarted = false;
        lassoThrown = false;
        Lasso.enabled = false;
        Throwing = false;
        aimingArrow.HideArrow();
        currentlyLassoed = null;
        aimingArrow = GetComponentInChildren<UIAimArrowBehavior>();
        aimingArrow.ShowArrow();
        Destroy(temp);
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
        timer += Time.deltaTime;   
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
        if (collision.GetComponentInParent<RoomBehavior>() != null)
        {
            RoomBehavior roomBehav = collision.GetComponentInParent<RoomBehavior>();
            if (collision.tag == "Door")
            {

                roomIAmIn = roomBehav.gridPosition;
                cameraBehav.UpdateLocation(roomIAmIn);

                if (roomIAmIn == roomGenerator.bottomRightRoom)
                {
                    transform.SetParent(collision.transform);
                    roomGenerator.ReachedTheEnd();
                }

            }
            if (collision.tag == "Spawn")
            {

                if (!roomBehav.hasBeenVisited && roomBehav.gridPosition != new Vector2(0, 0))
                {
                    roomBehav.SpawnEnemies();
                }
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
            Throwable collidedWith = collision.gameObject.GetComponent<Throwable>();

            //If the enemy was not thrown
            if (!collidedWith.thrown && !_invincible && collidedWith.GetComponent<EnemyBehavior>())
            {
                print("Player attacked by Enemy");
                stats.TakeDamage(collidedWith.Damage(ObjectStats.DamageTypes.TO_PLAYER));
                print("New health: " + stats.Health);

                if(stats.Health <= 0)
                {
                    StartCoroutine(GameEnd());
                }
                StartCoroutine(Invincible());
            }
        }
    }

    private void FixedUpdate()
    {
        HealthBar.value = stats.Health;
    }


    /// <summary>
    /// Temporarily makes the player invincible
    /// </summary>
    /// <returns>The amount of time the player is invincible for</returns>
    IEnumerator Invincible()
    {
        _invincible = true;
        GetComponent<SpriteRenderer>().color = Color.cyan;
        yield return new WaitForSeconds(stats.InvincibilityTime);
        GetComponent<SpriteRenderer>().color = Color.white;
        _invincible = false;
    }

    IEnumerator GameEnd()
    {
        List<GameObject> allEnemies = new List<GameObject>();
        allEnemies.Add(FindAnyObjectByType<EnemyBehavior>().gameObject);
        foreach(GameObject g in allEnemies)
        {
            Destroy(g);
        }
        yield return new WaitForSeconds(.01f);
        SceneManager.LoadScene("EndScene");
    }
}
