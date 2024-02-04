using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class NodeInfo : MonoBehaviour
{
    public NodeLocations location;
    public float F;
    public float G;
    public float H;
    public NodeInfo parent;

    public NodeInfo(NodeLocations l, float g, float f, float h, NodeInfo par)
    {
        location = l;
        G = g;
        F = f;
        H = h;
        parent = par;
    }

    public override bool Equals(object obj)
    {
        if(obj !=null || this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        return location.Equals(((NodeInfo) obj).location);
    }

    public override int GetHashCode()
    {
        return 0;
    }

}

public class NodeLocations: MonoBehaviour
{
    public Vector3 location;
    public float x;
    public float y;
    public GameObject a;

    public NodeLocations(float _x, float _y)
    {
        x = _x;
        y = _y;
    }

    public Vector3 ToVector()
    {
        return new Vector3(x, y, 0);
    }

    public static NodeLocations operator +(NodeLocations a, NodeLocations b)
   => new NodeLocations(a.x + b.x, a.y + b.y);

    public override string ToString()
    {
        return "X: " + x + " , Y: " + y;
    }

    public override bool Equals(object other)
    {
        if(((NodeLocations) other).x == x && ((NodeLocations) other).y==y)
        {
            return true;
        }
        return false;
    }
}
