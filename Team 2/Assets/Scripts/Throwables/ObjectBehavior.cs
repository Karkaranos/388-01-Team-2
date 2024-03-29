/*****************************************************************************
// File Name :         EnemyBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates the base class for Objects. Inherits from Throwable.

*****************************************************************************/
using UnityEngine;

public class ObjectBehavior : Throwable
{
    [SerializeField]
    private ObjectStats stats;
    public BoxCollider2D bc2D;
    [SerializeField]
    private bool hasBounceCap;

    public ObjectStats Stats { get => stats; set => stats = value; }

    /// <summary>
    /// Start is called on the first frame update. It gets a reference to the player
    /// and sets damage dealt
    /// </summary>
    private void Start()
    {
        base.obStat = stats;
        bc2D = GetComponent<BoxCollider2D>();
        bc2D.sharedMaterial = base.Bouncy;
    }


    /// <summary>
    /// Handles collisions with objects
    /// </summary>
    /// <param name="collision">The object collided with</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager am = FindObjectOfType<AudioManager>();
        if(am!=null)
        {
            am.PlayBounce();
        }
        CheckBounce(collision.gameObject);
    }
}
