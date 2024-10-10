#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Groupup
{
    public class GroupupWindow : EditorWindow
    {
        private enum ToolbarButton
        {
            Scenes,
            Presets,
            Splashs
        }

        private Texture _deleteBtnTexture;
        private Texture _lockBtnTexture;
        private Texture _unLockedBtnTexture;

        private Content _activeContent;

        private ToolbarButton selectedButton = ToolbarButton.Scenes;
        private ToolbarButton lastSelectedButton = ToolbarButton.Scenes;
        
        private Vector2 scrollPosition;

        [MenuItem("Window/GroupUp")]
        public static void ShowWindow()
        {
            GetWindow<GroupupWindow>("GroupUp");
        }

        private void OnEnable()
        {
            _activeContent = new ScenesWindowContent();
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 90;
            DrawToolbar();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            
            if (lastSelectedButton != selectedButton)
            {
                lastSelectedButton = selectedButton;
                switch (lastSelectedButton)
                {
                    case ToolbarButton.Scenes:
                        _activeContent = new ScenesWindowContent();
                        break;
                    case ToolbarButton.Presets:
                        _activeContent = new PresetWindowContent();
                        break;
                    case ToolbarButton.Splashs:
                        DrawButton3Content();
                        break;
                }
            }

            _activeContent?.DrawContent(position);

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.FlexibleSpace();
            GUILayout.Label("", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            selectedButton = (ToolbarButton)GUILayout.Toolbar((int)selectedButton,
                new string[] { "Scene", "Presets", "Splashs" }, EditorStyles.toolbarButton);

            GUILayout.EndHorizontal();
            
        }

        private void DrawButton3Content()
        {
            EditorGUILayout.LabelField("Info List", EditorStyles.boldLabel);
            
        }
    }
}
#endif
