/*****************************************************************************
// File Name :         ThrowingArmBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Calculates where to throw the lasso to

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThrowingArmBehavior : MonoBehaviour
{

    //refrence the headers for what the variables do
    [Header("Scripts Ref:")]
    public LassoBehavior Lasso;
    public PlayerBehavior PlayerBehav;
    [SerializeField] private Transform aimingArrow;

    [Header("Layers Settings:")]
    [SerializeField] private bool attachToAll = false;
    [SerializeField] private int throwableLayerNumber = 7;
    [SerializeField] private LayerMask layersToIgnore = new LayerMask();
    private float defaultRaycastDistance = 1000f;

    [Header("Transform Ref:")]
    public Transform Player;
    public Transform ThrowingArm;
    public Transform FirePoint;

    [Header("Distance & Width of Lasso Hitbox:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistance = 20;
    [SerializeField] private float lassoHitboxWidth;

    [Header("Cooldown Specs")]
    [SerializeField] private float cooldownMax;
    [SerializeField] private float cooldownTimer;
    public bool offCooldown;
    [HideInInspector] public Vector2 LassoPoint;
    [HideInInspector] public Vector2 LassoDistanceVector;

    [Header("Debug:")]
    [SerializeField] private bool inDebugMode;

    [Header("Missing")]
    [SerializeField] private GameObject empty;

    public Slider LassoBar;


    /// <summary>
    /// Sets default settings
    /// </summary>
    private void Start()
    {
        cooldownTimer = cooldownMax;
        Lasso.enabled = false;
        if (hasMaxDistance)
        {
            defaultRaycastDistance = maxDistance;
        }

    }

    /// <summary>
    /// Handles cooldowns and draws the hitbox of the lasso if debug
    /// mode is enabled
    /// </summary>
    private void Update()
    {
        //Handles the lasso's cooldown
        if (!offCooldown)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }
            else
            {
                offCooldown = true;
                cooldownTimer = cooldownMax;
            }
        }

        //in debug mode, draws the boxcast that the lasso uses
        if (inDebugMode)
        {
            Vector2 distanceVector = new Vector3(PlayerBehav.aimingVector.x, PlayerBehav.aimingVector.y, 0);
            Vector2 midpoint = new Vector2(aimingArrow.position.x + (distanceVector.x * maxDistance / 2),
                  aimingArrow.position.y + (distanceVector.y * maxDistance / 2));
            float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
            BoxCastDrawer.BoxCastAllAndDraw(midpoint, new Vector2(maxDistance, lassoHitboxWidth), angle,
            distanceVector, maxDistance, ~layersToIgnore);
        }
    }

    /// <summary>
    /// updates the lasso cooldown ui
    /// </summary>
    private void FixedUpdate()
    {
        LassoBar.value = cooldownTimer;
    }

    /// <summary>
    /// Handles the throwing of the lasso
    /// </summary>
    public void SetLassoPoint()
    {
        
        AudioManager am = FindObjectOfType<AudioManager>();
        PlayerBehav.Throwing = true;

        //sets variables needed to draw the boxcast
        Vector2 distanceVector = new Vector3(PlayerBehav.aimingVector.x, PlayerBehav.aimingVector.y, 0);
        Vector2 midpoint = new Vector2(aimingArrow.position.x + (distanceVector.x * maxDistance / 2),
              aimingArrow.position.y + (distanceVector.y * maxDistance / 2));
        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;

        //checks to see if a raycast in that direction would hit anything
        if (Physics2D.Raycast(FirePoint.position, distanceVector.normalized))
        {
            //draws the hitbox and stores everything the hitbox touches
            RaycastHit2D[] potentialHits = BoxCastDrawer.BoxCastAllAndDraw(midpoint, new Vector2(maxDistance, lassoHitboxWidth), angle,
            distanceVector, maxDistance, ~layersToIgnore);
            
            //checks to see if it hit anything
            if (potentialHits.Length > 0)
            {
                RaycastHit2D closest = potentialHits[0];
                float closestDistance = 100000;

                //finds the closest throwable object
                foreach (RaycastHit2D hit in potentialHits)
                {
                    if (hit.transform.gameObject.layer == 7)
                    {
                        float hitDistance = Mathf.Sqrt(Mathf.Pow(hit.transform.position.x - aimingArrow.transform.position.x, 2) +
                        Mathf.Pow(hit.transform.position.y - aimingArrow.transform.position.y, 2));
                        if (hitDistance < closestDistance)
                        {
                            closest = hit;
                            closestDistance = hitDistance;
                        }
                    }
                    
                }

                //checks to see if you hit something throwable
                if (closest.transform.gameObject.layer == throwableLayerNumber || attachToAll)
                {
                    //checks to see if the thing you hit is within the max throwing distance
                    if (Vector2.Distance(closest.point, FirePoint.position) <= maxDistance || !hasMaxDistance)
                    {
                        //checks to see if the lasso is off cooldown
                        if (offCooldown)
                        {
                            //throws the lasso
                            PlayerBehav.lassoThrown = true;
                            offCooldown = false;

                            //has the player register what they are holding 
                            PlayerBehav.currentlyLassoed = closest.transform.gameObject;

                            //has the lassoed object register that they got picked up
                            PlayerBehav.currentlyLassoed.GetComponent<Throwable>().pickedUp = true;

                            //moves the arrow to the hit object
                            PlayerBehav.aimingArrow.HideArrow();
                            PlayerBehav.aimingArrow = closest.transform.gameObject.GetComponentInChildren<UIAimArrowBehavior>();
                            PlayerBehav.aimingArrow.ShowArrow();

                            //stops the lassoed objects momentum
                            PlayerBehav.currentlyLassoed.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                            
                            //clears everything the picked up object has bounced with
                            PlayerBehav.currentlyLassoed.GetComponent<Throwable>().bouncedWith.Clear();

                            //sets where to throw the lasso to
                            LassoPoint = closest.transform.position;
                            LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;

                            //throws lasso
                            Lasso.Missed = false;
                            Lasso.enabled = true;

                            //plays the whip crack sound
                            if (am != null)
                            {
                                am.PlayWhipCrack();
                            }
                        }

                    }
                }
                else
                {
                    //player missed

                    Vector3 spawnPosition = FirePoint.transform.position;

                    //figures out where to throw the missed lasso tos
                    if (distanceVector.magnitude < .1f)
                    {
                        float vecAngle = aimingArrow.transform.rotation.eulerAngles.z;
                        spawnPosition.x += Mathf.Cos(vecAngle * Mathf.PI / 180) * maxDistance / 2;
                        spawnPosition.y += Mathf.Sin(vecAngle * Mathf.PI / 180) * maxDistance / 2;
                    }
                    else
                    {
                        spawnPosition.x += distanceVector.x * maxDistance / 2;
                        spawnPosition.y += distanceVector.y * maxDistance / 2;
                    }

                    //makes the object to throw the lasso at and throws lasso
                    GameObject fakeHit = Instantiate(empty, spawnPosition, Quaternion.identity);
                    PlayerBehav.currentlyLassoed = fakeHit;
                    PlayerBehav.StartCoroutine(PlayerBehav.ResetMissedLasso(PlayerBehav.currentlyLassoed));
                    PlayerBehav.aimingArrow.HideArrow();
                    LassoPoint = fakeHit.transform.position;
                    LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;

                    PlayerBehav.lassoThrown = true;
                    offCooldown = false;
                    Lasso.Missed = true;
                    Lasso.enabled = true;
                    PlayerBehav.Throwing = false;
                    if (am != null)
                    {
                        am.PlayWhoosh();
                    }
                }
            }
            else
            {
                //same as above

                 Vector3 spawnPosition = FirePoint.transform.position;

                if (distanceVector.magnitude < .1f)
                {
                    float vecAngle = aimingArrow.transform.rotation.eulerAngles.z;
                    spawnPosition.x += Mathf.Cos(vecAngle * Mathf.PI / 180) * maxDistance / 2;
                    spawnPosition.y += Mathf.Sin(vecAngle * Mathf.PI / 180) * maxDistance / 2;
                }
                else
                {
                    spawnPosition.x += distanceVector.x * maxDistance / 2;
                    spawnPosition.y += distanceVector.y * maxDistance / 2;
                }
                //print(spawnPosition);
                GameObject fakeHit = Instantiate(empty, spawnPosition, Quaternion.identity);
                PlayerBehav.currentlyLassoed = fakeHit;
                PlayerBehav.StartCoroutine(PlayerBehav.ResetMissedLasso(PlayerBehav.currentlyLassoed));
                PlayerBehav.currentlyLassoed = fakeHit;
                PlayerBehav.aimingArrow.HideArrow();
                LassoPoint = fakeHit.transform.position;
                LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;

                PlayerBehav.lassoThrown = true;
                offCooldown = false;
                Lasso.Missed = true;
                Lasso.enabled = true;
                PlayerBehav.Throwing = false;
                if (am != null)
                {
                    am.PlayWhoosh();
                }

            }



        }

    }


    /// <summary>
    /// allows the player to see the max distance while in the inspector
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (FirePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(FirePoint.position, maxDistance);
        }
    }
}
