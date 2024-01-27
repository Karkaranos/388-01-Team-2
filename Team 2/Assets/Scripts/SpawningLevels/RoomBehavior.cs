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
    [SerializeField] private List<EnemySpawnpoint> EnemiesToSpawn;
    public List<GameObject> spawnedEnemies = new List<GameObject>();

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

    public void SpawnEnemies()
    {
        hasBeenVisited = true;
        foreach (EnemySpawnpoint e in EnemiesToSpawn)
        {
            Vector2 spawnPoint = new Vector2(transform.position.x + e.SpawnPoint.x, transform.position.y + e.SpawnPoint.y);
            GameObject go = Instantiate(e.EnemyToSpawn, spawnPoint, Quaternion.identity);
            spawnedEnemies.Add(go);
            go.GetComponent<EnemyBehavior>().roomSpawnedIn = this;
            go.GetComponent<EnemyBehavior>().SpawnInRoom(gameObject.transform);
        }
    }

    /*public void EnemyDied(GameObject go)
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
           
        }
    }*/
}
