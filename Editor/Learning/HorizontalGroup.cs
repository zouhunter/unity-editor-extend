using UnityEngine;
using UnityEditor;

// Create a Horizontal Compound Button 
class HorizontalScopeExample : EditorWindow
{

    [MenuItem("Learning/Scope/Horizontal scope usage")]
    static void Init()
    {
        var window = GetWindow<HorizontalScopeExample>();
        window.Show();
    }
    void OnGUI()
    {
        using (var h = new EditorGUILayout.HorizontalScope("Button"))
        {
            if (GUI.Button(h.rect, GUIContent.none))
                Debug.Log("Go here");
            GUILayout.Label("I'm inside the button");
            GUILayout.Label("So am I");
        }
    }
}