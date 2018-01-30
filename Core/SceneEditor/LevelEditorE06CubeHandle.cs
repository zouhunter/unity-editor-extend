using UnityEngine;
using UnityEditor;
using System.Collections;
//1.建立Level层
//2.保存场景为GameE06 可以看到效果
// [InitializeOnLoad]可以确保这个类的构造器在编辑器加载时就被调用
[InitializeOnLoad]
public class LevelEditorE06CubeHandle : Editor
{
    public static Vector3 CurrentHandlePosition = Vector3.zero;
    public static bool IsMouseInValidArea = false;

    static Vector3 m_OldHandlePosition = Vector3.zero;

    static LevelEditorE06CubeHandle()
    {
        //The OnSceneGUI delegate is called every time the SceneView is redrawn and allows you
        //to draw GUI elements into the SceneView to create in editor functionality
        // OnSceneGUI委托在Scene视图每次被重绘时被调用
        // 这允许我们可以在Scene视图绘制自定义的GUI元素
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (IsInCorrectLevel() == false)
        {
            return;
        }

        bool isLevelEditorEnabled = EditorPrefs.GetBool("IsLevelEditorEnabled", true);

        //Ignore this. I am using this because when the scene GameE06 is opened we haven't yet defined any On/Off buttons
        //for the cube handles. That comes later in E07. This way we are forcing the cube handles state to On in this scene
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name == "GameE06")
            {
                isLevelEditorEnabled = true;
            }
        }

        if (isLevelEditorEnabled == false)
        {
            return;
        }

        // 更新Handle的位置
        UpdateHandlePosition();
        // 检查鼠标所在的位置是否有效
        UpdateIsMouseInValidArea(sceneView.position);
        // 检测是否需要重新绘制Handle
        UpdateRepaint();

        DrawCubeDrawPreview();
    }

    //I will use this type of function in many different classes. Basically this is useful to 
    //be able to draw different types of the editor only when you are in the correct scene so we
    //can have an easy to follow progression of the editor while hoping between the different scenes
    static bool IsInCorrectLevel()
    {
        return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name == "GameE06";
    }

    static void UpdateIsMouseInValidArea(Rect sceneViewRect)
    {
        // 确保cube handle只在需要的区域内绘制
        // 在本例我们就是当鼠标移动到自定义的GUI上或更低的位置上时，就简单地隐藏掉handle
        bool isInValidArea = Event.current.mousePosition.y < sceneViewRect.height - 35;

        if (isInValidArea != IsMouseInValidArea)
        {
            IsMouseInValidArea = isInValidArea;
            SceneView.RepaintAll();
        }
    }

    static void UpdateHandlePosition()
    {
        if (Event.current == null)
        {
            return;
        }

        Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Level")) == true)
        {
            Vector3 offset = Vector3.zero;

            if (EditorPrefs.GetBool("SelectBlockNextToMousePosition", true) == true)
            {
                offset = hit.normal;
            }

            CurrentHandlePosition.x = Mathf.Floor(hit.point.x - hit.normal.x * 0.001f + offset.x);
            CurrentHandlePosition.y = Mathf.Floor(hit.point.y - hit.normal.y * 0.001f + offset.y);
            CurrentHandlePosition.z = Mathf.Floor(hit.point.z - hit.normal.z * 0.001f + offset.z);

            CurrentHandlePosition += new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    static void UpdateRepaint()
    {
        //If the cube handle position has changed, repaint the scene
        if (CurrentHandlePosition != m_OldHandlePosition)
        {
            SceneView.RepaintAll();
            m_OldHandlePosition = CurrentHandlePosition;
        }
    }

    static void DrawCubeDrawPreview()
    {
        if (IsMouseInValidArea == false)
        {
            return;
        }

        Handles.color = new Color(EditorPrefs.GetFloat("CubeHandleColorR", 1f), EditorPrefs.GetFloat("CubeHandleColorG", 1f), EditorPrefs.GetFloat("CubeHandleColorB", 0f));

        DrawHandlesCube(CurrentHandlePosition);
    }

    static void DrawHandlesCube(Vector3 center)
    {
        Vector3 p1 = center + Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        Vector3 p2 = center + Vector3.up * 0.5f + Vector3.right * 0.5f - Vector3.forward * 0.5f;
        Vector3 p3 = center + Vector3.up * 0.5f - Vector3.right * 0.5f - Vector3.forward * 0.5f;
        Vector3 p4 = center + Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;

        Vector3 p5 = center - Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        Vector3 p6 = center - Vector3.up * 0.5f + Vector3.right * 0.5f - Vector3.forward * 0.5f;
        Vector3 p7 = center - Vector3.up * 0.5f - Vector3.right * 0.5f - Vector3.forward * 0.5f;
        Vector3 p8 = center - Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;

        // 我们可以使用Handles类来在Scene视图绘制3D物体
        // 如果实现恰当的话，我们甚至可以和handles进行交互，例如Unity的移动工具
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p4, p1);

        Handles.DrawLine(p5, p6);
        Handles.DrawLine(p6, p7);
        Handles.DrawLine(p7, p8);
        Handles.DrawLine(p8, p5);

        Handles.DrawLine(p1, p5);
        Handles.DrawLine(p2, p6);
        Handles.DrawLine(p3, p7);
        Handles.DrawLine(p4, p8);
    }
}
