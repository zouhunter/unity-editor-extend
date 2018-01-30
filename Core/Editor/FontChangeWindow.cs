using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;
namespace EditorTools
{
    public class FontChangeWindow : EditorWindow
    {

        [MenuItem("Tools/Font Replacement")]
        public static void Open()
        {
            FontChangeWindow windows = GetWindow<FontChangeWindow>();
            windows.Show();
        }

        private List<Text> m_Texts = new List<Text>();
        private Font m_Font;
        private ReorderableList m_List;
        private Vector2 listPos;
        private void OnEnable()
        {
            if (m_List == null)
                InitReorderableList();
        }

        void OnGUI()
        {
            m_Font = (Font)EditorGUILayout.ObjectField(m_Font, typeof(Font), false);

            EditorGUI.BeginDisabledGroup(Selection.activeTransform == null);
            if (GUILayout.Button(m_Texts.Count == 0 ? "Find All Text In Child" : "Refresh"))
            {
                m_Texts.Clear();
                var textsInScene = Selection.activeTransform.GetComponentsInChildren<Text>(true);
                if (textsInScene != null && textsInScene.Length != 0)
                    m_Texts.AddRange(textsInScene);
            }
            EditorGUI.EndDisabledGroup();

            using (var ver = new EditorGUILayout.ScrollViewScope(listPos))
            {
                listPos = ver.scrollPosition;
                m_List.DoLayoutList();
            }

            if (GUILayout.Button("Replace Font"))
            {
                if (m_Font != null)
                {
                    foreach (var text in m_Texts)
                    {
                        text.font = m_Font;
                        text.resizeTextForBestFit = false;
                    }
                }
            }
        }

        void InitReorderableList()
        {
            m_List = new ReorderableList(m_Texts, typeof(Text));
            m_List.onSelectCallback += list =>
            {
                var selectText = m_Texts[m_List.index];
                EditorGUIUtility.PingObject(selectText);
            };
        }
    }
}