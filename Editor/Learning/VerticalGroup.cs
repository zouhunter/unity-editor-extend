using UnityEngine;
using UnityEditor;

// Create a Vertical Compound Button 
class VerticalScopeExample : EditorWindow
{

    [MenuItem("Learning/Scope/Vertical scope usage")]
    static void Init()
    {
        var window = GetWindow<VerticalScopeExample>();
        window.Show();
    }

    void OnGUI()
    {
        using (var v = new EditorGUILayout.VerticalScope("Button"))
        {
            if (GUI.Button(v.rect, GUIContent.none))
                Debug.Log("Go here");
            GUILayout.Label("I'm inside the button");
            GUILayout.Label("So am I");
        }
    }
}