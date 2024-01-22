//////////////////////////////////////////////////////////////////////////////
/// Author: Tyler Hayes
/// Script: LevelSpawner.cs
/// Date: 10/11/23
/// Summary: This script handles procedural generation of levels
//////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelSpawner : MonoBehaviour
{
    //holds all of the possible levels to spawn - All levels get placed in the inspector
    [SerializeField] private List<GameObject> LevelPrefabs;

    //Temporary list - holds the levels that can spawn off another, taking into account doorheight
    private List<GameObject> PossibleSpawns = new List<GameObject>();

    //holds the level that it previously spawned
    public GameObject LastSpawnedLevel;

    //holds the distance between levels so they spawn seamlessly
    public float xSpawnOffset;

    //stores the tilemap of the last level spawned
    private Tilemap lastLevelGroundTilemap;

    //holds a list of all currently spawned levels
    [SerializeField] private List<GameObject> currentSpawnedRooms = new List<GameObject>();

    //holds the max number of spawned rooms - if the spawner
    //spawns a room and the number of spawned rooms go over,
    //it will delete the oldest room
    [SerializeField] int maxNumOfSpawnedRooms;

    //stores a refrence to a player
    public GameObject player;

    //this is a refrence to a starting room - doesnt delete this room
    [SerializeField] private GameObject hubRoom;

    void Awake()
    {
        //instantializes variables before start
        InitializeVars();
    }

    /// <summary>
    /// Initializes all variables needed
    /// </summary>
    private void InitializeVars()
    {
        //if it has no last spawned level, it stores a refrence to the hubroom
        if (LastSpawnedLevel == null)
        {
            if (hubRoom != null)
            {
                LastSpawnedLevel = hubRoom;
            }
            else
            {
                LastSpawnedLevel = Instantiate(PossibleSpawns[Random.Range(0, PossibleSpawns.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            }
            
        }

        //goes through hoops to get the last spawned levels' tilemap
        GameObject lastLevelGroundGrid = LastSpawnedLevel.FindChildWithTag("GroundGrid");
        GameObject lastLevelGround = lastLevelGroundGrid.FindChildWithTag("Ground");
        lastLevelGroundTilemap = lastLevelGround.FindComponentInChildWithTag<Tilemap>("Ground");
    }

    /// <summary>
    /// Spawns new level yay
    /// </summary>
    /// <param name="numberOfTimes">Repeats the number of times the level is supposed to spawn</param>
    public void SpawnNewLevel(int numberOfTimes)
    {
        //repeats the number of times the game will spawn
        for (int i = 0; i < numberOfTimes; i++)
        {
            //if the list of possible spawns has anything in it, this clears the list
            if (PossibleSpawns != null)
            {
                PossibleSpawns.Clear();
            }

            //stores a refrence to the LevelBehavior of the last spawned level
            LevelBehavior lastBehav = LastSpawnedLevel.GetComponent<LevelBehavior>();

            //adds all possible spawn levels to the possible spawn list
            foreach (GameObject p in LevelPrefabs)
            {
                //stores each prefab in a temp levelbehavior var
                LevelBehavior levelBehav = p.GetComponent<LevelBehavior>();

                //compares the prefab's starting height with the last spawned level's ending height
                if (levelBehav.startHeight == lastBehav.endHeight)
                {
                    //if they match, adds the prefab to the list of possible spawns
                    PossibleSpawns.Add(p);
                }
            }

            //gets a random level from the list of possible spawns
            GameObject objToSpawn = PossibleSpawns[Random.Range(0, PossibleSpawns.Count)];

            //sets the spawning offset to the last spawned level's width
            xSpawnOffset = lastLevelGroundTilemap.localBounds.size.x / 2;

            //instantiates the level to be spawned - sets it to be the last spawned level
            LastSpawnedLevel = Instantiate(objToSpawn,
                new Vector3(LastSpawnedLevel.transform.position.x + xSpawnOffset,
                LastSpawnedLevel.transform.position.y, LastSpawnedLevel.transform.position.z),
                Quaternion.identity);

            //gets a refrence to the newly spawned level's levelbehavior
            LevelBehavior newBehav = LastSpawnedLevel.GetComponent<LevelBehavior>();

            //gets a refrence to the ground tilemap of the new spawned level
            GameObject toSpawnGroundGrid = LastSpawnedLevel.FindChildWithTag("GroundGrid");
            GameObject actualSpawnGround = toSpawnGroundGrid.FindChildWithTag("Ground");
            lastLevelGroundTilemap = actualSpawnGround.GetComponent<Tilemap>();

            //adds the newly spawned level's offset
            newBehav.Offset(lastLevelGroundTilemap.localBounds.size.x / 2);

            //adds the current spawned room to the list of spawned rooms
            currentSpawnedRooms.Add(LastSpawnedLevel);

            //runs the function to delete the farthest room
            DeleteFarthestRoom();
        }

    }

    /// <summary>
    /// If the number of spawned rooms is too many, deletes the farthest room
    /// </summary>
    private void DeleteFarthestRoom()
    {
        //checks if the number of spawned rooms is more than the max
        if (currentSpawnedRooms.Count > maxNumOfSpawnedRooms)
        {
            //deletes the oldest room
            Destroy(currentSpawnedRooms[0]);
            currentSpawnedRooms.RemoveAt(0);
        }
    }
}
