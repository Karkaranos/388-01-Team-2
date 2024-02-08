using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;

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
    [SerializeField] private float lassoHitboxWidth;

    [Header("Cooldown Specs")]
    [SerializeField] private float cooldownMax;
    [SerializeField] private float cooldownTimer;
    public bool offCooldown;
    [HideInInspector] public Vector2 LassoPoint;
    [HideInInspector] public Vector2 LassoDistanceVector;

    [Header("Debug:")]
    [SerializeField] private bool inDebugMode;



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
        if (inDebugMode)
        {
            Vector2 distanceVector = new Vector3(PlayerBehav.aimingVector.x, PlayerBehav.aimingVector.y, 0);
            Vector2 midpoint = new Vector2(FirePoint.position.x + (distanceVector.x * maxDistance / 2),
                  FirePoint.position.y + (distanceVector.y * maxDistance / 2));
            float angle = Mathf.Atan2(distanceVector.x, distanceVector.y) * Mathf.Rad2Deg - 90;
            BoxCastDrawer.BoxCastAllAndDraw(midpoint, new Vector2(maxDistance, lassoHitboxWidth), angle,
            distanceVector, maxDistance, ~layersToIgnore);
        }
    }



    public void SetLassoPoint()
    {
        Debug.Log("Throwing Lasso");
        PlayerBehav.Throwing = true;
        Vector2 distanceVector = new Vector3(PlayerBehav.aimingVector.x, PlayerBehav.aimingVector.y, 0);
        Vector2 midpoint = new Vector2(FirePoint.position.x + (distanceVector.x * maxDistance / 2),
              FirePoint.position.y + (distanceVector.y * maxDistance / 2));
        //Debug.DrawLine(FirePoint.position, distanceVector.normalized, Color.green);
        if (Physics2D.Raycast(FirePoint.position, distanceVector.normalized))
        {
            float angle = Vector2.Angle(Player.transform.position, aimingArrow.transform.position);
            //RaycastHit2D _hit = Physics2D.Raycast(FirePoint.position, distanceVector.normalized, defaultRaycastDistance, ~layersToIgnore);
            RaycastHit2D[] potentialHits = BoxCastDrawer.BoxCastAllAndDraw(midpoint, new Vector2(maxDistance, lassoHitboxWidth), angle,
            distanceVector, maxDistance, ~layersToIgnore);
            
            if (potentialHits.Length > 0)
            {
                RaycastHit2D closest = potentialHits[0];
                float closestDistance = 100000;
                foreach (RaycastHit2D hit in potentialHits)
                {
                    float hitDistance = Mathf.Sqrt(Mathf.Pow(hit.transform.position.x - FirePoint.transform.position.x, 2) +
                        Mathf.Pow(hit.transform.position.y - FirePoint.transform.position.y, 2));
                    if (hitDistance < closestDistance)
                    {
                        closest = hit;
                        closestDistance = hitDistance;
                    }
                }
                if (closest.transform.gameObject.layer == throwableLayerNumber || attachToAll)
                {
                    if (Vector2.Distance(closest.point, FirePoint.position) <= maxDistance || !hasMaxDistance)
                    {
                        if (offCooldown)
                        {
                            if (!missText.IsActive())
                            {
                                missText.gameObject.SetActive(true);
                            }
                            missText.text = "You Hit " + closest.transform.gameObject.name;
                            PlayerBehav.lassoThrown = true;
                            offCooldown = false;
                            PlayerBehav.currentlyLassoed = closest.transform.gameObject;
                            PlayerBehav.currentlyLassoed.GetComponent<Throwable>().pickedUp = true;
                            PlayerBehav.aimingArrow.HideArrow();
                            PlayerBehav.aimingArrow = closest.transform.gameObject.GetComponentInChildren<UIAimArrowBehavior>();
                            PlayerBehav.currentlyLassoed.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                            PlayerBehav.aimingArrow.ShowArrow();
                            PlayerBehav.currentlyLassoed.GetComponent<Throwable>().bouncedWith.Clear();
                            LassoPoint = closest.transform.position;
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



    private void OnDrawGizmosSelected()
    {
        if (FirePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(FirePoint.position, maxDistance);
        }
    }
}
