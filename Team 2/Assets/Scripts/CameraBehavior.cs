/*****************************************************************************
// File Name :         CameraBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Fixes the camera to each room

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private RoomGenerator generator;
    private Vector2 posOffset;

    /// <summary>
    /// sets the room offset
    /// </summary>
    private void Start()
    {
        posOffset = generator.offset;
    }

    /// <summary>
    /// sends the camera to the grid position
    /// </summary>
    /// <param name="gridPos"> what position to go to </param>
    public void UpdateLocation(Vector2 gridPos)
    {
        transform.position = new Vector3(gridPos.x * posOffset.x, -gridPos.y * posOffset.y, -10);
    }
}
