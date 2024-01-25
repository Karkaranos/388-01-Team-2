/*****************************************************************************
// File Name :         EnemyBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates the base class for Enemies. Inherits from Throwable.

*****************************************************************************/
using UnityEngine;
using System.Collections;

public class EnemyBehavior : Throwable
{
    [SerializeField]
    private CharacterStats stats;
    private GameObject player;
    public BoxCollider2D bc2D;
    [SerializeField][Range(0,2)]
    private float flashTime;
    [SerializeField]
    [Range(0, .5f)]
    private float timeBetweenFlashes;
    bool isFlashing = false;

    public CharacterStats Stats { get => stats; set => stats = value; }

    private bool canMove = true;

    /// <summary>
    /// Start is called on the first frame update. It gets a reference to the player
    /// and sets damage dealt
    /// </summary>
    private void Start()
    {
        //base.DamageDealt = stats.DamageDealt;
        base.obStat = stats;
        bc2D = GetComponent<BoxCollider2D>();
        bc2D.sharedMaterial = base.Bouncy;
        GetComponent<Renderer>().material.color = Color.red;
        try
        {
            player = FindObjectOfType<PlayerBehavior>().gameObject;
            StartCoroutine(TrackPlayer());
        }
        catch
        {
            print("Player not found");
        }


    }

    /// <summary>
    /// Handles collisions with objects
    /// </summary>
    /// <param name="collision">The object collided with</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the object collided with is a throwable
        if (collision.gameObject.GetComponent<Throwable>()!=null)
        {
            Throwable collidedWith = collision.gameObject.GetComponent<Throwable>();
           
            //If the other object was thrown or both are enemies
            if(isBouncing)
            {
                bc2D.sharedMaterial = base.BounceCount();
                //print(isBouncing);
                Stats.TakeDamage(Damage(ObjectStats.DamageTypes.ON_BOUNCE));

                StartCoroutine(DamageFlash());
                print("New health: " + Stats.Health);
                if (Stats.Health == 0)
                {
                    Destroy(gameObject);
                }
                isBouncing = false;
            }
            else if (!isBouncing&&(collidedWith.thrown || collision.gameObject.GetComponent<EnemyBehavior>() != null))
            {
                Stats.TakeDamage(collidedWith.Damage(ObjectStats.DamageTypes.TO_ENEMY));
                print("New health: " + Stats.Health);

                StartCoroutine(DamageFlash());
                if (Stats.Health == 0)
                {
                    Destroy(gameObject);
                }
                isBouncing = true;
                bc2D.sharedMaterial = base.BounceCount();
            }
            //If this object was thrown
            else
            {
                //Maybe do something here later
            }
        }
        //If the object collided with is the player
        else if (collision.gameObject.GetComponent<PlayerBehavior>()!=null)
        {
            StartCoroutine(Freeze());
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Stats.TakeDamage(base.Damage(ObjectStats.DamageTypes.FROM_WALL));
            StartCoroutine(DamageFlash());
            print("New health: " + Stats.Health);
            if (Stats.Health == 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            bc2D.sharedMaterial = base.BounceCount();
        }
    }

    /// <summary>
    /// Path towards the player's current position
    /// </summary>
    /// <returns>Time between adjustments</returns>
    IEnumerator TrackPlayer()
    {
        for(; ; )
        {
            if(!pickedUp && canMove)
            {
                Vector2 targetPos = player.transform.position;
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


}
