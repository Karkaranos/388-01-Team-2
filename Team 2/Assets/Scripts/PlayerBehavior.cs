/*****************************************************************************
// File Name :         PlayerBehavior.cs
// Author :            Tyler Hayes, Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Handles the player's behavior

*****************************************************************************/

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
    [Header("Oasis")]
    [SerializeField]
    private float healPercent;
    [SerializeField]
    private bool oasesGiveManyHeal;

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
    private InputAction pause;

    [Header("Player Information:")]
    [SerializeField] private CharacterStats stats;
    [SerializeField]
    private bool _invincible;
    private float timer;

    public Slider HealthBar;

    //storing location
    [HideInInspector] public Vector2 roomIAmIn;
    private bool coroutineStarted = false;

    /// <summary>
    /// sets variables
    /// </summary>
    public void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
        pInput = GetComponent<PlayerInput>();

        move = pInput.actions.FindAction("Movement");
        aim = pInput.actions.FindAction("AimLasso");
        throwLasso = pInput.actions.FindAction("Throw");
        pause = pInput.actions.FindAction("Pause");

        if(pause == null)
        {
            print("Pause not found");
        }
        else
        {

            pause.performed += Pause_performed;
        }
        move.performed += Move_performed;


        aim.performed += Aim_performed;


        throwLasso.started += ThrowLasso_started;
        throwLasso.canceled += ThrowLasso_canceled;
        PlayerPrefs.SetInt("CurrentScore", 0);
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        FindObjectOfType<GameMenuController>().Pause();
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

    /// <summary>
    /// throws the lasso or throws the object the player is holding
    /// </summary>
    /// <param name="obj"></param>
    private void ThrowLasso_started(InputAction.CallbackContext obj)
    {
        //if they can throw
        if (timer >= 0.1 && !Throwing && !FindObjectOfType<GameMenuController>().isPaused)
        {
            timer = 0;
            //throw something if they are holding it
            if (currentlyLassoed != null && !currentlyLassoed.tag.Equals("Temp"))
            {
                print("error");
                currentlyLassoed.GetComponent<Throwable>().GetThrown(aimingVector);
                
            }
            else if (currentlyLassoed != null && currentlyLassoed.tag.Equals("Temp")&&!coroutineStarted)
            {
                print("should run");
                //StartCoroutine(ResetMissedLasso(currentlyLassoed));
            }
            else
            {
                //throws the lasso
                if (!lassoThrown)
                {

                    if (ThrowingArm.offCooldown)
                    {
                        ThrowingArm.SetLassoPoint();
                    }

                }
                else
                {
                    //resets the lasso
                    ResetLasso();
                }
            }
            
            
        }
        
    }

    /// <summary>
    /// resets all variables related to the lasso
    /// </summary>
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

    /// <summary>
    /// resets the missed lasso
    /// </summary>
    /// <param name="temp"></param>
    /// <returns></returns>
    public IEnumerator ResetMissedLasso(GameObject temp)
    {
        coroutineStarted = true;
        print("Reset missed lasso");
        yield return new WaitForSeconds(.5f);
        coroutineStarted = false;
        lassoThrown = false;
        Lasso.enabled = false;
        Throwing = false;
        aimingArrow.HideArrow();
        currentlyLassoed = null;
        aimingArrow = GetComponentInChildren<UIAimArrowBehavior>();
        aimingArrow.ShowArrow();
        print("destroyed");
        Destroy(temp);
    }

    /// <summary>
    /// enables the controls
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    /// <summary>
    /// disables the controls
    /// </summary>
    private void OnDisable()
    {
        playerControls.Disable();
    }

    /// <summary>
    /// moves and rotates if the player is inputting
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;
        HandleMovement();
        HandleRotation();
    }

    /// <summary>
    /// moves the player
    /// </summary>
    private void HandleMovement()
    {
        if ((canMoveWhileLassoing || !Lasso.enabled) && !FindObjectOfType<GameMenuController>().isPaused)
        {
            rb2D.velocity = movementVector * MovementSpeed;
        }
        else
        {
            rb2D.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// rotates the arrow around the player
    /// </summary>
    private void HandleRotation()
    {
        if (!FindObjectOfType<GameMenuController>().isPaused)
        {
            aimingArrow.Aim(aimingVector, controllerDeadzone, controllerRotateSmoothing);
        }
    }

    /// <summary>
    /// sends to the roombehavior when the player enters the room
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<RoomBehavior>() != null)
        {
            RoomBehavior roomBehav = collision.GetComponentInParent<RoomBehavior>();

            //if its the door it moves the camera and checks if the player
            //has reached the end
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

            //spawns enemies when the player enters the spawn hitbox
            if (collision.tag == "Spawn")
            {
                if (!roomBehav.hasBeenVisited && roomBehav.gridPosition != new Vector2(0, 0))
                {
                    roomBehav.SpawnEnemies();
                }
            }

            //heals the player when they reach the oasis
            if (collision.gameObject.tag.Equals("Oasis"))
            {
                AudioManager am = FindObjectOfType<AudioManager>();
                if(am!=null)
                {
                    am.PlayHeal();
                }
                stats.Heal(healPercent, false, 1);

                //destroys the trigger if the oasis doesnt give multiple heals
                if(!oasesGiveManyHeal)
                {
                    Destroy(collision.gameObject.GetComponentInChildren<ParticleSystem>().gameObject);
                    Destroy(collision.gameObject.GetComponent<Collider2D>());
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
            if (/*!collidedWith.thrown &&*/ !_invincible && collidedWith.GetComponent<EnemyBehavior>())
            {
                print("Player attacked by Enemy");
                stats.TakeDamage(collidedWith.Damage(ObjectStats.DamageTypes.TO_PLAYER));
                print("New health: " + stats.Health);
                AudioManager am = FindObjectOfType<AudioManager>();
                if (am != null)
                {
                    am.PlayPlayerDamage();
                }

                if (stats.Health <= 0)
                {
                    StartCoroutine(GameEnd());
                }
                StartCoroutine(Invincible());
            }
        }
    }

    /// <summary>
    /// updates the health bar
    /// </summary>
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
        GetComponent<SpriteRenderer>().color = new Color(0,1,1, .5f);
        yield return new WaitForSeconds(stats.InvincibilityTime);
        GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
        _invincible = false;
    }

    /// <summary>
    /// ends the game
    /// </summary>
    /// <returns></returns>
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
