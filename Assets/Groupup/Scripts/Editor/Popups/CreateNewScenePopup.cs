#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Groupup
{
    public class CreateNewScenePopup : EditorWindow
    {
        private string sceneName;
        private string authorName;
        private string email;
        private string notes;
        private string path;

        private Vector2 scrollPosition;


        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawInfoFields();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // Repaint the window to update its content
            Repaint();
        }

        private void DrawInfoFields()
        {
            GUILayout.Label("Create Info", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            sceneName = EditorGUILayout.TextField("Name:", sceneName);
            authorName = EditorGUILayout.TextField("Author:", authorName);
            email = EditorGUILayout.TextField("Email:", email);

            GUILayout.Label("Notes:");
            notes = EditorGUILayout.TextArea(notes, GUILayout.Height(200f));

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Path:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            GUILayout.TextField(path);

            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string newPath = EditorUtility.OpenFolderPanel("Select Root Folder", "", "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    path = FileUtil.GetProjectRelativePath(newPath);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(path));
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create", GUILayout.Width(200)))
            {
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.Refresh();
                }

                CreateScene();
                Close();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
        }

        private void CreateScene()
        {
            SceneCreater.CreateScene(sceneName, authorName, email, notes, path);
        }
    }
}
#endif