using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptable", menuName = "LevelScriptable")]
public class LevelScriptable : ScriptableObject
{
    public enum EntranceLocations
    {
        Up,
        Down,
        Left,
        Right
    }

    public List<EntranceLocations> entranceLocations;

    public LevelScriptable(List<EntranceLocations> entranceLocations)
    {
        this.entranceLocations = entranceLocations;
    }
}
