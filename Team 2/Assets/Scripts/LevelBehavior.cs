//////////////////////////////////////////////////////////////////////////////
/// Author: Tyler Hayes
/// Script: LevelBehavior.cs
/// Date: 10/11/23
/// Summary: This script goes on the levels themselves to provide the spawner
/// with information on the level itself
//////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehavior : MonoBehaviour
{
    //holds a startheight and an endheight
    public LevelScriptable.DoorHeight startHeight;
    public LevelScriptable.DoorHeight endHeight;

    private void Start()
    {

    }
    /// <summary>
    /// called by LevelSpawner.cs when the level is spawned
    /// </summary>
    /// <param name="offset">How much of an offset to have</param>
    public void Offset(float offset)
    {
        //sets the transform to itself plus the xOffset
        transform.position = new Vector3(transform.position.x + offset,
            transform.position.y, transform.position.z);
    }
}
