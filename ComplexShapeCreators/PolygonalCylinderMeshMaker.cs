using UnityEngine;

public class PolygonCylinder
{
    public int numSides;
    public float length;
    public float polygonVertexRadius;

    public Polygon polygon;

    public PolygonCylinder(int numSides, float length, float polygonVertexRadius)
    {
        this.numSides = numSides;
        this.length = length;
        this.polygonVertexRadius = polygonVertexRadius;
    }

    public void AddPolygonCylinderToMesh(MeshData meshData)
    {
        polygon = new Polygon(numSides);

        StackPolygons(meshData, polygon, length, polygonVertexRadius, polygonVertexRadius, -length, 0);

        if (Config.debugModeEnabled) PrintDebugInfo(meshData);
    }

    public static void StackPolygons(MeshData meshData, Polygon polygon, float totLength, float vertexRadius1, float vertexRadius2, float z1, float z2)
    {
        Vector2[] poly1Vs = polygon.GetVertices(vertexRadius1);
        Vector2[] poly2Vs = polygon.GetVertices(vertexRadius2);

        float[] angularUvs = polygon.angularUvs;

        for (int i1 = 0; i1 < polygon.numSides; i1++)
        {
            int i2 = (i1 + 1) % polygon.numSides;

            float angularUv1 = angularUvs[i1];
            float angularUv2 = angularUvs[i2];

            float zUv1 = z1 / totLength;
            float zUv2 = z2 / totLength;

            int idx1 = meshData.AddVertex(new Vector3(poly1Vs[i2].x, poly1Vs[i2].y, z1), new Vector2(angularUv2, zUv1));
            int idx2 = meshData.AddVertex(new Vector3(poly1Vs[i1].x, poly1Vs[i1].y, z1), new Vector2(angularUv1, zUv1));
            int idx3 = meshData.AddVertex(new Vector3(poly2Vs[i1].x, poly2Vs[i1].y, z2), new Vector2(angularUv1, zUv2));
            meshData.AddTriangleIdxs(idx1, idx2, idx3);

            idx1 = meshData.AddVertex(new Vector3(poly2Vs[i2].x, poly2Vs[i2].y, z2), new Vector2(angularUv2, zUv2));
            idx2 = meshData.AddVertex(new Vector3(poly1Vs[i2].x, poly1Vs[i2].y, z1), new Vector2(angularUv2, zUv1));
            idx3 = meshData.AddVertex(new Vector3(poly2Vs[i1].x, poly2Vs[i1].y, z2), new Vector2(angularUv1, zUv2));
            meshData.AddTriangleIdxs(idx1, idx2, idx3);
        }
    }

    private void PrintDebugInfo(MeshData meshData)
    {
        Debug.Log("Triangles used:\n" + meshData.TrianglesToString());
        Debug.Log($"NumVertices={meshData.vertices.Count}, NumTriangleIdxs={meshData.triangleIdxs.Count}, NumTriangles={meshData.Triangles.Length}");
    }
}

