#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Groupup
{
    public class ScenesWindowContent : Content
    {
        private Texture _deleteBtnTexture;

        public ScenesWindowContent()
        {
            _deleteBtnTexture = Resources.Load("Icons/delete") as Texture;
        }

        public override void DrawContent(Rect position)
        {
            WindowPosition = position;

            GUILayout.Label("Scenes", EditorStyles.boldLabel);

            foreach (InterfaceSOBase info in ResourceManager.Interfaces)
            {
                if (!info || !info.RootFolder)
                    continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("<b>Name:</b> " + info.SceneName,
                    new GUIStyle(EditorStyles.label) { richText = true });
                EditorGUILayout.LabelField("<b>Author:</b> " + info.Author,
                    new GUIStyle(EditorStyles.label) { richText = true });
                EditorGUILayout.LabelField("<b>Creation Date:</b> " + info.CreationDate,
                    new GUIStyle(EditorStyles.label) { richText = true });
                EditorGUILayout.LabelField("<b>Contact Email:</b> " + info.ContactEmail,
                    new GUIStyle(EditorStyles.label) { richText = true });
                GUI.enabled = false;
                EditorGUILayout.LabelField("<b>Notes:</b> " + info.Notes,
                    new GUIStyle(EditorStyles.label) { richText = true });
                GUI.enabled = true;
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_deleteBtnTexture, GUILayout.Width(30)))
                {
                    CreateDeleteScenePopup(info);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create", GUILayout.Width(200)))
            {
                CreateInfoPopup();
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void CreateInfoPopup()
        {
            CreateNewScenePopup popup = EditorWindow.GetWindow<CreateNewScenePopup>(true, "Create Scene", true);
            popup.position =
                new Rect(new Vector2(Screen.height, Screen.width),
                    new Vector2(400f, 400f));
            popup.ShowPopup();
        }

        private void CreateDeleteScenePopup(InterfaceSOBase info)
        {
            SubmitCanclePopup popup = EditorWindow.GetWindow<SubmitCanclePopup>(true, "Delete", true);
            popup.Init("Delete scene - " + info.SceneName + " -\nThis can not be undone!", "Delete", "Cancel",
                () => DeleteScene(info), null);
            popup.position = new Rect(new Vector2(Screen.height, Screen.width),
                new Vector2(270f, 120f));
            popup.ShowPopup();
        }
        
        private void DeleteScene(InterfaceSOBase info)
        {
            RemoveSceneFromBuild(info.name);
            string rootFolderPath = AssetDatabase.GetAssetPath(info.RootFolder);
            AssetDatabase.DeleteAsset(rootFolderPath);
            AssetDatabase.Refresh();
        }

        private void RemoveSceneFromBuild(string scenePath)
        {
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            // Überprüfe, ob die Szene im Build enthalten ist
            for (int i = 0; i < buildScenes.Length; i++)
            {
                if (buildScenes[i].path == scenePath)
                {
                    // Entferne die Szene aus dem Build
                    EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[buildScenes.Length - 1];
                    for (int j = 0; j < i; j++)
                    {
                        newBuildScenes[j] = buildScenes[j];
                    }

                    for (int j = i + 1; j < buildScenes.Length; j++)
                    {
                        newBuildScenes[j - 1] = buildScenes[j];
                    }

                    EditorBuildSettings.scenes = newBuildScenes;

                    Debug.Log(scenePath + " has been removed from the build settings.");
                    return;
                }
            }

            Debug.Log(scenePath + " is not in the build settings.");
        }
    }
}
#endif