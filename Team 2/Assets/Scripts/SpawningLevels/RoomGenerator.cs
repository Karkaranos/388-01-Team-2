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
    [SerializeField] private List<RoomList> rooms;
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



    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {

    }

    public void InsRooms()
    {
        int count = 0;
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                count++;
                Cell currentCell = board[Mathf.FloorToInt(i + j * gridSize.x)];

                if (GridStyle || currentCell.hasBeenVisited)
                {


                    GameObject tempRoom = PickRandomRoom();

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

                        /*if (hasReachedEnd && i == 0 && j == 0)
                        {
                            if (newDoor < 1)
                            {
                                currentCell.status[0] = true;
                            }
                            else
                            {
                                currentCell.status[3] = true;
                            }
                        }*/
                        roomBehav.overallStatus = currentCell.status;
                        roomBehav.UpdateRooms(currentCell.status);
                        roomBehav.gridPosition = new Vector2(i, j);
                        newRoom.name += " " + i + "-" + j;

                    }
                    else
                    {
                        Debug.Log("Random Room is Null");
                    }
                }
            }
        }
        bottomRightRoomGO = spawnedRooms[spawnedRooms.Count - 1];
        bottomRightRoom = bottomRightRoomGO.GetComponent<RoomBehavior>().gridPosition;
    }

    public GameObject PickRandomRoom()
    {
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

    public void GenerateFloor()
    {
        board = new List<Cell>();
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;


        while (k < 1000)
        {
            k++;
            board[currentCell].hasBeenVisited = true;

            if (!GridStyle && currentCell == board.Count - 1)
            {
                break;
            }

            //now we check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);
                int newCell = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
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
        InsRooms();
    }

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


    public void RespawnRooms()
    {

        foreach (GameObject go in spawnedRooms)
        {

            if (hasReachedEnd && go == bottomRightRoomGO)
            {
                mainCamera.UpdateLocation(new Vector2(0, 0));
                go.transform.position = new Vector2(0, 0);
                GameObject.FindGameObjectWithTag("Player").transform.parent = null;

            }
            else
            {
                Destroy(go);
            }

        }
        spawnedRooms.Clear();
        spawnedRooms.Add(bottomRightRoomGO);
        board.Clear();
        GenerateFloor();
    }

    public void ReachedTheEnd()
    {
        hasReachedEnd = true;
        RespawnRooms();

    }
}
