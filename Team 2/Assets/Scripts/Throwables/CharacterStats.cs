/*****************************************************************************
// File Name :         CharacterStats.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates a serializable class that stores and adjusts 
                        stats for a character. Inherits from ObjectStats

*****************************************************************************/
using UnityEngine;

[System.Serializable]
public class CharacterStats : ObjectStats
{
    #region Variables
    [Header("Basic Stats:")]
    [SerializeField]
    private float _health;
    [SerializeField]
    private float _maxHealth;
    [SerializeField]
    private float _speed;
    private float _originalSpeed;


    [Header("Effects:")]
    [SerializeField]
    private float _freezeTime;
    [SerializeField]
    private float _invincTime;




    public float Health { get => _health; set => _health = value; }
    public float Speed { get => _speed; set => _speed = value; }
    //public float DamageDealt { get => _baseDmgDealt; }
    public float FreezeTime { get => _freezeTime; }
    public float InvincibilityTime { get => _invincTime; set => _invincTime = value; }

    #endregion

    #region Functions
    /// <summary>
    /// Reduces the health the current character has and returns the result
    /// </summary>
    /// <param name="damage">The amount of health to reduce by</param>
    /// <returns>The new health</returns>
    public float TakeDamage(float damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
        }

        return Health;
    }

    /// <summary>
    /// Increases the health the current character has and returns the result
    /// </summary>
    /// <param name="amountHealed">The amount of health to recover</param>
    /// <param name="healthBuffed">Returns true if character has health buff</param>
    /// <param name="buffAmount">If buffed, the health multiplier</param>
    /// <returns>The new health</returns>
    public float Heal(float amountHealed, bool healthBuffed, float buffAmount)
    {
        if(buffAmount < 1 || !healthBuffed)
        {
            buffAmount = 1;
        }

        Health *= (1 + amountHealed);

        if(Health > _maxHealth * buffAmount)
        {
            Health = _maxHealth * buffAmount;
        }

        return Health;
    }

    #endregion


}
