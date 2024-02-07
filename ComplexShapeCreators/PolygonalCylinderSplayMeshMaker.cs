using UnityEngine;
using System.IO;
using UnityEngine.UIElements;


public class PolygonalCylinderSplay
{
    private Polygon polygon;
    private SplayData splayData;
    private float baseVertexRadius;

    public PolygonalCylinderSplay(Polygon polygon, float baseVertexRadius, SplayData splayData)
    {
        this.polygon = polygon; // the data 
        this.splayData = splayData; // the pts that define the curve that begins parallel to the cylinder's sides and ends parallel to the plane
        this.baseVertexRadius = baseVertexRadius; // the radius of the cylinder before any splaying
    }

    /* 
        This function will form the splay at the end of the cylinder, starting at z=0 and splaying into z>0.
        The splay will be formed by stacking polygons of increasing radii as z grows larger, and the 
        specific radii and z coords chosen will come from the "splayData", which calculates the radii
        and z coords so that the splay is parabolic.
    */
    public void AddPolygonalCylinderSplayToMesh(MeshData meshData)
    {
        var prevVertexRadius = baseVertexRadius; // initialize the radius of the "previous" polygon to be the radius of the cylinder.
        var prevZ = 0f; // the original cylinder goes from z=-length to z=0, so the splay will start at z=0 and splay into z>0

        float nextVertexRadius = -1f; // nextVertexRadius will generally be larger than prevVertexRadius as we stack larger and larger polygons at higher z-coords in the splay
        var totSplayLength = splayData.GetTotalChangeInY(); // the splay length in the z direction
        for (int splayLevel = 1; splayLevel <= this.splayData.numDivisions; splayLevel++)
        {
            var splayPt = splayData.xyChanges[splayLevel]; // get the amount of change in x and y (actually, x and z here) at this splay pt in the curved splay

            // the splay pts contain the total x and y changes from the end of cylinder to the end of the "splayLevel"
            // therefore, the radius at the end of this splayLevel is equal to splayPt.x plus the radius at the end of the cylinder (baseVertexRadius)
            nextVertexRadius = baseVertexRadius + splayPt.x;
            // and the nextZ coord is splayPt.y plus the z coordinate at the end of the cylinder (which is 0)
            var nextZ = splayPt.y;

            // generate the vertices and triangles for the mesh between these two polygons of different radii and different z-coords
            PolygonCylinder.StackPolygons(meshData, polygon, totSplayLength, prevVertexRadius, nextVertexRadius, prevZ, nextZ);

            // stack the next polygon upon the polygon just stacked
            prevVertexRadius = nextVertexRadius;
            prevZ = nextZ;
        }

        // "stack" one more polygon at the same z coord as the last one in order to form the "plane".
        int planeSizeFactor = 10;
        PolygonCylinder.StackPolygons(meshData, polygon, totSplayLength, nextVertexRadius, nextVertexRadius * planeSizeFactor, prevZ, prevZ);

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


