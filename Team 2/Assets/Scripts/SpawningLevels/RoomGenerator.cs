/*****************************************************************************
// File Name :         RoomGenerator.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Randomly Generates rooms in a grid

*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


public class RoomGenerator : MonoBehaviour
{

    [Header("Grid Settings:")]
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private int startPos = 0;
    [SerializeField] private List<Cell> board;
    public static bool GridStyle;

    [Header("Room Settings:")]
    //possible spawns
    [SerializeField] private List<RoomList> rooms;
    //specific spawns
    [SerializeField] private List<FixedRoomSpawns> setSpawns;

    [Header("Debug Information:")]
    public Vector2 offset;
    public Vector2 bottomRightRoom;
    [SerializeField] private CameraBehavior mainCamera;
    private GameObject bottomRightRoomGO;

    private int totalWeights;
    [SerializeField] private List<GameObject> spawnedRooms;
    [SerializeField] private bool hasReachedEnd;
    private Vector2 calcOffset;



    /// <summary>
    /// Sets variables and generates a floor when the scene is started
    /// </summary>
    void Start()
    {
        hasReachedEnd = false;
        bottomRightRoomGO = null;
        calcOffset = offset;
        spawnedRooms = new List<GameObject>();
        foreach (RoomList room in rooms)
        {
            totalWeights += room.weight;
        }
        GenerateFloor();

    }

    /// <summary>
    /// Actually instantiates the rooms
    /// </summary>
    public void InsRooms()
    {
        //these two for loops make it so it instantiates in a 
        //grid pattern
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                //grabs the current cell to make
                Cell currentCell = board[Mathf.FloorToInt(i + j * gridSize.x)];

                //passes over the cell if it's generating in organic style or
                //if the cell hasn't been visited
                if (GridStyle || currentCell.hasBeenVisited)
                {
                    //generates a random room
                    GameObject tempRoom = PickRandomRoom();

                    //checks to see if this position should be a specific room
                    foreach (FixedRoomSpawns r in setSpawns)
                    {
                        if (r.GridPosition.x == i && r.GridPosition.y == j)
                        {
                            tempRoom = r.room;
                        }
                    }
                    if (tempRoom != null)
                    {

                        GameObject newRoom = null;

                        //either makes the new room, or sets it to the bottom right room
                        //once the player reaches the end
                        if (hasReachedEnd && i == 0 && j == 0)
                        {
                            newRoom = bottomRightRoomGO;
                        }
                        else
                        {
                            newRoom = Instantiate(tempRoom, new Vector3(i * calcOffset.x, -j * calcOffset.y, 0), Quaternion.identity, transform);
                        }

                        
                        spawnedRooms.Add(newRoom);
                        RoomBehavior roomBehav = newRoom.GetComponent<RoomBehavior>();

                        //sets the new room's doors
                        roomBehav.overallStatus = currentCell.status;
                        roomBehav.UpdateRooms(currentCell.status);

                        //updates their grid position and name
                        roomBehav.gridPosition = new Vector2(i, j);
                        newRoom.name += " " + i + "-" + j;

                    }
                    else
                    {
                        throw new Exception("Room cannot be null");
                    }
                }
            }
        }

        //sets the new bottom right room after spawning them all
        bottomRightRoomGO = spawnedRooms[spawnedRooms.Count - 1];
        bottomRightRoom = bottomRightRoomGO.GetComponent<RoomBehavior>().gridPosition;
    }

    /// <summary>
    /// selects a random room from a list
    /// </summary>
    /// <returns> a room </returns>
    public GameObject PickRandomRoom()
    {
        //adds up all the weights so it picks randomly including the weights
        int rand = UnityEngine.Random.Range(0, totalWeights + 1);
        foreach (RoomList room in rooms)
        {
            rand -= room.weight;
            if (rand <= 0)
            {
                return room.room;
            }
        }
        return null;
    }

    /// <summary>
    /// makes the path the player follows
    /// </summary>
    public void GenerateFloor()
    {
        //adds a cell for each room in the grid
        board = new List<Cell>();
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                board.Add(new Cell());
            }
        }

        //sets up some vars to use later
        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        //1000 means the the ai will keep going through the path until it 
        //either moves 1000 times or finishes generating
        while (k < 1000)
        {
            k++;
            board[currentCell].hasBeenVisited = true;

            //leaves the loop if the devs want it to generate organically and the ai has
            //reached the bottom right room
            if (!GridStyle && currentCell == board.Count - 1)
            {
                break;
            }

            //now we check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            //checks to see if it has any neighbors that havent been visited yet
            if (neighbors.Count == 0)
            {
                //if the path is also back at the start, it breaks the loop
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    //otherwise it turns around
                    currentCell = path.Pop();
                }
            }
            else
            {
                //adds the new cell to the stack
                path.Push(currentCell);
                
                //picks a random neighbor to go to
                int newCell = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];

                //opens the path between the new cell and the last one
                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }

        //after it generates where the paths are, it instantiates the rooms
        InsRooms();
    }

    /// <summary>
    /// checks to see if the given cell has any neighbors it can go to
    /// </summary>
    /// <param name="cell"> the current cell </param>
    /// <returns> a list of neighbors that havent been visited </returns>
    public List<int> CheckNeighbors(int cell)
    {
        List<int> returnList = new List<int>();

        //check up neighbor
        if (cell - gridSize.x >= 0 && !board[Mathf.FloorToInt(cell - gridSize.x)].hasBeenVisited)
        {
            returnList.Add(Mathf.FloorToInt(cell - gridSize.x));
        }
        //check down neighbor
        if (cell + gridSize.x < board.Count && !board[Mathf.FloorToInt(cell + gridSize.x)].hasBeenVisited)
        {
            returnList.Add(Mathf.FloorToInt(cell + gridSize.x));
        }

        //check right neighbor
        if ((cell + 1) % gridSize.x != 0 && !board[Mathf.FloorToInt(cell + 1)].hasBeenVisited)
        {
            returnList.Add(Mathf.FloorToInt(cell + 1));
        }

        //check left neighbor
        if (cell % gridSize.x != 0 && !board[Mathf.FloorToInt(cell - 1)].hasBeenVisited)
        {
            returnList.Add(Mathf.FloorToInt(cell - 1));
        }

        return returnList;
    }

    /// <summary>
    /// deletes and respawns each room when the player gets to the end
    /// </summary>
    public void RespawnRooms()
    {
        //deletes all the rooms other than the one the player is in
        foreach (GameObject go in spawnedRooms)
        {

            if (hasReachedEnd && go == bottomRightRoomGO)
            {
                //moves the player's room to the top left room
                mainCamera.UpdateLocation(new Vector2(0, 0));
                go.transform.position = new Vector2(0, 0);
                GameObject.FindGameObjectWithTag("Player").transform.parent = null;

            }
            else
            {
                Destroy(go);
            }

        }

        //clears the list and re adds the room the players in
        spawnedRooms.Clear();
        spawnedRooms.Add(bottomRightRoomGO);
        board.Clear();

        //makes a new floor
        GenerateFloor();
    }

    /// <summary>
    /// triggers when the player has reached the end
    /// kickstarts respawning the rooms
    /// </summary>
    public void ReachedTheEnd()
    {
        hasReachedEnd = true;
        RespawnRooms();
    }
}
