using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private int startPos = 0;
    [SerializeField] private GameObject room;
    [SerializeField] private Vector2 offset;
    [SerializeField] private List<Cell> board;
    [SerializeField] private bool GridStyle;

    // Start is called before the first frame update
    void Start()
    {
        GenerateFloor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InsRooms()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Cell currentCell = board[Mathf.FloorToInt(i + j * gridSize.x)];

                if (GridStyle || currentCell.hasBeenVisited) {

                    GameObject newRoom = Instantiate(room, new Vector3(i * offset.x, -j * offset.y, 0), Quaternion.identity, transform);
                    RoomBehavior roomBehav = newRoom.GetComponent<RoomBehavior>();
                    roomBehav.UpdateRooms(currentCell.status);

                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
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
                int newCell = neighbors[Random.Range(0, neighbors.Count)];
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

}
