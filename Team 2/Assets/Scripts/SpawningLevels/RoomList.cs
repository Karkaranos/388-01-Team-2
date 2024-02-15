/*****************************************************************************
// File Name :         RoomList.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Holds a room to spawn and it's associated weight

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomList
{
    public GameObject room;
    public int weight;

    public RoomList(GameObject room, int weight)
    {
        this.room = room;
        this.weight = weight;
    }
}
