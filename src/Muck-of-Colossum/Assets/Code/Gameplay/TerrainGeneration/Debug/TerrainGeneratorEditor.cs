using UnityEditor;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Debug
{
    [CustomEditor(typeof(TerrainGeneratorDebug))]
    public class TerrainGeneratorEditor : Editor
    {
        private bool showMesh;
        private bool showNoise;
        private bool showTexture;
        private bool showHeight;

        public override void OnInspectorGUI()
        {
            TerrainGeneratorDebug t = (TerrainGeneratorDebug)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();

            DrawSOSection("Mesh Settings", t.meshSettings, ref showMesh);
            DrawSOSection("Noise Settings", t.noiseSetting, ref showNoise);
            DrawSOSection("Texture Settings", t.textureSettings, ref showTexture);
            DrawSOSection("Height Map Settings", t.heightMapSettings, ref showHeight);

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
                t.Generate();
        }

        private void DrawSOSection(string title, ScriptableObject so, ref bool foldout)
        {
            if (so == null)
            {
                EditorGUILayout.HelpBox($"{title} is NULL", MessageType.Info);
                return;
            }

            foldout = EditorGUILayout.Foldout(foldout, title, true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                Editor editor = CreateEditor(so);
                editor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }
        }
    }
}