using UnityEngine;
using System.Collections.Generic;

public class MeshData
{
    public List<Vector3> vertices;
    public List<int> triangleIdxs;
    public List<Vector2> uvs;

    private int lastVertexIdx = -1;

    public MeshData()
    {
        vertices = new List<Vector3>();
        triangleIdxs = new List<int>();
        uvs = new List<Vector2>();
        lastVertexIdx = -1;
    }

    public int AddVertex(Vector3 v, Vector2 uv)
    {
        vertices.Add(v);
        uvs.Add(uv);

        lastVertexIdx++;
        return lastVertexIdx;
    }

    public void AddTriangleIdxs(int i1, int i2, int i3)
    {
        triangleIdxs.Add(i1);
        triangleIdxs.Add(i2);
        triangleIdxs.Add(i3);
    }

    public Triangle[] Triangles
    {
        get
        {
            int triangleCount = triangleIdxs.Count / 3;
            Triangle[] triangleArray = new Triangle[triangleCount];
            for (int i = 0; i < triangleCount; i++)
            {
                int idx = i * 3;
                triangleArray[i] = new Triangle(vertices[triangleIdxs[idx]], vertices[triangleIdxs[idx + 1]], vertices[triangleIdxs[idx + 2]]);
            }
            return triangleArray;
        }
    }

    public string TrianglesToString()
    {
        string result = "";
        foreach (Triangle t in Triangles)
        {
            result += t.ToString() + "\n";
        }
        return result;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangleIdxs.ToArray(),
            uv = uvs.ToArray()
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}

public static class DemoMeshData
{
    public static MeshData GetMeshDataForQuad()
    {
        MeshData meshData = new MeshData();

        var v = new Vector3(0, 0, 0);
        var uv = new Vector2(0, 0);
        var vIdx1 = meshData.AddVertex(v, uv);

        v = new Vector3(1, 1, 0);
        uv = new Vector2(1, 1);
        var vIdx2 = meshData.AddVertex(v, uv);

        v = new Vector3(1, 0, 0);
        uv = new Vector2(1, 0);
        var vIdx3 = meshData.AddVertex(v, uv);

        meshData.AddTriangleIdxs(vIdx1, vIdx2, vIdx3); //CLOCKWISE

        v = new Vector3(0, 0, 0);
        uv = new Vector2(0, 0);
        var vIdx4 = meshData.AddVertex(v, uv);

        v = new Vector3(0, 1, 0);
        uv = new Vector2(0, 1);
        var vIdx5 = meshData.AddVertex(v, uv);

        v = new Vector3(1, 1, 0);
        uv = new Vector2(1, 1);
        var vIdx6 = meshData.AddVertex(v, uv);

        meshData.AddTriangleIdxs(vIdx4, vIdx5, vIdx6); //CLOCKWISE

        return meshData;
    }
}