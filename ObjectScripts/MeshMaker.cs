using System;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{
    public bool debugModeEnabled = false;
    public bool autoUpdate = false;
    public bool addWormhole = true;

    [Range(3, 100)]
    public int numSides = 50; // the numSides in the polygon (e.g. 6 means the cross-section is hexagonal)

    [Min(0.01f)]
    public float length = 20; // the length of the polygonal cylinder

    [Min(0.01f)]
    public float polygonVertexRadius = 1; // the radius of the polygonal cylinder

    [Range(0, 40)]
    public int numSplaySubdivisions = 20; // this controls how well the splay approximates a parabolic curve
    public float totChangeInU = 10; // the size of the splay in the wormhole


    public void MakeMesh()
    {
        Config.debugModeEnabled = debugModeEnabled;
        MakeCylinderMesh();
    }

    public void MakeCylinderMesh()
    {
        if (Config.debugModeEnabled) Debug.Log("GenerateMesh invoked...");


        if (Config.debugModeEnabled) Debug.Log("Getting texture...");
        var texture = GetTexture();

        var meshData = new MeshData();

        if (Config.debugModeEnabled) Debug.Log("Getting mesh...");
        var pc = new PolygonCylinder(numSides, length, polygonVertexRadius);
        pc.AddPolygonCylinderToMesh(meshData);

        if (addWormhole)
        {
            if (Config.debugModeEnabled) Debug.Log("Getting SPLAY data...");
            var splayData = new SplayData(numSplaySubdivisions, totChangeInU);
            if (Config.debugModeEnabled) splayData.SaveToCSV($"{Config.debugFilePath}/splayData.csv");

            if (Config.debugModeEnabled) Debug.Log("Adding SPLAY mesh...");
            var pcs = new PolygonalCylinderSplay(pc.polygon, pc.polygonVertexRadius, splayData);
            pcs.AddPolygonalCylinderSplayToMesh(meshData);
        }

        if (Config.debugModeEnabled) Debug.Log("Drawing mesh...");
        MeshDrawer drawer = FindObjectOfType<MeshDrawer>();
        drawer.DrawMesh(meshData, texture);

        Debug.Log("Mesh generation complete"); // always prints
    }

    private static Texture2D GetTexture()
    {
        Texture2D texture = new Texture2D(1, 1);
        return texture;
    }
}