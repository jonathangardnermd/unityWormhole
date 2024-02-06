using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshMaker))]
public class EditorForMeshMaker : Editor
{
    public override void OnInspectorGUI()
    {
        MeshMaker maker = (MeshMaker)target;

        if (DrawDefaultInspector())
        {
            if (maker.autoUpdate)
            {
                maker.MakeMesh();
            }
        }

        if (GUILayout.Button("Generate Mesh"))
        {
            maker.MakeMesh();
        }
    }
}