using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCheckBehavior : MonoBehaviour
{
    public bool isOverlapping = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Throwable")
        {
            isOverlapping = true;
            GetComponent<SpriteRenderer>().color = Color.green;
            //Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            isOverlapping = true;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            isOverlapping = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Throwable")
        {
            isOverlapping = true;
            GetComponent<SpriteRenderer>().color = Color.green;
            //Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            isOverlapping = true;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            isOverlapping = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }
}
