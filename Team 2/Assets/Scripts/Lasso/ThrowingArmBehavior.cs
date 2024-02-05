using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngineInternal;

public class ThrowingArmBehavior : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public LassoBehavior Lasso;
    public PlayerBehavior PlayerBehav;
    [SerializeField] private TMP_Text missText;
    [SerializeField] private Transform aimingArrow;
    [SerializeField] private TMP_Text cooldownText;

    [Header("Layers Settings:")]
    [SerializeField] private bool attachToAll = false;
    [SerializeField] private int throwableLayerNumber = 7;
    [SerializeField] private LayerMask layersToIgnore = new LayerMask();
    private float defaultRaycastDistance = 1000f;

    [Header("Transform Ref:")]
    public Transform Player;
    public Transform ThrowingArm;
    public Transform FirePoint;

    [Header("Distance & Width:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistance = 20;
    [SerializeField] private float lassoWidth = 1;
    

    [Header("Cooldown Specs")]
    [SerializeField] private float cooldownMax;
    [SerializeField] private float cooldownTimer;
    public bool offCooldown;
    [HideInInspector] public Vector2 LassoPoint;
    [HideInInspector] public Vector2 LassoDistanceVector;
    
    

    private void Start()
    {
        cooldownTimer = cooldownMax;
        Lasso.enabled = false;
        if (hasMaxDistance)
        {
            defaultRaycastDistance = maxDistance;
        }

    }

    private void Update()
    {
        if (!offCooldown)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                cooldownText.text = "Lasso Cooldown: " + cooldownTimer;
            }
            else
            {
                offCooldown = true;
                cooldownText.text = "Lasso Cooldown: 0";
                cooldownTimer = cooldownMax;
            }
        }
    }

    

    public void SetLassoPoint()
    {
        Debug.Log("Throwing Lasso");
        PlayerBehav.Throwing = true;
        Vector2 distanceVector = new Vector2(PlayerBehav.aimingVector.x, PlayerBehav.aimingVector.y);
        Debug.Log(distanceVector);
        
            if (Physics2D.Raycast(FirePoint.position, distanceVector.normalized))
            {
                RaycastHit2D _hit = Physics2D.Raycast(FirePoint.position, distanceVector.normalized, defaultRaycastDistance, ~layersToIgnore);
                if (_hit)
                {
                    if (_hit.transform.gameObject.layer == throwableLayerNumber || attachToAll)
                    {
                        if (Vector2.Distance(_hit.point, FirePoint.position) <= maxDistance || !hasMaxDistance)
                        {
                            if (offCooldown)
                            {
                            
                                if(!missText.IsActive())
                                {
                                    missText.gameObject.SetActive(true);
                                }
                                missText.text = "You Hit " + _hit.transform.gameObject.name;
                                PlayerBehav.lassoThrown = true;
                                offCooldown = false;
                                PlayerBehav.currentlyLassoed = _hit.transform.gameObject;
                                PlayerBehav.currentlyLassoed.GetComponent<Throwable>().pickedUp = true;
                                PlayerBehav.aimingArrow.HideArrow();
                                PlayerBehav.aimingArrow = _hit.transform.gameObject.GetComponentInChildren<UIAimArrowBehavior>();
                                PlayerBehav.currentlyLassoed.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                                PlayerBehav.aimingArrow.ShowArrow();
                                PlayerBehav.currentlyLassoed.GetComponent<Throwable>().bouncedWith.Clear();
                                LassoPoint = _hit.transform.position;
                                Debug.Log(LassoPoint);
                                LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;
                            
                                Lasso.Missed = false;
                                Lasso.enabled = true;
                            }
                        
                        }
                    }
                else
                {
                    missText.text = "You Missed";
                    PlayerBehav.Throwing = false;
                    /* LassoPoint = FirePoint.position + Vector3.forward * maxDistance;
                     LassoX = LassoPoint.x;
                     Debug.Log(LassoPoint);
                     LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;
                     Lasso.Missed = true;
                     Lasso.enabled = true;*/
                }
            }
            else
            {
                missText.text = "You Missed";
                PlayerBehav.Throwing = false;
                /* LassoPoint = FirePoint.position + Vector3.forward * maxDistance;
                 LassoX = LassoPoint.x;
                 Debug.Log(LassoPoint);
                 LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;
                 Lasso.Missed = true;
                 Lasso.enabled = true;*/
            }



        }
        
    }

   
    public void DrawBox()
    {
        Vector2 distanceVector = new Vector2(PlayerBehav.aimingVector.x, PlayerBehav.aimingVector.y);
        Physics2D.BoxCast(FirePoint.position, new Vector2(maxDistance, lassoWidth), Vector2.Angle(Vector2.zero, distanceVector), distanceVector.normalized);
    }


    private void OnDrawGizmosSelected()
    {
        if (FirePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(FirePoint.position, maxDistance);
        }
    }
}
