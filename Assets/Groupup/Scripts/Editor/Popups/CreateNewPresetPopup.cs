#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Groupup
{
    public class CreateNewPresetPopup : EditorWindow
    {
        private string presetName;

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80;

            EditorGUILayout.BeginVertical();

            DrawInfoFields();

            EditorGUILayout.EndVertical();

            // Repaint the window to update its content
            Repaint();
        }

        private void DrawInfoFields()
        {
            EditorGUILayout.Space(30);

            presetName = EditorGUILayout.TextField("Name:", presetName);

            GUILayout.Space(20f);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(presetName));

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                SceneInfoPreset preset = CreateInstance<SceneInfoPreset>();
                preset.presetName = presetName;

                string folderName = "Groupup";
                string[] guids = AssetDatabase.FindAssets("t:Folder " + folderName);

                string folderPath = Path.Combine(AssetDatabase.GUIDToAssetPath(guids[0]), "Resources/ScenePresets",
                    presetName + ".asset");
                AssetDatabase.CreateAsset(preset, folderPath);
                AssetDatabase.SaveAssets();

                ResourceManager.Refresh();
                Close();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif
