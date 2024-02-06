using UnityEngine;
using System.IO;


public class PolygonalCylinderSplay
{
    private Polygon polygon;
    private SplayData splayData;
    private float baseVertexRadius;

    public PolygonalCylinderSplay(Polygon polygon, float baseVertexRadius, SplayData splayData)
    {
        this.polygon = polygon;
        this.splayData = splayData;
        this.baseVertexRadius = baseVertexRadius;
    }

    public void AddPolygonalCylinderSplayToMesh(MeshData meshData)
    {
        var baseVertexRadius = this.baseVertexRadius;
        var prevVertexRadius = baseVertexRadius;
        var prevZ = 0f;

        float nextVertexRadius = 0f;
        var totSplayLength = splayData.GetTotalChangeInY();
        for (int splayLevel = 1; splayLevel <= this.splayData.numDivisions; splayLevel++)
        {
            var sd = this.splayData.xyChanges[splayLevel];
            nextVertexRadius = baseVertexRadius + sd.x;

            var nextZ = sd.y;
            PolygonCylinder.StackPolygons(meshData, polygon, totSplayLength, prevVertexRadius, nextVertexRadius, prevZ, nextZ);

            prevVertexRadius = nextVertexRadius;
            prevZ = nextZ;
        }
        PolygonCylinder.StackPolygons(meshData, polygon, totSplayLength, nextVertexRadius, nextVertexRadius * 100, prevZ, prevZ);

        if (Config.debugModeEnabled) PrintDebugInfo(meshData);
    }

    private void PrintDebugInfo(MeshData meshData)
    {
        var triangleStr = meshData.TrianglesToString();
        Debug.Log("After splay: Triangles used:\n" + triangleStr);
        SaveToCSV(triangleStr, $"{Config.debugFilePath}/triangles.txt");
        Debug.Log($"After splay: NumVertices={meshData.vertices.Count}, NumTriangleIdxs={meshData.triangleIdxs.Count}, NumTriangles={meshData.Triangles.Length}");
    }

    private void SaveToCSV(string triangleStr, string filePath)
    {
        System.Text.StringBuilder csvContent = new System.Text.StringBuilder();
        csvContent.AppendLine(triangleStr);
        File.WriteAllText(filePath, csvContent.ToString());
    }
}


