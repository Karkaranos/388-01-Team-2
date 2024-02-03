using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCheckBehavior : MonoBehaviour
{
    [HideInInspector] public bool isOverlapping = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isOverlapping = true;
        print("trigger entered: invalid path");
        GetComponent<SpriteRenderer>().color = Color.gray;
        //Destroy(gameObject);
    }
}
