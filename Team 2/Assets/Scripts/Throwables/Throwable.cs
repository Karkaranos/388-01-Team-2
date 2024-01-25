/*****************************************************************************
// File Name :         Throwable.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates the parent Throwable class

*****************************************************************************/
using UnityEngine;

public class Throwable : MonoBehaviour
{
    #region Variables

    public bool thrown = false;
    public bool pickedUp = false;


    private float damageDealt;
    public ObjectStats obStat;

    [Header("Bouncing:")]
    [SerializeField] private PhysicsMaterial2D notBouncy;
    [SerializeField] private PhysicsMaterial2D bouncy;
    [SerializeField] private int maxBounceCount;
    private int bounceCount;
    public bool isBouncing;

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
        print(type);
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





    #endregion

}
