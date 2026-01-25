using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BatchThumbnailExporter : MonoBehaviour
{
    [SerializeField] private string inputPath = "Assets/Prefabs";
    [SerializeField] private string outputPath = "Assets/Icons";
    [SerializeField] private int iconSize = 128;
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0);

    private void Start()
    {
        GenerateIcons();
    }

    private void GenerateIcons()
    {
        if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { inputPath });

        if (guids.Length == 0) return;

        RuntimePreviewGenerator.BackgroundColor = backgroundColor;
        RuntimePreviewGenerator.PreviewDirection = new Vector3(-1f,-0.58f,-0.58f);
        
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                EditorUtility.DisplayProgressBar("Generating Icons", $"Processing {prefab.name}", (float)i / guids.Length);

                Texture2D texture = RuntimePreviewGenerator.GenerateModelPreview(prefab.transform, iconSize, iconSize, false);
                
                if (texture != null)
                {
                    SaveTexture(texture, prefab.name);
                    DestroyImmediate(texture);
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("Done!");
    }

    private void SaveTexture(Texture2D tex, string name)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(
            tex.width, 
            tex.height, 
            0, 
            RenderTextureFormat.Default, 
            RenderTextureReadWrite.Linear);

        Graphics.Blit(tex, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;
        Texture2D readableText = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        readableText.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableText.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        byte[] bytes = readableText.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(outputPath, name + ".png"), bytes);
        
        DestroyImmediate(readableText);
    }
}