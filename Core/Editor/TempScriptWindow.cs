#region statement
/*【author】        : zouhunter*/
/*【time】          : 2018/2/2*/
/*【description】   : 这是一个快速创建类模板的窗体扩展,帮助你减少重复劳动。*/
/*
        1.支持枚举、模型、结构和继承等类的模板。
        2.支持快速创建通用型UI界面脚本
        3.支持自定义模板类
        4.自动生成作者、创建时间、描述等功能
        5.支持工程同步（EditorPrefer）
*/
#endregion

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

namespace EditorTools
{
    /// <summary>
    /// 一个创建脚本模板的窗口
    /// </summary>
    public class TempScriptWindow : EditorWindow
    {
        [MenuItem("Tools/TempScriptWindow")]
        static void Open()
        {
            var window = TempScriptHelper.GetWindow();
            window.wantsMouseMove = true;
        }

        [SerializeField]
        private List<ScriptTemplate> templates;
        private MonoScript script;
        [SerializeField]
        private bool isSetting;
        [SerializeField]
        private string authorName;
        [SerializeField]
        private string[] templateNames;
        [SerializeField]
        private int currentIndex;
        [SerializeField]
        private TempScriptHeader headerInfo;
        private Vector2 scrollPos;
        private ReorderableList detailList;

        private void OnEnable()
        {
            InitEnviroment();
        }

        private void OnDisable()
        {
            if (templates != null)
            {
                foreach (var item in templates)
                {

                    item.SaveToJson();
                }
            }
            EditorUtility.SetDirty(this);
            TempScriptHelper.SaveWindow(this);
        }
        private void OnGUI()
        {
            DrawHead();
            if (isSetting)
            {
                //绘制设置信息
                DrawSettings();
            }
            else
            {
                if (templates == null)
                {
                    Debug.Log("template == null");
                    templates = new List<ScriptTemplate>();
                }

                if (templates.Count == 0)
                {

                    Debug.Log("AddTemplates");
                    AddTemplates();
                }

                if (headerInfo == null)
                {
                    headerInfo = new TempScriptHeader();
                }

                if (detailList == null)
                {
                    InitDetailList();
                }

                if (detailList.list != headerInfo.detailInfo)
                {
                    detailList.list = headerInfo.detailInfo;
                }

                currentIndex = GUILayout.Toolbar(currentIndex, templateNames);
                using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scroll.scrollPosition;
                    if (templates.Count > currentIndex)
                    {
                        var currentTemplates = templates[currentIndex];
                        if (currentTemplates.GetType().FullName != currentTemplates.type)
                        {
                            templates[currentIndex] = TempScriptHelper.LoadFromJson(currentTemplates);
                        }
                        templates[currentIndex].OnGUI();
                    }
                    else
                    {
                        templates.Clear();
                    }
                }
                DrawFoot();
            }

        }
        private void InitDetailList()
        {
            detailList = new ReorderableList(headerInfo.detailInfo, typeof(string), true, false, true, true);
            detailList.onAddCallback += (x) => { headerInfo.detailInfo.Add(""); };
            detailList.drawHeaderCallback = (x) => { EditorGUI.LabelField(x, "详细信息"); };
            detailList.drawElementCallback += (x, y, z, w) => { headerInfo.detailInfo[y] = EditorGUI.TextField(x, headerInfo.detailInfo[y]); };
        }

        private void InitEnviroment()
        {
            if (script == null) script = MonoScript.FromScriptableObject(this);
            if (string.IsNullOrEmpty(authorName)) authorName = TempScriptHelper.GetAuthor();
            if (string.IsNullOrEmpty(authorName))
            {
                isSetting = true;
            }
        }

        private void AddTemplates()
        {
            templates.Add(new EnumScriptTemplate());
            templates.Add(new StaticClassTemplate());
            templates.Add(new DataModelTemplate());
            templates.Add(new ExtendClassTemplate());
            templates.Add(new StructTempate());
            templates.Add(new UIPanelTempate());
            templateNames = templates.ConvertAll<string>(x => x.Name).ToArray();
        }

