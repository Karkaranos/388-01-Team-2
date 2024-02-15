/*****************************************************************************
// File Name :         FixedRoomSpawns.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Stores a room and it's grid position

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FixedRoomSpawns
{
    public GameObject room;
    public Vector2 GridPosition;

    public FixedRoomSpawns(GameObject room, Vector2 gridPosition)
    {
        this.room = room;
        GridPosition = gridPosition;
    }
}
