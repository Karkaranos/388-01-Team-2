/*****************************************************************************
// File Name :         Throwable.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates the parent Throwable class

*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Throwable : MonoBehaviour
{
    #region Variables

    public bool thrown = false;
    public bool pickedUp = false;
    public bool killed;

    
    [SerializeField] protected float forceModifier;
    [SerializeField] protected float maxVelocity;
    protected float hiddenModifier = 100;


    private float damageDealt;
    public ObjectStats obStat;

    [Header("Bouncing:")]
    [SerializeField] protected PhysicsMaterial2D notBouncy;
    [SerializeField] protected PhysicsMaterial2D bouncy;
    [SerializeField] private int maxBounceCount;
    public bool isBouncing;

    public List<GameObject> bouncedWith = new List<GameObject>();


    public PhysicsMaterial2D Bouncy { get => bouncy; }

    #endregion

    #region Functions

    /// <summary>
    /// Deals the appropriate type of damage
    /// </summary>
    /// <param name="type">The type of damage to deal</param>
    /// <returns>The value of damage</returns>
    public float Damage(ObjectStats.DamageTypes type)
    {
        //print(type);
        if (type == ObjectStats.DamageTypes.TO_PLAYER)
        {
            return obStat.DMGToPlayer;
        }
        else if (type == ObjectStats.DamageTypes.TO_ENEMY)
        {
            return obStat.DMGToEnemy;
        }
        else if (type == ObjectStats.DamageTypes.ON_BOUNCE)
        {
            return obStat.BounceDMG;
        }
        else
        {
            return 25;
        }
    }

    /// <summary>
    /// Checks whether the object has bounced with the current object. Stops bouncing if objects have previously bounced.
    /// Can be overwritten by child classes
    /// </summary>
    /// <param name="obj">The object bounced with</param>
    /// <returns>A bouncy or not bouncy material, depending on bounce status</returns>
    protected virtual PhysicsMaterial2D CheckBounce(GameObject obj)
    {
        if (!bouncedWith.Contains(obj))
        {
            isBouncing = true;
            bouncedWith.Add(obj);
            return bouncy;
        }
        else
        {
            isBouncing = false;
            thrown = false;
            return notBouncy;
        }
    }

    /// <summary>
    /// Applies force to an object to mimic being thrown
    /// </summary>
    /// <param name="arrow">The aiming arrow's position</param>
    public void GetThrown(Vector2 arrow)
    {
        //Reset force and grab references
        thrown = true;
        bouncedWith.Clear();
        PlayerBehavior pbehav = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        //Calculate the angle being thrown at based on the arrow's position and add a consistent force
        float angle = Mathf.Atan2(arrow.y, arrow.x) * Mathf.Rad2Deg;
        float xForce = Mathf.Cos(angle * Mathf.PI / 180) * forceModifier * hiddenModifier;
        float yForce = Mathf.Sin(angle * Mathf.PI / 180) * forceModifier * hiddenModifier;
        Vector2 moveForce = Vector2.ClampMagnitude(new Vector2(xForce, yForce), maxVelocity * hiddenModifier);

        //Add force, reset lasso, and start stopping
        GetComponent<Rigidbody2D>().AddForce(moveForce);
        pbehav.ResetLasso();
        //StartCoroutine(KillForce());
        
    }

    /// <summary>
    /// Stops force on all throwables after a minimum velocity is reached
    /// </summary>
    /// <returns></returns>
    IEnumerator KillForce()
    {
        yield return new WaitForSeconds(2f);
        thrown = false;
    }

    /// <summary>
    /// Sets the parent room for this object
    /// </summary>
    /// <param name="parent">The transform of the room spawned in</param>
    public void SpawnInRoom(Transform parent)
    {
        transform.SetParent(parent);
    }

    #endregion
}
