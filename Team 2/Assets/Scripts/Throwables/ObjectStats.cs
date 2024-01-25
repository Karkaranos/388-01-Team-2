/*****************************************************************************
// File Name :         ObjectStats.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates a serializable class that stores and adjusts 
                        stats for objects

*****************************************************************************/
using UnityEngine;

[System.Serializable]
public class ObjectStats 
{
    [Header("Damage:")]
    [SerializeField]
    private float _baseDmgDealt;
    [SerializeField]
    private float _dmgCap;
    [SerializeField]
    private float _dmgToPlayer;
    [SerializeField]
    private float _bounceDamage;
    [SerializeField]
    private float _dmgToEnemy;
    public float BaseDamageDealt { get => _baseDmgDealt; }
    public float DMGToPlayer { get => _dmgToPlayer; }
    public float DMGToEnemy { get => _dmgToEnemy; }
    public float BounceDMG { get => _bounceDamage; }

    public enum DamageTypes
    {
        TO_PLAYER, TO_ENEMY, ON_BOUNCE, FROM_WALL
    }

}
