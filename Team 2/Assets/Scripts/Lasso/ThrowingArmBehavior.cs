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
   
    [HideInInspector] public Vector2 LassoPoint;
    [HideInInspector] public Vector2 LassoDistanceVector;
    

    private void Start()
    {
        Lasso.enabled = false;

    }

    private void Update()
    {
        if (PlayerBehav.lassoThrown)
        {
            SetGrapplePoint();
        }
       
        else if (!PlayerBehav.lassoThrown)
        {
            Lasso.enabled = false;
        }
        
    }

    

    void SetGrapplePoint()
    {
        //
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
                        LassoPoint = _hit.point;
                        LassoDistanceVector = LassoPoint - (Vector2)ThrowingArm.position;
                        Lasso.enabled = true;
                    }
                }
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
