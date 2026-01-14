using Code.Gameplay.ResourceSystem;
using Code.Gameplay.TerrainGeneration.Debug;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Resource))]
public class ResourceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Resource t = (Resource)target;
        if (GUILayout.Button("Hit"))
            t.GetHit();
        if (GUILayout.Button("Destroy"))
            t.BeDestroyed();
    }
}