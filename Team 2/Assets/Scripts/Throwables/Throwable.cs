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
    private int bounceCount;
    public bool isBouncing;

    [SerializeField]
    private float timeUntilStopping;
    private float stopTime;

    public List<GameObject> bouncedWith = new List<GameObject>();

    public float DamageDealt { get => damageDealt; set => damageDealt = value; }
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
            //print("bounced with new object");
            isBouncing = true;
            bouncedWith.Add(obj);
            return bouncy;
        }
        else
        {
            //print("Hit previously bounced with");
            isBouncing = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return notBouncy;
        }
    }

    public void GetThrown(Vector2 arrow)
    {
        thrown = true;
        PlayerBehavior pbehav = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        float angle = Mathf.Atan2(arrow.y, arrow.x) * Mathf.Rad2Deg;
        float xForce = Mathf.Cos(angle * Mathf.PI / 180) * forceModifier * hiddenModifier;
        float yForce = Mathf.Sin(angle * Mathf.PI / 180) * forceModifier * hiddenModifier;
        Vector2 moveForce = Vector2.ClampMagnitude(new Vector2(xForce, yForce), maxVelocity * hiddenModifier);

        GetComponent<Rigidbody2D>().AddForce(moveForce);
        pbehav.ResetLasso();
        StartCoroutine(KillForce());
        
    }

    IEnumerator KillForce()
    {
        stopTime = timeUntilStopping;
        while(GetComponent<Rigidbody2D>().velocity.magnitude > 50)
        {
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity * .8f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //thrown = false;
    }

    public void SpawnInRoom(Transform parent)
    {
        transform.SetParent(parent);
    }
}





    #endregion
