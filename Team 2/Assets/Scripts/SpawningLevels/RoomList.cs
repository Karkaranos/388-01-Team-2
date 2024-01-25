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
