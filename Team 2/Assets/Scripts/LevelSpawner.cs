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
    private List<PossibleSpawnTempHolder> PossibleSpawns = new List<PossibleSpawnTempHolder>();

    //holds the level that it previously spawned
    public GameObject LastSpawnedLevel;
    
    private LevelBehavior lastBehav;

    private GameObject lastTriedLevel;

    //holds the distance between levels so they spawn seamlessly
    public float xSpawnOffset;
    public float ySpawnOffset;

    //stores the tilemap of the last level spawned
    private Tilemap lastLevelWallsTilemap;

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
                LastSpawnedLevel = Instantiate(PossibleSpawns[Random.Range(0, PossibleSpawns.Count)].level, new Vector3(0, 0, 0), Quaternion.identity);
            }
            
        }

        //goes through hoops to get the last spawned levels' tilemap
        GameObject lastLevelWalls = LastSpawnedLevel.FindChildWithTag("SideWalls");
        lastLevelWallsTilemap = lastLevelWalls.GetComponent<Tilemap>();

        
    }

    public List<PossibleSpawnTempHolder> GetPossibleSpawns()
    {
        List<PossibleSpawnTempHolder> returnList = new List<PossibleSpawnTempHolder>();
        //if the list of possible spawns has anything in it, this clears the list
        if (PossibleSpawns != null)
        {
            PossibleSpawns.Clear();
        }

        //stores a refrence to the LevelBehavior of the last spawned level
        lastBehav = LastSpawnedLevel.GetComponent<LevelBehavior>();

        //adds all possible spawn levels to the possible spawn list
        foreach (GameObject p in LevelPrefabs)
        {
            //stores each prefab in a temp levelbehavior var
            LevelBehavior levelBehav = p.GetComponent<LevelBehavior>();

            //compares the prefab's starting height with the last spawned level's ending height
            foreach (LevelScriptable.EntranceLocations e in levelBehav.entranceLocations)
            {
                foreach (LevelScriptable.EntranceLocations l in lastBehav.entranceLocations)
                {
                    if ((e == LevelScriptable.EntranceLocations.Up && l == LevelScriptable.EntranceLocations.Down) ||
                        (e == LevelScriptable.EntranceLocations.Down && l == LevelScriptable.EntranceLocations.Up) ||
                        (e == LevelScriptable.EntranceLocations.Left && l == LevelScriptable.EntranceLocations.Right) ||
                        (e == LevelScriptable.EntranceLocations.Right && l == LevelScriptable.EntranceLocations.Left))
                    {
                        returnList.Add(new PossibleSpawnTempHolder(p, l));
                    }
                }
            }
        }

        return returnList;
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
            
            do
            {
                if (lastTriedLevel != null)
                {
                    Destroy(lastTriedLevel);
                }
                TryToSpawnLevel();
            }
            while (!CanStillSpawn());
            LastSpawnedLevel = lastTriedLevel;
            lastTriedLevel = null;
            
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

    private bool CanStillSpawn()
    {
        lastBehav.DeleteCommonEntrances();
        if (lastBehav.entranceLocations == null)
        {
            return false;
        }
        return true;
    }

    private void TryToSpawnLevel()
    {
        Debug.Log("level tried");
        lastTriedLevel = LastSpawnedLevel;
        //gets a random level from the list of possible spawns
        PossibleSpawnTempHolder chosenLevel = GetPossibleSpawns()[Random.Range(0, PossibleSpawns.Count)];

        GameObject objToSpawn = chosenLevel.level;


        //sets the spawning offset to the last spawned level's width

        switch (chosenLevel.commonLocation)
        {
            case LevelScriptable.EntranceLocations.Left:
                xSpawnOffset = -(lastLevelWallsTilemap.localBounds.size.x);
                ySpawnOffset = 0;
                break;
            case LevelScriptable.EntranceLocations.Right:
                xSpawnOffset = lastLevelWallsTilemap.localBounds.size.x;
                ySpawnOffset = 0;
                break;
            case LevelScriptable.EntranceLocations.Up:
                ySpawnOffset = lastLevelWallsTilemap.localBounds.size.y;
                xSpawnOffset = 0;
                break;
            case LevelScriptable.EntranceLocations.Down:
                ySpawnOffset = -(lastLevelWallsTilemap.localBounds.size.y);
                xSpawnOffset = 0;
                break;
            default:
                Debug.Log("Something fucked up with the enum for the common location");
                break;
        }


        //instantiates the level to be spawned - sets it to be the last spawned level
        lastTriedLevel = Instantiate(objToSpawn,
            new Vector3(LastSpawnedLevel.transform.position.x + xSpawnOffset,
            LastSpawnedLevel.transform.position.y + ySpawnOffset, LastSpawnedLevel.transform.position.z),
            Quaternion.identity);

        lastBehav = lastTriedLevel.GetComponent<LevelBehavior>();
        lastBehav.InitializeVars();
    }
}
