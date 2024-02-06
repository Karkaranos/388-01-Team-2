using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private RoomGenerator generator;
    private Vector2 posOffset;
    private void Start()
    {
        posOffset = generator.offset;
    }
    public void UpdateLocation(Vector2 gridPos)
    {
        transform.position = new Vector3(gridPos.x * posOffset.x, -gridPos.y * posOffset.y, -10);
    }
}
