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
        //base.DamageDealt = stats.DamageDealt;
        base.obStat = stats;
        bc2D = GetComponent<BoxCollider2D>();
        bc2D.sharedMaterial = base.Bouncy;
    }

    public override void GetThrown(Vector2 arrow)
    {
        if(!hasBounceCap)
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
        }
        else
        {

        }
        base.GetThrown(arrow);
    }

    /// <summary>
    /// Handles collisions with objects
    /// </summary>
    /// <param name="collision">The object collided with</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckBounce(collision.gameObject);
    }
}
