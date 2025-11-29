using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class CodeCollectorEditor : EditorWindow
{
    private string folderPath = "RitualGrounds/Scripts";
    private string filePattern = "*.cs";
    private string outputFileName = "CollectedCode.txt";
    private bool includeSubfolders = true;

    [MenuItem("Tools/Собрать код")]
    public static void ShowWindow()
    {
        GetWindow<CodeCollectorEditor>("Code Collector");
    }

    private void OnGUI()
    {
        GUILayout.Label("Настройки сбора кода", EditorStyles.boldLabel);
        
        folderPath = EditorGUILayout.TextField("Путь к папке:", folderPath);
        filePattern = EditorGUILayout.TextField("Расширение:", filePattern);
        outputFileName = EditorGUILayout.TextField("Выходной файл:", outputFileName);
        includeSubfolders = EditorGUILayout.Toggle("Включить подпапки:", includeSubfolders);

        if (GUILayout.Button("Собрать код"))
        {
            CollectCode();
        }
    }

    private void CollectCode()
    {
        string fullFolderPath = Path.Combine(Application.dataPath, folderPath);
        
        if (!Directory.Exists(fullFolderPath))
        {
            EditorUtility.DisplayDialog("Ошибка", $"Папка не найдена: {fullFolderPath}", "OK");
            return;
        }

        SearchOption searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] files = Directory.GetFiles(fullFolderPath, filePattern, searchOption);

        if (files.Length == 0)
        {
            EditorUtility.DisplayDialog("Предупреждение", $"Файлы не найдены", "OK");
            return;
        }

        StringBuilder combinedCode = new StringBuilder();
        combinedCode.AppendLine($"// Собрано файлов: {files.Length}");
        combinedCode.AppendLine($"// Дата: {System.DateTime.Now}");
        combinedCode.AppendLine(new string('=', 80));
        combinedCode.AppendLine();

        foreach (string filePath in files)
        {
            string relativePath = filePath.Replace(Application.dataPath, "Assets");
            combinedCode.AppendLine($"// ========== {relativePath} ==========");
            combinedCode.AppendLine();
            combinedCode.AppendLine(File.ReadAllText(filePath, Encoding.UTF8));
            combinedCode.AppendLine();
            combinedCode.AppendLine(new string('-', 80));
            combinedCode.AppendLine();
        }

        string outputPath = Path.Combine(Application.dataPath, outputFileName);
        File.WriteAllText(outputPath, combinedCode.ToString(), Encoding.UTF8);
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Успех", $"Создан файл: Assets/{outputFileName}\nСобрано файлов: {files.Length}", "OK");
    }
}
