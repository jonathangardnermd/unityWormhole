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

    /*
    This function will create the sides of the cylinder by "stacking" two polygons: 
    one at z=-length and the other at z=0.
    */
    public void AddPolygonCylinderToMesh(MeshData meshData)
    {
        polygon = new Polygon(numSides); // calculate the vertices of a unit regular polygon with the specified number of sides

        // "stacking" the polygons creates the triangles and vertices to connect them, forming the sides of the cylinder
        StackPolygons(meshData, polygon, length, polygonVertexRadius, polygonVertexRadius, -length, 0);

        if (Config.debugModeEnabled) PrintDebugInfo(meshData);
    }

    public static void StackPolygons(MeshData meshData, Polygon polygon, float totLength, float vertexRadius1, float vertexRadius2, float z1, float z2)
    {
        // calculate the vertices of the two polygons with the two (potentially different) radii
        Vector2[] poly1Vs = polygon.GetVertices(vertexRadius1);
        Vector2[] poly2Vs = polygon.GetVertices(vertexRadius2);

        float[] angularUvs = polygon.angularUvs;

        for (int i1 = 0; i1 < polygon.numSides; i1++)
        {
            // add the vertices and triangles that form the quad between poly1's i1 and i2 vertices and poly2's i1 and i2 vertices
            int i2 = (i1 + 1) % polygon.numSides;

            // set the uv data in the 2 dimensions (angular and z directions) to stretch the texture correctly
            float angularUv1 = angularUvs[i1];
            float angularUv2 = angularUvs[i2];
            float zUv1 = z1 / totLength;
            float zUv2 = z2 / totLength;

            // add the first triangle of the quad
            int idx1 = meshData.AddVertex(new Vector3(poly1Vs[i2].x, poly1Vs[i2].y, z1), new Vector2(angularUv2, zUv1));
            int idx2 = meshData.AddVertex(new Vector3(poly1Vs[i1].x, poly1Vs[i1].y, z1), new Vector2(angularUv1, zUv1));
            int idx3 = meshData.AddVertex(new Vector3(poly2Vs[i1].x, poly2Vs[i1].y, z2), new Vector2(angularUv1, zUv2));
            meshData.AddTriangleIdxs(idx1, idx2, idx3);

            // add the second triangle of the quad
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

