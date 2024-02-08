using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindPathAStar : MonoBehaviour
{
    [SerializeField] private LayerMask pathfindUsingMe;

    [SerializeField] List<NodeInfo> openNodes = new List<NodeInfo>();
    [SerializeField] List<NodeInfo> closedNodes = new List<NodeInfo>();

    [SerializeField] private GameObject target;
    private Transform start;
    //public GameObject pathP;

    NodeInfo goalNode;
    NodeInfo startNode;
    NodeInfo lastNode;
    public List<NodeLocations> locations = new List<NodeLocations>();

    [SerializeField] GameObject roomSpawnedIn;

    bool done = false;

    bool pathingStarted;
    [SerializeField] GameObject testSpot;

    //For whatever reason it will only move in whatever direction is top here
    private List<NodeLocations> directions = new List<NodeLocations>(){
        new NodeLocations(-.5f,0),
        new NodeLocations(0,.5f),
        new NodeLocations(.5f,0),
        new NodeLocations(0,-.5f),
    };

    void FindNewPath()
    {
        done = false;

        print("new path started");

        locations.Clear();
        for(float x = (int)roomSpawnedIn.transform.position.x - 8; x < (int)roomSpawnedIn.transform.position.x + 9; x+=.5f)
        {
            for (float y = (int)roomSpawnedIn.transform.position.y - 4; y < (int)roomSpawnedIn.transform.position.y + 4; y+=.5f)
            {
                Vector3 spawnPos = new Vector3(x, y, 0);
                GameObject testSpawn = Instantiate(testSpot, spawnPos, Quaternion.identity);
                if(!testSpawn.GetComponent<SpawnCheckBehavior>().isOverlapping)
                {
                    NodeLocations temp = new NodeLocations(x, y);
                    locations.Add(temp);
                    //print(temp);
                    
                }
                //Destroy(testSpawn);
            }
        }

        goalNode = new NodeInfo(new NodeLocations(target.transform.position.x, target.transform.position.z), 0, 0, 0, null);

        print("Goal: " + goalNode.location.ToString());
        startNode = new NodeInfo(new NodeLocations(transform.position.x, transform.position.z), 0, 0, 0, null);

        openNodes.Clear();
        closedNodes.Clear();

        openNodes.Add(startNode);
        print("Open nodes: " + openNodes.Count);
        lastNode = startNode;

        StartCoroutine(SearchTimer());
    }

    IEnumerator SearchTimer()
    {

        for (; ; )
        {
            if(!done)
            {                
                transform.position = lastNode.location.ToVector();
                Search(lastNode);
                yield return new WaitForSeconds(2f);

            }
        }
    }

    private void Search(NodeInfo thisNode)
    {
        if(thisNode.Equals(goalNode))
        {
            done = true;
            return;
        }

        for(int i=0; i<4; i++)
        {
            NodeLocations neighbor = directions[i] + thisNode.location;
            //print(neighbor);

            if(/*IsValid(neighbor) &&*/ !IsClosed(neighbor))
            {
                

                float G = Vector3.Distance(thisNode.location.ToVector(), neighbor.ToVector()) + thisNode.G;
                float H = Vector3.Distance(neighbor.ToVector(), goalNode.location.ToVector());
                float F = G + H;

                //print("Neighbor values: F: " + F.ToString("0.00") + " G: " + H.ToString("0.00") + " H: " + H.ToString("0.00"));


                //WHY THE FUCK DOES THIS ONLY RUN 4X
                if(!UpdateMarker(neighbor, G, H, F, thisNode))
                {
                    print("added open node");
                    openNodes.Add(new NodeInfo(neighbor, G, F, H, thisNode));
                }
                else
                {
                    print("dupe open node");
                }
            }
            else
            {
                print("match not found in one direction");
            }
        }

        print("Open nodes: " + openNodes.Count);

        if (openNodes.Count > 0)
        {
            openNodes = openNodes.OrderBy(p => p.F).ThenBy(n => n.H).ToList<NodeInfo>();
            NodeInfo ni = (NodeInfo)openNodes.ElementAt(0);
            closedNodes.Add(ni);
            openNodes.RemoveAt(0);

            lastNode = ni;
        }
        else
        {
            print("no movement should be occuring");
        }

    }

    bool UpdateMarker(NodeLocations pos, float g, float h, float f, NodeInfo ni)
    {
        foreach(NodeInfo n in openNodes)
        {
            //print("ran update test comparing " +  n.location.ToString() + " and " + pos.ToString());

            if (n.location.Equals(pos))
            {
                n.G = g;
                n.H = h;
                n.F = f;
                n.parent = ni;
                print("should not run");
                return true;
            }

        }
        return false;
    }

    private bool IsClosed(NodeLocations spot)
    {
        foreach(NodeInfo n in closedNodes)
        {
            if(n.location = spot)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsValid(NodeLocations spot)
    {
        foreach (NodeLocations n in locations)
        {
            if (n == spot)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name.Equals("Spawn"))
        {
            roomSpawnedIn = collision.transform.GetComponentInParent<RoomBehavior>().gameObject;
            if(!pathingStarted)
            {
                pathingStarted = true;
                //FindNewPath();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerBehavior>().gameObject;
        FindNewPath();
        //locations = new List<NodeLocations>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
