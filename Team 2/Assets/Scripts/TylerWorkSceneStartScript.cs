using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TylerWorkSceneStartScript : MonoBehaviour
{
    [SerializeField] private GameObject levelSpawner;
    private LevelSpawner lsBehav;
    [SerializeField] public int roomsToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        lsBehav = levelSpawner.GetComponent<LevelSpawner>();
        lsBehav.SpawnNewLevel(roomsToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
