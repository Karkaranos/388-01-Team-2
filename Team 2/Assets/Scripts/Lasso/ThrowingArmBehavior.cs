using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingArmBehavior : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public LassoBehavior Lasso;
    public PlayerBehavior PlayerBehav;

    [Header("Layers Settings:")]
    [SerializeField] private bool attachToAll = false;
    [SerializeField] private int throwableLayerNumber = 7;
    [SerializeField] private LayerMask layersToIgnore = new LayerMask();

    private float raycastDistance = 1000f;

    [Header("Transform Ref:")]
    public Transform Player;
    public Transform ThrowingArm;
    public Transform FirePoint;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistance = 20;


    [SerializeField] private float cooldownMax;
    [SerializeField] private float timer;
     public bool canShoot;
    [SerializeField] public Vector2 LassoPoint;
    [HideInInspector] public Vector2 LassoDistanceVector;
    public double LassoX;
    
    

    private void Start()
    {
        Lasso.enabled = false;
        if (hasMaxDistance)
        {
            raycastDistance = maxDistance;
        }

    }

    private void Update()
    {
        if (!canShoot)
        {
            if (timer < cooldownMax)
            {
                timer += Time.deltaTime;
            }
            else
            {
                canShoot = true;
                timer = 0;
            }
        }
    }

    

    public void SetLassoPoint()
    {
        
            
            Vector2 distanceVector = new Vector3(PlayerBehav.aiming.x * 100, PlayerBehav.aiming.y * 100, 0) - ThrowingArm.position;
            Debug.DrawLine(FirePoint.position, distanceVector.normalized);
            if (Physics2D.Raycast(FirePoint.position, distanceVector.normalized))
            {
                RaycastHit2D _hit = Physics2D.Raycast(FirePoint.position, distanceVector.normalized, raycastDistance, ~layersToIgnore);
                if (_hit)
                {
                    if (_hit.transform.gameObject.layer == throwableLayerNumber || attachToAll)
                    {
                        if (Vector2.Distance(_hit.point, FirePoint.position) <= maxDistance || !hasMaxDistance)
                        {
                            if (canShoot)
                            {
                                PlayerBehav.Throwing = true;
                                canShoot = false;
                                PlayerBehav.currentlyLassoed = _hit.transform.gameObject;
                                LassoX = _hit.point.x;
                                LassoPoint = _hit.point;
                                Debug.Log(LassoPoint);
                                LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;
                                Lasso.Missed = false;
                                Lasso.enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    /* LassoPoint = FirePoint.position + Vector3.forward * maxDistance;
                     LassoX = LassoPoint.x;
                     Debug.Log(LassoPoint);
                     LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;
                     Lasso.Missed = true;
                     Lasso.enabled = true;*/
                }

            
            }
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
