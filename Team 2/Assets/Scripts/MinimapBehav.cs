using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBehav : MonoBehaviour
{
    [SerializeField] private RoomGenerator rG;
    // Start is called before the first frame update
    void Start()
    {
        UpdateMinimap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMinimap()
    {
        transform.position = new Vector3(rG.gridSize.x / 2 * rG.offset.x - 8.5f,
            rG.gridSize.y / 2 * -rG.offset.y + 4.5f, -20);
    }
}
