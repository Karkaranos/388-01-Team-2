/*****************************************************************************
// File Name :         EnemySpawnpoint.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Holds a throwable object and where it spawns

*****************************************************************************/

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
