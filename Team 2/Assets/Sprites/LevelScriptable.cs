using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptable", menuName = "LevelScriptable")]
public class LevelScriptable : ScriptableObject
{
    public enum DoorHeight
    {
        High,
        Low,
        Medium
    }

    public DoorHeight startHeight;
    public DoorHeight endHeight;
    public LevelScriptable(DoorHeight startHeight, DoorHeight endHeight)
    {
        this.startHeight = startHeight;
        this.endHeight = endHeight;

    }
}
