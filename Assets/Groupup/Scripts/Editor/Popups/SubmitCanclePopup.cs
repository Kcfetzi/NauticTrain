#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Groupup
{
    public class SubmitCanclePopup : EditorWindow
    {
        private UnityAction _submitAction;
        private UnityAction _cancelAction;

        private string _title;
        private string _submitBtnText;
        private string _cancelBtnText;


        public void Init(string title, string submitBtnText, string cancelBtnText, UnityAction submitAction,
            UnityAction cancelAction)
        {
            _title = title;
            _submitBtnText = submitBtnText;
            _cancelBtnText = cancelBtnText;
            _submitAction = submitAction;
            _cancelAction = cancelAction;

        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80;

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_title, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_submitBtnText, GUILayout.Width(80)))
            {
                _submitAction?.Invoke();
                Close();
            }

            if (GUILayout.Button(_cancelBtnText, GUILayout.Width(80)))
            {
                _cancelAction?.Invoke();
                Close();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Repaint the window to update its content
            Repaint();
        }
    }
}
#endif