        public void LoadOldTemplates()
        {
            for (int i = 0; i < templates.Count; i++)
            {
                if (templates[i] == null)
                {
                    templates = null;
                    return;
                }

                var newitem = TempScriptHelper.LoadFromJson(templates[i]);

                if (newitem == null)
                {
                    templates = null;
                    return;
                }
                templates[i] = newitem;
            }
        }

        private void DrawHead()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.ObjectField(script, typeof(MonoScript), false);
                if (!isSetting && GUILayout.Button("setting", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    isSetting = true;
                }
                else if (isSetting && GUILayout.Button("confer", EditorStyles.miniButtonRight, GUILayout.Width(60)) && !string.IsNullOrEmpty(authorName))
                {
                    isSetting = false;
                }
            }
        }

        private void DrawSettings()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.SelectableLabel("作者:", EditorStyles.miniLabel, GUILayout.Width(60));
                authorName = EditorGUILayout.TextField(authorName);
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrEmpty(authorName))
                    {
                        TempScriptHelper.SaveAuthor(authorName);
                    }
                }
            }
        }
        private void DrawFoot()
        {
            using (var horm = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("命名", GUILayout.Width(60));
                headerInfo.scriptName = EditorGUILayout.TextField(headerInfo.scriptName);
                EditorGUILayout.LabelField("简介", GUILayout.Width(60));
                headerInfo.description = EditorGUILayout.TextField(headerInfo.description);
                if (GUILayout.Button("Create", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    OnCreateButtonClicked();
                }
            }
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                using (var vertical = new EditorGUILayout.VerticalScope())
                {
                    detailList.DoLayoutList();
                }
                if (GUILayout.Button("Clear", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    headerInfo = null;
                    templates.Clear();
                }
            }
        }

        private void OnCreateButtonClicked()
        {
            var scriptStr = templates[currentIndex].Create(headerInfo);
            if (string.IsNullOrEmpty(scriptStr))
            {
                EditorUtility.DisplayDialog("生成失败", "请看日志！", "确认");
                return;
            }

            if (string.IsNullOrEmpty(headerInfo.scriptName))
            {
                EditorUtility.DisplayDialog("脚本名为空", "请填写代码名称！", "确认");
                return;
            }


            string path = null;
            if (ProjectWindowUtil.IsFolder(Selection.activeInstanceID))
            {
                path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            }
            else if (Selection.activeObject != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    path = assetPath.Replace(System.IO.Path.GetFileName(assetPath), "");
                }
            }

            if (!string.IsNullOrEmpty(path))
            {
                var scriptPath = string.Format("{0}/{1}.cs", path, headerInfo.scriptName);
                System.IO.File.WriteAllText(scriptPath, scriptStr, System.Text.Encoding.UTF8);

            }
            else
            {
                EditorUtility.DisplayDialog("路径不明", "请选中文件夹后重试", "确认");
            }
        }
    }


    /// <summary>
    /// 任何脚本的头
    /// </summary>
    [System.Serializable]
    public class TempScriptHeader
    {
        public string author;
        public string time;
        public string description;
        public List<string> detailInfo = new List<string>();
        public string scriptName;
    }

    /// <summary>
    /// 静态工具类
    /// </summary>
    public static class TempScriptHelper
    {
        private const string prefer_key = "temp_script_autor_name";
        private const string prefer_window = "temp_script_window";
        public static void SaveAuthor(string author)
        {
            EditorPrefs.SetString(prefer_key, author);
        }

        public static string GetAuthor()
        {
            return EditorPrefs.GetString(prefer_key);
        }

        public static void SaveWindow(TempScriptWindow window)
        {
            var json = JsonUtility.ToJson(window);
            EditorPrefs.SetString(prefer_window, json);
        }
        public static TempScriptWindow GetWindow()
        {
            var window = EditorWindow.GetWindow<TempScriptWindow>();
            if (EditorPrefs.HasKey(prefer_key))
            {
                var json = EditorPrefs.GetString(prefer_window);
                JsonUtility.FromJsonOverwrite(json, window);
                window.LoadOldTemplates();
                return window;
            }
            return window;
        }
        internal static ScriptTemplate LoadFromJson(ScriptTemplate old)
        {
            if (!string.IsNullOrEmpty(old.type) && old.GetType().FullName != old.type)
            {
                var temp = Activator.CreateInstance(Type.GetType(old.type));
                JsonUtility.FromJsonOverwrite(old.json, temp);
                return temp as ScriptTemplate;
            }
            else
            {
                return old;
            }
        }
    }

    /// <summary>
    /// 代码创建模板的模板
    /// </summary>
    [System.Serializable]
    public class ScriptTemplate
    {
        public string json;
        public string type;

        public virtual string Name { get { return null; } }
        public virtual string Create(TempScriptHeader header) { return null; }
        public virtual void OnGUI() { }

        internal void SaveToJson()
        {
            json = null;
            json = JsonUtility.ToJson(this);
            type = this.GetType().FullName;
        }
    }

    /// <summary>
    /// 1.枚举类型脚本
    /// </summary>
    [Serializable]
    public class EnumScriptTemplate : ScriptTemplate
    {
        public override string Name
        {
            get
            {
                return "Enum";
            }
        }
        [SerializeField]
        private List<string> types = new List<string>();
        private ReorderableList reorderableList;

        public EnumScriptTemplate()
        {
            reorderableList = new ReorderableList(types, typeof(string));
            reorderableList.onAddCallback += (x) => { types.Add(""); };
            reorderableList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "枚举列表"); };
            reorderableList.drawElementCallback += (x, y, z, w) => { types[y] = EditorGUI.TextField(x, types[y]); };
        }
        public override string Create(TempScriptHeader header)
        {
            return "xxX";
        }

        public override void OnGUI()
        {
            reorderableList.DoLayoutList();
        }
    }

    /// <summary>
    /// 2.数据模拟类
    /// </summary>
    public class DataModelTemplate : ScriptTemplate
    {
        public override string Name
        {
            get
            {
                return "Model";
            }
        }

        public override string Create(TempScriptHeader header)
        {
            return "";
        }

        public override void OnGUI()
        {

        }
    }

    /// <summary>
    /// 3.静态类
    /// </summary>
    public class StaticClassTemplate : ScriptTemplate
    {
        public override string Name
        {
            get
            {
                return "Static";
            }
        }

        public override string Create(TempScriptHeader header)
        {
            return "";
        }

        public override void OnGUI()
        {

        }
    }

    /// <summary>
    /// 4.继承其他类的子类
    /// </summary>
    public class ExtendClassTemplate : ScriptTemplate
    {
        public override string Name
        {
            get
            {
                return "Extend";
            }
        }

        public override string Create(TempScriptHeader header)
        {
            return "";
        }

        public override void OnGUI()
        {

        }
    }

    /// <summary>
    /// 5.结构体模板
    /// </summary>
    public class StructTempate : ScriptTemplate
    {
        public override string Name
        {
            get
            {
                return "Struct";
            }
        }

        public override string Create(TempScriptHeader header)
        {
            return "";
        }

        public override void OnGUI()
        {

        }
    }

    /// <summary>
    /// UI模板
    /// </summary>
    public class UIPanelTempate : ScriptTemplate
    {
        public override string Name
        {
            get
            {
                return "UIPanel";
            }
        }

        public override string Create(TempScriptHeader header)
        {
            return "";
        }

        public override void OnGUI()
        {

        }
    }

    ///下面可以自定义你的代码生成模板,并在窗体方法RegistTempates中注册
    ///...
}
