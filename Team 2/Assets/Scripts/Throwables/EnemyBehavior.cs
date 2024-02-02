/*****************************************************************************
// File Name :         EnemyBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates the base class for Enemies. Inherits from Throwable.

*****************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyBehavior : Throwable
{
    #region Variables
    [SerializeField]
    private CharacterStats stats;
    private PlayerBehavior pbehav;
    public BoxCollider2D bc2D;
    [SerializeField][Range(0,2)]
    private float flashTime;
    [SerializeField]
    [Range(0, .5f)]
    private float timeBetweenFlashes;
    bool isFlashing = false;

    public RoomBehavior roomSpawnedIn;

    [Header("Pathfinding")]
    [SerializeField]private Transform target;
    [SerializeField] private GameObject player;
    NavMeshAgent agent;
    private bool pathfindingActivated = false;
    [SerializeField]
    private float detectionRadius;
    [SerializeField]
    private float radiusActive;
    [SerializeField]
    private float maxRaycastDistance;

    [Header("Layers Settings:")]
    [SerializeField] private bool attachToAll = false;
    [SerializeField] private int playerLayerNumber = 3;
    [SerializeField] private int obstacleLayerNumber = 7;
    [SerializeField] private LayerMask layersToIgnore = new LayerMask();
    [SerializeField] private LayerMask layersToIgnoreForObs = new LayerMask();
    private float defaultRaycastDistance = 1000f;


    private bool searching;

    public CharacterStats Stats { get => stats; set => stats = value; }

    private bool canMove = true;
    private bool usingNavMesh;

    #endregion

    #region Functions

    /// <summary>
    /// Start is called on the first frame update. It gets a reference to the player
    /// and sets damage dealt
    /// </summary>
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //base.DamageDealt = stats.DamageDealt;
        obStat = stats;
        bc2D = GetComponent<BoxCollider2D>();
        bc2D.sharedMaterial = Bouncy;
        GetComponent<Renderer>().material.color = Color.red;


        
        //If the player can be found, track them.
        /*
        try
        {
            target = FindObjectOfType<PlayerBehavior>().gameObject.transform;
            pbehav = target.gameObject.GetComponent<PlayerBehavior>();
            if(SceneManager.GetActiveScene().name.Equals("MainScene"))
            {
                StartCoroutine(TrackPlayer());
            }

        }
        catch
        {
            print("Player not found");
        }*/
        

    }

    private void Update()
    {
        if (canMove && !SceneManager.GetActiveScene().name.Equals("MainScene")&&!pathfindingActivated&&target!=null)        
        {
            CheckPath();

            //player = null;
            usingNavMesh = true;
        }

        if(player == null && !searching&&!usingNavMesh )
        {
            StartCoroutine(SearchForTarget());
        }
        else if (!pathfindingActivated && player !=null&&!usingNavMesh)
        {
            StartCoroutine(TrackPlayer());
        }
    }


    private void CheckPath()
    {
        Vector2 targetPos = target.transform.position;
        Vector2 difference;

        difference.x = targetPos.x - transform.position.x;
        difference.y = targetPos.y - transform.position.y;
        RaycastHit2D _hit = Physics2D.CircleCast(transform.position, detectionRadius/2, difference, detectionRadius/2, ~layersToIgnoreForObs);
        if (_hit)
        {
            print("Enemy has located " + _hit.transform.gameObject);
            if (_hit.transform.gameObject.tag.Equals("Throwable")||_hit.transform.gameObject.tag.Equals("Enemy"))
            {
                print("should not be on player");
                agent.SetDestination(ObstacleManuvering(_hit.transform));
            }
            else if (_hit.transform.gameObject.tag.Equals("Player"))
            {
                print("tracking player");
                agent.SetDestination(target.position);
            }
        }
    }

    private Vector3 ObstacleManuvering(Transform obPos)
    {
        Vector3 obstacleToPlayer;
        //Look into a*
        Vector3 newPos = obPos.position;
        obstacleToPlayer = target.transform.position - obPos.position;

        //Change this to check current position against the target position
        if(Mathf.Abs(obstacleToPlayer.y) <.5f && Mathf.Abs(obstacleToPlayer.x) >= .5f)
        {
            newPos.x += obstacleToPlayer.x % 2;
        }
        else if (Mathf.Abs(obstacleToPlayer.y) >= .5f && Mathf.Abs(obstacleToPlayer.x) < .5f)
        {
            newPos.y += obstacleToPlayer.y % 2;
        }
        else
        {
            newPos.x += obstacleToPlayer.x % 2;
            newPos.y += obstacleToPlayer.y % 2;
        }

        print("new target: " + newPos);
        return newPos;

    }

    IEnumerator SearchForTarget()
    {
        searching = true;

        if (Physics2D.CircleCast(transform.position, 10, Vector2.zero, 10, ~layersToIgnore))
        {
            Debug.DrawLine(transform.position, new Vector3(0, detectionRadius, 0), Color.green);
            RaycastHit2D _hit = Physics2D.CircleCast(transform.position, detectionRadius, Vector2.zero, detectionRadius, ~layersToIgnore);
            if (_hit)
            {
                if (_hit.transform.gameObject.layer == playerLayerNumber)
                {
                    Vector2 dir = new Vector2(_hit.transform.position.x - transform.position.x, _hit.transform.position.y - transform.position.y);

                    Debug.DrawLine(transform.position, dir, Color.red);
                    RaycastHit2D _LOS = Physics2D.Raycast(transform.position, dir, maxRaycastDistance, ~layersToIgnore);
                    if(_LOS)
                    {
                        if(_LOS.transform.gameObject.layer == playerLayerNumber)
                        {
                            player = _LOS.transform.gameObject;
                            target = _LOS.transform;
                            print("Target  of " + player.name + " aquired");
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(.5f);
        searching = false;
    }


    /// <summary>
    /// Handles collisions with objects
    /// </summary>
    /// <param name="collision">The object collided with</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print("Hit " + collision.gameObject.name);
        CheckBounce(collision.gameObject);
        //If the object collided with is a throwable
        if (collision.gameObject.GetComponent<Throwable>()!=null)
        {
            Throwable collidedWith = collision.gameObject.GetComponent<Throwable>();
            
            //If the enemy collided with an enemy or was thrown
            if ((collidedWith.thrown || collision.gameObject.GetComponent<EnemyBehavior>() != null))
            {
                Stats.TakeDamage(collidedWith.Damage(ObjectStats.DamageTypes.TO_ENEMY));
                print("New health: " + Stats.Health);

                StartCoroutine(DamageFlash());
                if (Stats.Health == 0)
                {
                    killed = true;
                    Destroy(gameObject);
                }
            }
        }
        //If the object collided with is the player
        else if (collision.gameObject.GetComponent<PlayerBehavior>()!=null)
        {
            StartCoroutine(Freeze());
        }
    }

    /// <summary>
    /// Path towards the player's current position
    /// </summary>
    /// <returns>Time between adjustments</returns>
    IEnumerator TrackPlayer()
    {
        pathfindingActivated = true;
        print("other movement loaded");
        for(; ; )
        {
            if(!pickedUp && canMove && !usingNavMesh)
            {
                Vector2 targetPos = target.transform.position;
                Vector2 difference;
                Vector2 moveForce = Vector2.zero;

                difference.x = targetPos.x - transform.position.x;
                difference.y = targetPos.y - transform.position.y;
                if (difference.x < 0)
                {
                    moveForce.x += -1 * stats.Speed * Time.deltaTime;
                }
                else
                {
                    moveForce.x += 1 * stats.Speed * Time.deltaTime;
                }
                if (difference.y < 0)
                {
                    moveForce.y += -1 * stats.Speed * Time.deltaTime;
                }
                else
                {
                    moveForce.y += 1 * stats.Speed * Time.deltaTime;
                }
                transform.Translate(moveForce, Space.Self);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    /// <summary>
    /// Temporarily freezes the enemy
    /// </summary>
    /// <returns>The amount of time frozen for</returns>
    public IEnumerator Freeze()
    {
        canMove = false;
        yield return new WaitForSeconds(Stats.FreezeTime);
        canMove = true;
    }

    /// <summary>
    /// Changes the enemy's color to show taking damage
    /// </summary>
    /// <returns></returns>
    public IEnumerator DamageFlash()
    {
        if(!isFlashing)
        {
            StartCoroutine(Freeze());
            isFlashing = true;
            float flashedFor = 0;
            while (flashedFor < flashTime)
            {
                if (GetComponent<Renderer>().material.color == Color.white)
                {
                    GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    GetComponent<Renderer>().material.color = Color.white;
                }
                flashedFor += timeBetweenFlashes;
                yield return new WaitForSeconds(timeBetweenFlashes);
            }
            isFlashing = false;
        
        }
    }

    /// <summary>
    /// Checks whether the enemy has bounced with the current object and
    /// takes the appropriate damage type. Stops bouncing if objects have previously bounced.
    /// </summary>
    /// <param name="obj">The object bounced with</param>
    /// <returns>A bouncy or not bouncy material, depending on bounce status</returns>
    protected override PhysicsMaterial2D CheckBounce(GameObject obj)
    {
        //If it hits or bounces with the wall, take wall damage
        if (obj.tag == "Wall"&&(isBouncing))
        {
            Stats.TakeDamage(base.Damage(ObjectStats.DamageTypes.FROM_WALL));
            StartCoroutine(DamageFlash());
        }
        //If it bounces into an enemy, take bounce damage
        else if (isBouncing && obj.tag != "Enemy" && obj.name != "Tilemap" && !obj.name.Contains("Wall") && !obj.name.Contains("Door"))
        {
            Stats.TakeDamage(Damage(ObjectStats.DamageTypes.ON_BOUNCE));
            StartCoroutine(DamageFlash());
            print("hkshlsg");
        }

        print("New health: " + Stats.Health);
        if (Stats.Health == 0)
        {
            killed = true;
            Destroy(gameObject);

        }

        //If the object hasn't been bounced with previously, add it
        if (!bouncedWith.Contains(obj)&&obj.tag != "Wall" && obj.tag != "Rooms")
        {
            isBouncing = true;
            bouncedWith.Add(obj);
            return bouncy;
        }
        //Otherwise, stop bouncing
        else
        {
            //print("Hit previously bounced with");
            isBouncing = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return notBouncy;
        }
    }
    private void OnDestroy()
    {
        
        
        if (pickedUp)
        {
            pbehav.aimingArrow = pbehav.gameObject.GetComponentInChildren<UIAimArrowBehavior>();
            pbehav.ResetLasso();
        }
        if(killed)
        {
            GameObject.FindObjectOfType<GameManager>().enemyDefeated();
        }
    }
    #endregion
}
