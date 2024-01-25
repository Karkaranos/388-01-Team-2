using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    [Header("Debug Information:")]
    //0 - up, 1 - down, 2 - right, 3 - left
     public GameObject[] walls;
     public GameObject[] doors;
     public Vector2 gridPosition;

    /// <summary>
    /// closes and opens rooms
    /// </summary>
    /// <param name="status">whether or not a door is in each direction</param>
   public void UpdateRooms(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
