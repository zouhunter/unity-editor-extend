#region statement
/************************************************************************************* 
    * 作    者：       zouhunter
    * 时    间：       2018-02-02 
    * 详    细：       1.支持枚举、模型、结构和继承等类的模板。
                       2.支持快速创建通用型UI界面脚本
                       3.支持自定义模板类
                       4.自动生成作者、创建时间、描述等功能
                       5.支持工程同步（EditorPrefer）
   *************************************************************************************/
#endregion

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.CodeDom;
using System.IO;
using System.Text;
using NUnit.Framework.Constraints;
using NUnit.Framework;

namespace EditorTools
{
    #region Window
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
        [SerializeField]
        private string path;
       
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
                            currentTemplates = templates[currentIndex] = TempScriptHelper.LoadFromJson(currentTemplates);
                        }

                        currentTemplates.OnGUI();

                        if (currentTemplates.GetType().FullName == typeof(ScriptTemplate).FullName)
                        {
                            templates.Clear();
                            Debug.Log(currentTemplates.GetType());
                        }
                    }
                    else
                    {
                        templates.Clear();
                        Debug.Log("templates.Count <= currentIndex");
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

            foreach (var item in templates)
            {
                //不能再父级的构造器中与获取type的方法
                item.OnEnable();
            }
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
                EditorGUILayout.LabelField("Namespace", GUILayout.Width(70));
                headerInfo.nameSpace = EditorGUILayout.TextField(headerInfo.nameSpace, GUILayout.Width(60));
                EditorGUILayout.LabelField("Type", GUILayout.Width(40));
                headerInfo.scriptName = EditorGUILayout.TextField(headerInfo.scriptName, GUILayout.Width(60));
                EditorGUILayout.LabelField("简介", GUILayout.Width(40));
                headerInfo.description = EditorGUILayout.TextField(headerInfo.description);

                if (GUILayout.Button("Load", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    OnLoadButtonClicked();
                }
            }
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                using (var vertical = new EditorGUILayout.VerticalScope())
                {
                    detailList.DoLayoutList();
                }
                using (var vertical = new EditorGUILayout.VerticalScope(GUILayout.Width(60)))
                {
                    if (GUILayout.Button("Create", EditorStyles.miniButtonRight, GUILayout.Height(60)))
                    {
                        OnCreateButtonClicked();
                    }
                    if (GUILayout.Button("Clear", EditorStyles.miniButtonRight))
                    {
                        headerInfo = null;
                        templates.Clear();
                    }
                }
            }
        }
        /// <summary>
        /// 点击创建
        /// </summary>
        private void OnCreateButtonClicked()
        {
            headerInfo.author = authorName;
            headerInfo.time = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
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


            if (string.IsNullOrEmpty(path))
            {
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
            }


            if (!string.IsNullOrEmpty(path))
            {
                var scriptPath = string.Format("{0}/{1}.cs", path, headerInfo.scriptName);
                System.IO.File.WriteAllText(scriptPath, scriptStr, System.Text.Encoding.UTF8);
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("路径不明", "请选中文件夹后重试", "确认");
            }
        }

        /// <summary>
        /// 点击加载代码
        /// </summary>
        private void OnLoadButtonClicked()
        {
            if (!(Selection.activeObject is TextAsset))
            {
                EditorUtility.DisplayDialog("未选中", "请选中需要解析的代码后继续", "确认");
                return;
            }

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.EndsWith(".cs"))
            {
                EditorUtility.DisplayDialog("未选中", "请选中需要解析的代码后继续", "确认");
                return;
            }

            using (var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp"))
            {
                EditorUtility.DisplayDialog("未开发", ".net 3.5暂无该实现", "确认");

                var fileContent = System.IO.File.ReadAllText(path, Encoding.UTF8);
                using (StringReader sr = new StringReader(fileContent))
                {
                    var nameSpaceUnit = provider.Parse(sr);
                    Debug.Log(nameSpaceUnit);
                }
            }
        }
    }
    #endregion

    #region Tools
    /// <summary>
    /// 任何脚本的头
    /// </summary>
    [System.Serializable]
    public class TempScriptHeader
    {
        public string author;
        public string time;
        public string description;
        public List<string> codeRule = new List<string>();
        public List<string> detailInfo = new List<string>();
        public string scriptName;
        public string nameSpace;
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
                old.type = old.GetType().FullName;
                return old;
            }
        }
    }
    #endregion

    #region Templates
    /// <summary>
    /// 代码创建模板的模板
    /// </summary>
    [System.Serializable]
    public class ScriptTemplate
    {
        public string json;
        public string type;
        public virtual string Name { get { return null; } }
        public virtual string Create(TempScriptHeader header) { return GetHeader(header); }
        public virtual void OnGUI() { }

        public void OnEnable()
        {
            type = this.GetType().FullName;
            Debug.Log(type);
        }
        internal void SaveToJson()
        {
            json = null;
            json = JsonUtility.ToJson(this);
            type = this.GetType().FullName;
        }
        protected string GetHeader(TempScriptHeader header)
        {
            var str1 = "#region statement\r\n" +
            "/*************************************************************************************   \r\n" +
            "    * 作    者：       {0}\r\n" +
            "    * 时    间：       {1}\r\n" +
            "    * 说    明：       ";
            var str2 = "\r\n                       ";
            var str3 = "\r\n* ************************************************************************************/" +
            "\r\n#endregion\r\n";

            var headerStr = string.Format(str1, header.author, header.time);
            for (int i = 0; i < header.detailInfo.Count; i++)
            {
                if (i == 0)
                {
                    headerStr += string.Format("{0}.{1}", i + 1, header.detailInfo[i]);
                }
                else
                {
                    headerStr += string.Format("{0}{1}.{2}", str2, i + 1, header.detailInfo[i]);
                }
            }
            headerStr += str3;
            return headerStr;
        }


        protected string ComplieToString(CodeNamespace nameSpace)
        {
            using (Microsoft.CSharp.CSharpCodeProvider cprovider = new Microsoft.CSharp.CSharpCodeProvider())
            {
                StringBuilder fileContent = new StringBuilder();
                var option = new System.CodeDom.Compiler.CodeGeneratorOptions();
                option.BlankLinesBetweenMembers = false;
                using (StringWriter sw = new StringWriter(fileContent))
                {
                    cprovider.GenerateCodeFromNamespace(nameSpace, sw, option);
                }
                return fileContent.ToString();
            }
        }
    }
    #endregion


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
        private List<EnumItem> elements = new List<EnumItem>();
        private ReorderableList reorderableList;

        [System.Serializable]
        public class EnumItem
        {
            public string elementName;
            public string comment;
        }

        public EnumScriptTemplate()
        {
            reorderableList = new ReorderableList(elements, typeof(string));
            reorderableList.onAddCallback += (x) => { elements.Add(new EnumItem()); };
            reorderableList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "枚举列表"); };
            reorderableList.drawElementCallback += (x, y, z, w) =>
            {
                DrawEnumItem(x, elements[y]);
            };
        }

        protected void DrawEnumItem(Rect rect, EnumItem tupe)
        {
            var left = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            var right = new Rect(rect.x + rect.width * 0.4f, rect.y, rect.width * 0.6f, EditorGUIUtility.singleLineHeight);
            var center = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
            tupe.elementName = EditorGUI.TextField(left, tupe.elementName);
            EditorGUI.LabelField(center, "Comment");
            tupe.comment = EditorGUI.TextField(right, tupe.comment);
        }
        public override string Create(TempScriptHeader header)
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            foreach (var item in elements)
            {
                CodeMemberField prop = new CodeMemberField();
                prop.Name = item.elementName;
                prop.Comments.Add(new CodeCommentStatement(item.comment));
                fields.Add(prop);
            }

            CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration(header.scriptName);
            wrapProxyClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            wrapProxyClass.IsEnum = true;

            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(header.description, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            foreach (var field in fields)
            {
                wrapProxyClass.Members.Add(field);
            }

            CodeNamespace nameSpace = new CodeNamespace(header.nameSpace);
            nameSpace.Types.Add(wrapProxyClass);
            var headerStr = base.Create(header);
            return headerStr + ComplieToString(nameSpace);
        }


        public override void OnGUI()
        {
            reorderableList.DoLayoutList();
        }
    }

    /// <summary>
    /// 2.数据模拟类
    /// </summary>
    [Serializable]
    public class DataModelTemplate : ScriptTemplate
    {
        [System.Serializable]
        public class DataItem
        {
            public string type;
            public string elementName;
            public string comment;
        }
        public override string Name
        {
            get
            {
                return "Model";
            }
        }
        [SerializeField]
        private List<DataItem> elements = new List<DataItem>();
        private ReorderableList reorderableList;

        public DataModelTemplate()
        {
            reorderableList = new ReorderableList(elements, typeof(string));
            reorderableList.onAddCallback += (x) => { elements.Add(new DataItem()); };
            reorderableList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "模型名"); };
            reorderableList.drawElementCallback += (x, y, z, w) =>
            {
                DrawDataItem(x, elements[y]);
            };
        }
        public override string Create(TempScriptHeader header)
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            foreach (var item in elements)
            {
                CodeMemberField prop = new CodeMemberField();
                prop.Type = new CodeTypeReference(item.type,CodeTypeReferenceOptions.GenericTypeParameter);
                prop.Attributes = MemberAttributes.Public;
                prop.Name = item.elementName;
                prop.Comments.Add(new CodeCommentStatement(item.comment));
                fields.Add(prop);
            }

            CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration(header.scriptName);
            wrapProxyClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            wrapProxyClass.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(System.SerializableAttribute).FullName));
            wrapProxyClass.IsClass = true;

            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(header.description, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            foreach (var field in fields)
            {
                wrapProxyClass.Members.Add(field);
            }

            CodeNamespace nameSpace = new CodeNamespace(header.nameSpace);
            nameSpace.Types.Add(wrapProxyClass);
            var headerStr = base.Create(header);
            return headerStr + ComplieToString(nameSpace);
        }

        private void DrawDataItem(Rect rect, DataItem dataItem)
        {
            var rect01 = new Rect(rect.x, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight);
            var rect02 = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            var rect03 = new Rect(rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);

            dataItem.elementName = EditorGUI.TextField(rect01, dataItem.elementName);
            dataItem.type = EditorGUI.TextField(rect02, dataItem.type);
            dataItem.comment = EditorGUI.TextField(rect03, dataItem.comment);
        }

        public override void OnGUI()
        {
            reorderableList.DoLayoutList();
        }
    }

    /// <summary>
    /// 3.静态类
    /// </summary>
    [Serializable]
    public class StaticClassTemplate : ScriptTemplate
    {
        public override string Name
        {
            get
            {
                return "Static";
            }
        }
        public StaticClassTemplate()
        {

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
    [Serializable]
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
    [Serializable]
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
    [Serializable]
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
