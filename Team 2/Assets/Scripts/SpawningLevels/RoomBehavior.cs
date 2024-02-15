/*****************************************************************************
// File Name :         RoomBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Handles the behavior of the rooms themselves

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomBehavior : MonoBehaviour
{
    //true if open, false if closed
    [Header("Debug Information:")]
    //0 - up, 1 - down, 2 - right, 3 - left
    public GameObject[] walls;
    public GameObject[] doors;
    public bool[] overallStatus = new bool[4];

    //stores the position of the room
    public Vector2 gridPosition;

    //holds the enemies that the room will spawn
    [SerializeField] private List<EnemySpawnpoint> EnemiesToSpawn;
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    //checks to see if the room has already been visited
    public bool hasBeenVisited;

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

    /// <summary>
    /// Spawns all the objects when the player enters the room
    /// </summary>
    public void SpawnEnemies()
    {
        
        hasBeenVisited = true;
        foreach (EnemySpawnpoint e in EnemiesToSpawn)
        {
            Vector2 spawnPoint = new Vector2(transform.position.x + e.SpawnPoint.x, transform.position.y + e.SpawnPoint.y);
            GameObject go = Instantiate(e.EnemyToSpawn, spawnPoint, Quaternion.identity);
            go.GetComponent<Throwable>().SpawnInRoom(transform);
            
            
        }
        
    }

    
}
