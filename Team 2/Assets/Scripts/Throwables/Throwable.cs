/*****************************************************************************
// File Name :         Throwable.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates the parent Throwable class

*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;

public class Throwable : MonoBehaviour
{
    #region Variables

    public bool thrown = false;
    public bool pickedUp = false;


    private float damageDealt;
    public ObjectStats obStat;

    [Header("Bouncing:")]
    [SerializeField] protected PhysicsMaterial2D notBouncy;
    [SerializeField] protected PhysicsMaterial2D bouncy;
    [SerializeField] private int maxBounceCount;
    private int bounceCount;
    public bool isBouncing;

    public List<GameObject> bouncedWith = new List<GameObject>();

    public float DamageDealt { get => damageDealt; set => damageDealt = value; }
    public PhysicsMaterial2D Bouncy { get => bouncy; }

    public PhysicsMaterial2D BounceCount()
    {
        if(!isBouncing)
        {
            //bounceCount++;
            //isBouncing = true;
            print("off");
            return bouncy;
        }
        else
        {
            //bounceCount = 0;
           // isBouncing = false;
            return notBouncy;
        }
    }

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
}





    #endregion
