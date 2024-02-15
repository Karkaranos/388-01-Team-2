/*****************************************************************************
// File Name :         Cell.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Holds each room's data to spawn them efficiently

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool hasBeenVisited = false;

    //these are to see which pathways the rooms have open and closed
    public bool[] status = new bool[4];
}
