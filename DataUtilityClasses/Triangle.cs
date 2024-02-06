// using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector3 vertex1;
    public Vector3 vertex2;
    public Vector3 vertex3;

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        vertex1 = v1;
        vertex2 = v2;
        vertex3 = v3;
    }

    public override string ToString()
    {
        return string.Format("{0,-40}{1,-40}{2,-40}", vertex1, vertex2, vertex3);
    }
}
