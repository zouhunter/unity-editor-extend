using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class DataTableWindow : EditorWindow
{

    [MenuItem("Window/DataTable")]
    static void CreateWindow()
    {
        EditorWindow window = EditorWindow.GetWindow<DataTableWindow>("数据配制", true);
        window.position = new Rect(200, 300, 600, 400);
        window.maxSize = new Vector2(600, 400);
    }

    private LoadForceDataObject dataObj;

    private Vector2 bodyScorll;
    private List<DataList> dataLists
    {
        get
        {
            return dataObj.dataList ?? (dataObj.dataList = new List<DataList>());
        }
    }
    private Dictionary<DataItem, string> stringTempDic = new Dictionary<DataItem, string>();
    private Dictionary<DataList, Vector2> listScroll = new Dictionary<DataList, Vector2>();
    private Dictionary<DataList, bool> viewDic = new Dictionary<DataList, bool>();
    private Dictionary<DataItem, bool> ItemViewDic = new Dictionary<DataItem, bool>();
    private DataList waitDeleteList;
    static class LayoutOption
    {
        public static GUILayoutOption minWidth = GUILayout.Width(20);
        public static GUILayoutOption shortWidth = GUILayout.Width(50);
        public static GUILayoutOption mediaWidth = GUILayout.Width(75);
        public static GUILayoutOption longWidth = GUILayout.Width(100);
        public static GUILayoutOption maxWidth = GUILayout.Width(200);

        public static GUILayoutOption shortHigh = GUILayout.Height(EditorGUIUtility.singleLineHeight);
        public static GUILayoutOption mediaHigh = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);
        public static GUILayoutOption longHigh = GUILayout.Height(EditorGUIUtility.singleLineHeight * 5);
        public static GUILayoutOption maxHight = GUILayout.Height(EditorGUIUtility.singleLineHeight * 10);
    }

    void OnEnable()
    {
        Object obj = Selection.activeObject;
        if (obj != null && obj is LoadForceDataObject)
        {
            dataObj = (LoadForceDataObject)obj;
        }
    }

    void OnGUI()
    {
        DrawHead();
        if (dataObj != null)
        {
            DrawBody();
        }
        DrawListsTools();
    }

    void DrawHead()
    {
        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            GUI.Box(scope.rect, new GUIContent());
            EditorGUILayout.SelectableLabel("数据源", LayoutOption.shortWidth);
            dataObj = EditorGUILayout.ObjectField(dataObj, typeof(LoadForceDataObject), false, LayoutOption.longWidth) as LoadForceDataObject;
        }

    }
    void DrawBody()
    {
        using (var scope = new EditorGUILayout.ScrollViewScope(bodyScorll))
        {
            bodyScorll = scope.scrollPosition;
            foreach (var item in dataLists)
            {
                using (var vscope = new EditorGUILayout.VerticalScope())
                {
                    GUI.Box(vscope.rect, new GUIContent());
                    if (DrawListHeader(item))
                    {
                        DrawListBody(item);
                        GUILayout.Space(EditorGUIUtility.singleLineHeight);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            if (waitDeleteList != null)
            {
                dataLists.Remove(waitDeleteList);
            }
        }
    }

    void DrawListsTools()
    {
        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("+", LayoutOption.shortWidth))
            {
                dataLists.Add(new DataList());
            }
            GUI.backgroundColor = Color.white;
        }
    }
    bool DrawListHeader(DataList list)
    {
        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            if (!viewDic.ContainsKey(list))
            {
                viewDic.Add(list, true);
            }
            //using(var fScope = new EditorGUILayout.FadeGroupScope())
            if (viewDic[list] = GUILayout.Toggle(viewDic[list], list.name,LayoutOption.mediaHigh))
            {
                GUI.backgroundColor = Color.green;
                GUI.Box(scope.rect, "");
                GUI.backgroundColor = Color.white;
                EditorGUILayout.LabelField("ListName", LayoutOption.longWidth);
                list.name = EditorGUILayout.TextField(list.name, LayoutOption.longWidth);
                list.values = list.values ?? new List<DataItem>();

                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button("+", LayoutOption.minWidth))
                {
                    list.values.Add(new DataItem());
                }
               
                GUI.backgroundColor = Color.white;
                return true;
            }
            else
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("-", LayoutOption.minWidth))
                {
                    waitDeleteList = list;
                }
                GUI.backgroundColor = Color.white;
                return false;
            }

        }
    }

    void DrawListBody(DataList list)
    {
        if (!listScroll.ContainsKey(list))
        {
            listScroll.Add(list, Vector2.zero);
        }
        using (var scope = new EditorGUILayout.ScrollViewScope(listScroll[list], LayoutOption.maxHight))
        {
            listScroll[list] = scope.scrollPosition;
            if (DrawDataItemHead(list.key, "Key"))
            {
                DrawDataItemBody(list.key);
                DrawItemAxis(list.key);
            }
            DrawValuesItems(list.values);
        }
    }


    void DrawItemAxis(DataItem item)
    {
        item = item ?? new DataItem();
        using (var hScope = new EditorGUILayout.HorizontalScope())
        {
            GUI.Box(hScope.rect, "");
            EditorGUILayout.LabelField("graphName", LayoutOption.mediaWidth);
            EditorGUILayout.LabelField("graph", LayoutOption.mediaWidth);
            EditorGUILayout.LabelField("axisMin", LayoutOption.mediaWidth);
            EditorGUILayout.LabelField("axisMax", LayoutOption.mediaWidth);
            EditorGUILayout.LabelField("axisSize", LayoutOption.mediaWidth);

        }
        using (var hScope = new EditorGUILayout.HorizontalScope())
        {
            GUI.Box(hScope.rect, "");
            item.graphName = EditorGUILayout.TextField(item.graphName ?? "", LayoutOption.mediaWidth);
            item.graph = EditorGUILayout.Toggle(item.graph, LayoutOption.mediaWidth);
            item.axisMin = EditorGUILayout.FloatField(item.axisMin, LayoutOption.mediaWidth);
            item.axisMax = EditorGUILayout.FloatField(item.axisMax, LayoutOption.mediaWidth);
            item.axisSize = EditorGUILayout.FloatField(item.axisSize, LayoutOption.mediaWidth);
        }
    }

    void DrawValuesItems(List<DataItem> items)
    {
        items = items ?? new List<DataItem>();

        for (int i = 0; i < items.Count; i++)
        {
            if (DrawValueItemTool(items, items[i]))
            {
                if (DrawDataItemHead(items[i], "Value" + (i + 1)))
                {
                    DrawDataItemBody(items[i]);
                    DrawItemAxis(items[i]);
                }
            }
            else
            {
                return;
            }
        }
    }

    private bool DrawValueItemTool(List<DataItem> items, DataItem item)
    {
        using (var hScope = new EditorGUILayout.HorizontalScope())
        {
            GUI.Box(hScope.rect, "");
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("-", LayoutOption.minWidth,LayoutOption.shortHigh))
            {
                items.Remove(item);
                return false;
            }
            GUI.backgroundColor = Color.white;
        }
        return true;
    }
    bool DrawDataItemHead(DataItem item, string keyName)
    {
        item = item ?? new DataItem();
        using (var hScope = new EditorGUILayout.HorizontalScope())
        {
            GUI.Box(hScope.rect, "");

            GUILayout.Label(keyName, LayoutOption.shortWidth);
            
            if (!ItemViewDic.ContainsKey(item))
            {
                ItemViewDic.Add(item, true);
            }

            if (ItemViewDic[item] = EditorGUILayout.Toggle(ItemViewDic[item],LayoutOption.minWidth, LayoutOption.shortHigh))
            {
                GUILayout.Label("UnitName", LayoutOption.mediaWidth);
                GUILayout.Label("Data", LayoutOption.mediaWidth);
                return true;
            }
         
            return false;
        }
    }
    void DrawDataItemBody(DataItem item)
    {
        item = item ?? new DataItem();
        using (var hScope = new EditorGUILayout.HorizontalScope())
        {
            GUI.Box(hScope.rect, "");
            item.name = EditorGUILayout.TextField(item.name ?? "", LayoutOption.mediaWidth);
            item.unitName = EditorGUILayout.TextField(item.unitName ?? "", LayoutOption.mediaWidth);
            if (!stringTempDic.ContainsKey(item))
            {
                stringTempDic.Add(item, "");
            }

            stringTempDic[item] = EditorGUILayout.TextField(stringTempDic[item], LayoutOption.mediaWidth);

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("转换-->", LayoutOption.mediaWidth))
            {
                item.values.Clear();
                Debug.Log(item.values.Count);
                string[] array = stringTempDic[item].Split(new char[] { ',', ',', ' ' });
                float value;
                for (int i = 0; i < array.Length; i++)
                {
                    if (float.TryParse(array[i], out value))
                    {
                        item.values.Add(value);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "数据不合法", "确认");
                    }
                }
            }
            GUI.backgroundColor = Color.white;
            item.values = item.values ?? new List<float>();
            for (int i = 0; i < item.values.Count; i++)
            {
                item.values[i] = EditorGUILayout.FloatField(item.values[i], LayoutOption.shortWidth);
            }
        }
    }
    void OnDisable()
    {
        EditorUtility.SetDirty(dataObj);
        Debug.Log("Save");
    }
}
