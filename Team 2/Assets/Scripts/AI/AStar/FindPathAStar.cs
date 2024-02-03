using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindPathAStar : MonoBehaviour
{
    [SerializeField] private LayerMask pathfindUsingMe;

    List<NodeInfo> openNodes = new List<NodeInfo>();
    List<NodeInfo> closedNodes = new List<NodeInfo>();

    [SerializeField] private GameObject target;
    private Transform start;
    public GameObject pathP;

    NodeInfo goalNode;
    NodeInfo startNode;
    NodeInfo lastNode;
    public List<NodeLocations> locations;

    [SerializeField] GameObject roomSpawnedIn;

    bool done = false;

    bool pathingStarted;
    [SerializeField] GameObject testSpot;

    void FindNewPath()
    {
        done = false;

        locations= new List<NodeLocations>();
        for(int x = (int)roomSpawnedIn.transform.position.x - 8; x < (int)roomSpawnedIn.transform.position.x + 9; x++)
        {
            for (int y = (int)roomSpawnedIn.transform.position.y - 4; y < (int)roomSpawnedIn.transform.position.y + 4; y++)
            {
                Vector3 spawnPos = new Vector3(x, y, 0);
                GameObject testSpawn = Instantiate(testSpot, spawnPos, Quaternion.identity);
                if(!testSpawn.GetComponent<SpawnCheckBehavior>().isOverlapping)
                {
                    NodeLocations temp = new NodeLocations(x, y);
                    locations.Add(temp);
                    print(temp);
                    
                }
                Destroy(testSpawn);
            }
        }

        goalNode = new NodeInfo(new NodeLocations(target.transform.position.x, target.transform.position.z), 0, 0, 0, null);



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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
