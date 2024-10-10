#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Groupup
{
    public class PresetWindowContent : Content
    {
        private Texture _deleteBtnTexture;
        private Texture _expandMoreBtnTexture;
        private Texture _expandLessBtnTexture;
        private Texture _testtexture;


        public PresetWindowContent()
        {
            _deleteBtnTexture = Resources.Load("Icons/delete") as Texture;
            _expandMoreBtnTexture = Resources.Load("Icons/expand_more") as Texture;
            _expandLessBtnTexture = Resources.Load("Icons/expand_less") as Texture;
            _testtexture = Resources.Load("Icons/add") as Texture;
        }

        public override void DrawContent(Rect position)
        {
            WindowPosition = position;

            GUILayout.Label("InfoPresets", EditorStyles.boldLabel);

            EditorGUILayout.Space(10);

            foreach (SceneInfoPreset infoPreset in ResourceManager.InfoPresets)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                infoPreset.presetName = EditorGUILayout.TextField("Presetname", infoPreset.presetName, GUILayout.MaxWidth(350));
                
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (infoPreset.visible)
                {
                    if (GUILayout.Button(new GUIContent(_expandLessBtnTexture), GUILayout.Width(30)))
                    {
                        infoPreset.visible = !infoPreset.visible;
                    }
                }
                else
                {
                    if (GUILayout.Button(new GUIContent(_expandMoreBtnTexture), GUILayout.Width(30)))
                    {
                        infoPreset.visible = !infoPreset.visible;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(15);

                if (infoPreset.visible)
                {
                    EditorGUI.indentLevel += 6;

                    for (int i = 0; i < infoPreset.scenes.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        infoPreset.scenes[i] =
                            (InterfaceSOBase)EditorGUILayout.ObjectField(infoPreset.scenes[i], typeof(InterfaceSOBase), false);

                        if (GUILayout.Button(new GUIContent(_deleteBtnTexture), GUILayout.Width(30)))
                        {
                            infoPreset.scenes.RemoveAt(i);
                            i--;
                            EditorUtility.SetDirty(infoPreset);
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    GUILayout.Space(10);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+", GUILayout.Width(30)))
                    {
                        infoPreset.scenes.Add(null);
                        EditorUtility.SetDirty(infoPreset);
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(
                            new GUIContent(_deleteBtnTexture,
                                "Delete this service. This is not reversible, so use with care!!!"),
                            GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        CreateDeletePresetPopup(infoPreset);
                        EditorUtility.SetDirty(infoPreset);
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel -= 6;
                }

                GUILayout.Space(10);
                EditorGUILayout.EndVertical();
            }


            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create", GUILayout.Width(200)))
            {
                CreatePresetPopup();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void CreatePresetPopup()
        {
            CreateNewPresetPopup popup = EditorWindow.GetWindow<CreateNewPresetPopup>(true, "Create Preset", true);
            popup.position = new Rect(new Vector2(Screen.height, Screen.width),
                new Vector2(270f, 120f));
            popup.ShowPopup();
        }

        private void CreateDeletePresetPopup(SceneInfoPreset preset)
        {
            SubmitCanclePopup popup = EditorWindow.GetWindow<SubmitCanclePopup>(true, "Delete", true);
            popup.Init("Delete preset - " + preset.name + " -\nThis can not be undone!", "Delete", "Cancel",
                () => DeletePreset(preset), null);
            popup.position = new Rect(new Vector2(Screen.height, Screen.width),
                new Vector2(270f, 120f));
            popup.ShowPopup();
        }

        private void DeletePreset(SceneInfoPreset preset)
        {
            string presetPath = AssetDatabase.GetAssetPath(preset);
            AssetDatabase.DeleteAsset(presetPath);
            AssetDatabase.Refresh();
            ResourceManager.Refresh();
        }
        
    }
}
#endif