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

        // 실선 그리기
        DrawLine();

        GUILayout.Label("JSON Files List", EditorStyles.boldLabel);

        Draw();

        DrawLine();
        // 파일 개수 출력
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
                LoadJsonFiles(); // 파일 목록을 다시 로드하여 삭제된 모든 파일을 목록에서 제거합니다.
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
        // Application.persistentDataPath 폴더를 엽니다.
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
                // 파일 사이즈 계산 및 표시 로직은 동일하게 유지
                double fileSizeBytes = fileInfo.Length;
                string fileSize;
                if (fileSizeBytes < 1024 * 1024) // 1MB 미만이면 KB 단위로 표시
                {
                    fileSize = (fileSizeBytes / 1024).ToString("F2") + " KB";
                }
                else // 그렇지 않으면 MB 단위로 표시
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
                            LoadJsonFiles(); // 파일 목록을 다시 로드하여 삭제된 파일을 목록에서 제거합니다.
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                UnityEngine.Debug.LogWarning($"File not found: {ex.Message}");
                LoadJsonFiles(); // 파일 목록을 다시 로드하여 없어진 파일을 목록에서 제거합니다.
            }
        }




        GUILayout.EndScrollView();
    }

}
