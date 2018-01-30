using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[InitializeOnLoad]
public class LevelEditorE07ToolsMenu : Editor
{
    //This is a public variable that gets or sets which of our custom tools we are currently using
    //0 - No tool selected
    //1 - The block eraser tool is selected
    //2 - The "Add block" tool is selected
    public static int SelectedTool
    {
        get
        {
            return EditorPrefs.GetInt("SelectedEditorTool", 0);
        }
        set
        {
            if (value == SelectedTool)
            {
                return;
            }

            EditorPrefs.SetInt("SelectedEditorTool", value);

            switch (value)
            {
                case 0:
                    EditorPrefs.SetBool("IsLevelEditorEnabled", false);

                    Tools.hidden = false;
                    break;
                case 1:
                    EditorPrefs.SetBool("IsLevelEditorEnabled", true);
                    EditorPrefs.SetBool("SelectBlockNextToMousePosition", false);
                    EditorPrefs.SetFloat("CubeHandleColorR", Color.magenta.r);
                    EditorPrefs.SetFloat("CubeHandleColorG", Color.magenta.g);
                    EditorPrefs.SetFloat("CubeHandleColorB", Color.magenta.b);

                    //Hide Unitys Tool handles (like the move tool) while we draw our own stuff
                    Tools.hidden = true;
                    break;
                default:
                    EditorPrefs.SetBool("IsLevelEditorEnabled", true);
                    EditorPrefs.SetBool("SelectBlockNextToMousePosition", true);
                    EditorPrefs.SetFloat("CubeHandleColorR", Color.yellow.r);
                    EditorPrefs.SetFloat("CubeHandleColorG", Color.yellow.g);
                    EditorPrefs.SetFloat("CubeHandleColorB", Color.yellow.b);

                    //Hide Unitys Tool handles (like the move tool) while we draw our own stuff
                    Tools.hidden = true;
                    break;
            }
        }
    }
    static LevelBlocks m_LevelBlocks;

    static LevelEditorE07ToolsMenu()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;

        // EditorApplication.hierarchyWindowChanged可以让我们知道是否在编辑器加载了一个新的场景
        EditorApplication.hierarchyWindowChanged -= OnSceneChanged;
        EditorApplication.hierarchyWindowChanged += OnSceneChanged;
        //EditorApplication.projectWindowItemOnGUI += OnProjectWindow;
        
        m_LevelBlocks = AssetDatabase.LoadAssetAtPath<LevelBlocks>("Assets/Core/SceneEditor/my_blocks.asset");
    }


    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        //EditorApplication.projectWindowItemOnGUI -= OnProjectWindow;
        EditorApplication.hierarchyWindowChanged -= OnSceneChanged;
    }

    static void OnSceneChanged()
    {
        if (IsInCorrectLevel() == true)
        {
            Tools.hidden = LevelEditorE07ToolsMenu.SelectedTool != 0;
        }
        else
        {
            Tools.hidden = false;
        }
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (IsInCorrectLevel() == false)
        {
            return;
        }

        DrawToolsMenu(sceneView.position);

        EventAction();

        DrawPrefabs(sceneView.position);
    }

    static void EventAction( )
    {
        // 通过创建一个新的ControlID我们可以把鼠标输入的Scene视图反应权从Unity默认的行为中抢过来
        // FocusType.Passive意味着这个控制权不会接受键盘输入而只关心鼠标输入
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        // 如果是鼠标左键被点击同时没有其他特定按键按下的话
        if (Event.current.type == EventType.mouseDown &&
            Event.current.button == 0 &&
            Event.current.alt == false &&
            Event.current.shift == false &&
            Event.current.control == false)
        {
            if (LevelEditorE06CubeHandle.IsMouseInValidArea == true)
            {
                if (LevelEditorE07ToolsMenu.SelectedTool == 1)
                {
                    // 如果选择的是erase按键（从场景七的静态变量SelectedTool判断得到），移除Cube          
                    RemoveBlock(LevelEditorE06CubeHandle.CurrentHandlePosition);
                }

                if (LevelEditorE07ToolsMenu.SelectedTool == 2)
                {
                    /// 如果选择的是add按键（从场景七的静态变量SelectedTool判断得到），添加Cube
                    AddBlock(LevelEditorE06CubeHandle.CurrentHandlePosition);
                }
            }
        }

        // 如果按下了Escape，我们就自动取消选择当前的按钮
        if (Event.current.type == EventType.keyDown &&
            Event.current.keyCode == KeyCode.Escape)
        {
            LevelEditorE07ToolsMenu.SelectedTool = 0;
        }

        if (Event.current.keyCode == KeyCode.A)
        {
            return;
        }

        // 把我们自己的controlId添加到默认的control里，这样Unity就会选择我们的控制权而非Unity默认的Scene视图行为
        HandleUtility.AddDefaultControl(controlId);
    }

 
    private static void RemoveBlock(Vector3 currentHandlePosition)
    {
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = currentHandlePosition;
    }

    private static void AddBlock(Vector3 currentHandlePosition)
    {
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = currentHandlePosition;
    }

    static bool IsInCorrectLevel()
    {
        return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name == "GameE07";
    }

    static void DrawToolsMenu(Rect position)
    {
        //// 通过使用Handles.BeginGUI()，我们可以开启绘制Scene视图的GUI元素
        //Handles.BeginGUI();

        ////Here we draw a toolbar at the bottom edge of the SceneView
        //// 这里我们在Scene视图的底部绘制了一个工具条
        //GUILayout.BeginArea(new Rect(0, position.height - 35, position.width, 20), EditorStyles.toolbar);
        //{
        //    string[] buttonLabels = new string[] { "None", "Erase", "Paint" };

        //    // GUILayout.SelectionGrid提供了一个按钮工具条
        //    // 通过把它的返回值存储在SelectedTool里可以让我们根据不同的按钮来实现不同的行为
        //    SelectedTool = GUILayout.SelectionGrid(
        //        SelectedTool,
        //        buttonLabels,
        //        3,
        //        EditorStyles.toolbarButton,
        //        GUILayout.Width(300));
        //}
        //GUILayout.EndArea();

        //Handles.EndGUI();
    }
    private static void DrawPrefabs(Rect position)
    {
        // 通过使用Handles.BeginGUI()，我们可以开启绘制Scene视图的GUI元素
        Handles.BeginGUI();

        //Here we draw a toolbar at the bottom edge of the SceneView
        // 这里我们在Scene视图的底部绘制了一个工具条
        GUILayout.BeginArea(new Rect(0, 0, 100, position.height), EditorStyles.toolbar);
        {
            GUIContent content = new GUIContent(AssetPreview.GetAssetPreview(m_LevelBlocks.Blocks[0].Prefab));
            if (GUI.Button(new Rect(0, 0, 100, 100), content))
            {
                Debug.Log(m_LevelBlocks.Blocks[0].Prefab);
            }
        }
        GUILayout.EndArea();

        Handles.EndGUI();
    }

}