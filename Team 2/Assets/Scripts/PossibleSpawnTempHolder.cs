using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleSpawnTempHolder
{
    public GameObject level;
    public LevelScriptable.EntranceLocations commonLocation;

    public PossibleSpawnTempHolder(GameObject level, LevelScriptable.EntranceLocations commonLocation)
    {
        this.level = level;
        this.commonLocation = commonLocation;
    }
}
