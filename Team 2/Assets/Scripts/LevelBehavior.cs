//////////////////////////////////////////////////////////////////////////////
/// Author: Tyler Hayes
/// Script: LevelBehavior.cs
/// Date: 10/11/23
/// Summary: This script goes on the levels themselves to provide the spawner
/// with information on the level itself
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBehavior : MonoBehaviour
{
    //holds a startheight and an endheight
    public List<LevelScriptable.EntranceLocations> entranceLocations;
    public List<LevelScriptable.EntranceLocations> locationsToRemove;

    private Tilemap WallsTilemap;
    private int layerMask = 6;
    private RaycastHit2D raycastHit;

    private void Start()
    {
        InitializeVars();
       
    }
    
    public void InitializeVars()
    {
        GameObject Walls = gameObject.FindChildWithTag("SideWalls");
        WallsTilemap = Walls.GetComponent<Tilemap>();
    }

    public void DeleteCommonEntrances()
    {
        
        for (int i = entranceLocations.Count - 1; i >= 0; i--)
        {
            LevelScriptable.EntranceLocations entrance = entranceLocations[i];
            switch (entrance)
            {
                case LevelScriptable.EntranceLocations.Up:
                    raycastHit = Physics2D.Raycast(gameObject.transform.position, Vector2.up, WallsTilemap.localBounds.size.y / 2 + 10);
                    if (raycastHit.collider != null)
                    {
                        entranceLocations.Remove(entrance);
                    }
                    break;
                case LevelScriptable.EntranceLocations.Down:
                    raycastHit = Physics2D.Raycast(gameObject.transform.position, Vector2.down, WallsTilemap.localBounds.size.y / 2 + 10);
                    if (raycastHit.collider != null)
                    {
                        entranceLocations.Remove(entrance);
                    }
                    break;
                case LevelScriptable.EntranceLocations.Left:
                    raycastHit = Physics2D.Raycast(gameObject.transform.position, Vector2.left, WallsTilemap.localBounds.size.x / 2 + 10);
                    if (raycastHit.collider != null)
                    {
                        entranceLocations.Remove(entrance);
                    }
                    break;
                case LevelScriptable.EntranceLocations.Right:
                    raycastHit = Physics2D.Raycast(gameObject.transform.position, Vector2.right, WallsTilemap.localBounds.size.x / 2 + 10);
                    if (raycastHit.collider != null)
                    {
                        entranceLocations.Remove(entrance);
                    }
                    break;
            }
        }
        

    }


    public void CloseEntrances()
    {

    }

}
