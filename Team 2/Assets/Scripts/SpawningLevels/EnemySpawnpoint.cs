using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnpoint
{
    public GameObject EnemyToSpawn;
    public Vector2 SpawnPoint;

    public EnemySpawnpoint(GameObject enemyToSpawn, Vector2 spawnPoint)
    {
        EnemyToSpawn = enemyToSpawn;
        SpawnPoint = spawnPoint;
    }
}
