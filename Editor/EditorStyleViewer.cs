using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorStyleViewer : EditorWindow
{
    Vector2 scrollPosition = Vector2.zero;
    string search = "";

    [MenuItem("Tools/GUIStyleLib/EditorStyleViewer")]
    static public void Init()
    {
        EditorWindow.GetWindow(typeof(EditorStyleViewer));
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        {
            GUILayout.Label("Click a Sample to copy its name to your clipboard", "MiniBoldLable");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Search:");
            search = EditorGUILayout.TextField(search);
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        {
            foreach (GUIStyle style in GUI.skin.customStyles)
            {
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                    GUILayout.Space(7);
                    if (GUILayout.Button(style.name, style))
                    {
                        EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(11f);
                }
            }
        }
        GUILayout.EndScrollView();

    }
}
