using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class OpenInfoFileWindow
{
    [MenuItem("Tools/Open Info File Window")]
    private static void OpenWindow()
    {
        InfoFileWindow.ShowWindow();
    }
}

public class InfoFileWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string[] jsonFiles;

    public static void ShowWindow()
    {
        var window = GetWindow<InfoFileWindow>("Info File Window");
        window.minSize = new Vector2(300, 200);
        window.LoadJsonFiles();
    }

    private void OnGUI()
    {
        GUILayout.Label("Ver. 1.1.0", EditorStyles.boldLabel);
        GUILayout.Label("JSON Files in Persistent Data Path", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Folder"))
        {
            OpenFolder();
        }

        if (GUILayout.Button("Refresh JSON Files List"))
        {
            LoadJsonFiles();
        }

        GUILayout.Space(5);

        // �Ǽ� �׸���
        DrawLine();

        GUILayout.Label("JSON Files List", EditorStyles.boldLabel);

        Draw();

        DrawLine();
        // ���� ���� ���
        if (jsonFiles != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Total Files: {jsonFiles.Length}", EditorStyles.boldLabel);
            DeleteAllFiles();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("Total Files: 0", EditorStyles.boldLabel);
        }
    }

    private void DeleteAllFiles()
    {
        if (GUILayout.Button("Delete All", GUILayout.Width(70)))
        {
            if (EditorUtility.DisplayDialog("Confirm Deletion of All Files",
                "Are you sure you want to delete all files?", "Yes", "No"))
            {
                foreach (string file in jsonFiles)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                AssetDatabase.Refresh();
                LoadJsonFiles(); // ���� ����� �ٽ� �ε��Ͽ� ������ ��� ������ ��Ͽ��� �����մϴ�.
            }
        }
    }

    private void DrawLine()
    {
        var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, Color.gray);
    }

    private void OpenFolder()
    {
        // Application.persistentDataPath ������ ���ϴ�.
        Process.Start(Application.persistentDataPath);
    }

    private void LoadJsonFiles()
    {
        jsonFiles = Directory.GetFiles(Application.persistentDataPath, "*.json");
    }

    private void Draw()
    {
        if (jsonFiles == null || jsonFiles.Length == 0)
        {
            GUILayout.Label("No JSON files found.");
            return;
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);


        foreach (string file in jsonFiles)
        {
            try
            {
                var fileInfo = new FileInfo(file);
                // ���� ������ ��� �� ǥ�� ������ �����ϰ� ����
                double fileSizeBytes = fileInfo.Length;
                string fileSize;
                if (fileSizeBytes < 1024 * 1024) // 1MB �̸��̸� KB ������ ǥ��
                {
                    fileSize = (fileSizeBytes / 1024).ToString("F2") + " KB";
                }
                else // �׷��� ������ MB ������ ǥ��
                {
                    fileSize = (fileSizeBytes / 1024 / 1024).ToString("F2") + " MB";
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(fileInfo.Name + " (" + fileSize + ")", GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm File Deletion",
     $"Are you sure you want to delete the {fileInfo.Name} file?", "Yes", "No"))
                        {
                            File.Delete(file);
                            AssetDatabase.Refresh();
                            LoadJsonFiles(); // ���� ����� �ٽ� �ε��Ͽ� ������ ������ ��Ͽ��� �����մϴ�.
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                UnityEngine.Debug.LogWarning($"File not found: {ex.Message}");
                LoadJsonFiles(); // ���� ����� �ٽ� �ε��Ͽ� ������ ������ ��Ͽ��� �����մϴ�.
            }
        }




        GUILayout.EndScrollView();
    }

}
