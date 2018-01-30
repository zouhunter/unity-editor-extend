using UnityEngine;
using UnityEditor;

// Simple Editor Window that creates a scroll view with a Label inside
class BeginEndScrollView : EditorWindow
{
    Vector2 scrollPos;
    string t = "This is a string inside a Scroll view!";
    [MenuItem("Learning/Scope/Write text on ScrollView")]
    static void Init()
    {
        var window = GetWindow<BeginEndScrollView>();
        window.Show();
    }

    void OnGUI()
    {
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Width(100), GUILayout.Height(100)))
            {
                scrollPos = scrollView.scrollPosition;
                GUILayout.Label(t);
            }
            if (GUILayout.Button("Add More Text", GUILayout.Width(100), GUILayout.Height(100)))
                t += " \nAnd this is more text!";
        }
        if (GUILayout.Button("Clear")) t = "";
    }
}