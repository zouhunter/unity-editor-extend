using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Tools for the editor
/// </summary>

public class EditorTools
{
    static Texture2D mBackdropTex;
    static Texture2D mContrastTex;
    static Texture2D mGradientTex;
    static Texture2D mAdditionTex;
    static Texture2D mDeleteTex;
    static GameObject mPrevious;
    static GUIStyle mHeaderStyle;
    static GUIStyle mRickTxtStyle;
    static GUIStyle mFoldoutStyle;
    static GUIStyle mPreButtonStyle;
    static GUIStyle mErrorStyle;
    static GUIContent mIconToolbarPlus;
    static GUIContent mIconToolbarMinus;

    /// <summary>
    /// Returns a blank usable 1x1 white texture.
    /// </summary>

    static public Texture2D blankTexture
    {
        get
        {
            return EditorGUIUtility.whiteTexture;
        }
    }

    /// <summary>
    /// Returns a usable texture that looks like a dark checker board.
    /// </summary>

    static public Texture2D backdropTexture
    {
        get
        {
            if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
                new Color(0.1f, 0.1f, 0.1f, 0.5f),
                new Color(0.2f, 0.2f, 0.2f, 0.5f));
            return mBackdropTex;
        }
    }

    /// <summary>
    /// Returns a usable texture that looks like a high-contrast checker board.
    /// </summary>

    static public Texture2D contrastTexture
    {
        get
        {
            if (mContrastTex == null) mContrastTex = CreateCheckerTex(
                new Color(0f, 0.0f, 0f, 0.5f),
                new Color(1f, 1f, 1f, 0.5f));
            return mContrastTex;
        }
    }

    /// <summary>
    /// Gradient texture is used for title bars / headers.
    /// </summary>

    static public Texture2D gradientTexture
    {
        get
        {
            if (mGradientTex == null) mGradientTex = CreateGradientTex();
            return mGradientTex;
        }
    }

    static public GUIStyle headerStyle
    {
        get
        {
            if (mHeaderStyle == null)
            {
                mHeaderStyle = new GUIStyle(GUI.skin.FindStyle("ShurikenModuleTitle"));
                headerStyle.contentOffset = new Vector2(3, -2);
            }

            return mHeaderStyle;
        }
    }

    static public GUIStyle rickTxtStyle
    {
        get
        {
            if (mRickTxtStyle == null)
            {
                mRickTxtStyle = new GUIStyle("AS TextArea");
                mRickTxtStyle.richText = true;
            }

            return mRickTxtStyle;
        }
    }

    static public GUIStyle foldoutStyle
    {
        get
        {
            if (mFoldoutStyle == null)
            {
                mFoldoutStyle = new GUIStyle("ShurikenEffectBg");
                mFoldoutStyle.padding = new RectOffset(10, 10, 0, 0);
            }
            return mFoldoutStyle;
        }
    }

    static public GUIStyle preButtonStyle
    {
        get
        {
            if (mPreButtonStyle == null) mPreButtonStyle = "RL FooterButton";
            return mPreButtonStyle;
        }
    }

    static public GUIStyle errrorStyle
    {
        get
        {
            if (mErrorStyle == null) mErrorStyle = "ErrorLabel";
            return mErrorStyle;
        }
    }

    static public GUIContent iconToolbarPlus
    {
        get
        {
            if (mIconToolbarPlus == null) mIconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
            return mIconToolbarPlus;
        }
    }

    static public GUIContent iconToolbarMinus
    {
        get
        {
            if (mIconToolbarMinus == null) mIconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "remove selection from list");
            return mIconToolbarMinus;
        }
    }

    static Texture2D CreateCheckerTex(Color c0, Color c1)
    {
        Texture2D tex = new Texture2D(16, 16);
        tex.name = "[Generated] Checker Texture";
        tex.hideFlags = HideFlags.DontSave;

        for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
        for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
        for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
        for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    static Texture2D CreateGradientTex()
    {
        Texture2D tex = new Texture2D(1, 16);
        tex.name = "[Generated] Gradient Texture";
        tex.hideFlags = HideFlags.DontSave;

        Color c0 = new Color(1f, 1f, 1f, 0f);
        Color c1 = new Color(1f, 1f, 1f, 0.4f);

        for (int i = 0; i < 16; ++i)
        {
            float f = Mathf.Abs((i / 15f) * 2f - 1f);
            f *= f;
            tex.SetPixel(0, i, Color.Lerp(c0, c1, f));
        }

        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return tex;
    }

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false); }

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false); }

    static public bool DrawHeader(string text, bool forceOn) { return DrawHeader(text, text, forceOn); }

    static public bool DrawHeader(string text, string key, bool forceOn)
    {
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        text = "<b><size=11>" + text + "</size></b>";
        if (state) text = "\u25B2 " + text;
        else text = "\u25BC " + text;
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    static public void BeginContents()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    static public void BeginBounds()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
    }

    static public void EndBounds()
    {
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    static public void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    static public bool BeginFoldout(string text, string key)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (state) text = "\u25B2 " + text;
        else text = "\u25BC " + text;

        GUILayout.BeginVertical("ShurikenEffectBg", GUILayout.MinHeight(EditorGUIUtility.singleLineHeight));

        state = GUI.Toggle(GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight), state, text, headerStyle);

        EditorPrefs.SetBool(key, state);

        return state;
    }

    static public void EndFoldout()
    {
        GUILayout.EndVertical();
    }

    static public void DrawToolbars(ref bool plus, ref bool minus, bool canRemove)
    {
        Rect rect = GUILayoutUtility.GetRect(4f, 13f, new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true)
        });

        float xMax = rect.xMax;
        float num = xMax - 8f;
        num -= 50f;
        Rect rect1 = new Rect(num + 4f, rect.y - 3f, 25f, 13f);
        Rect rect2 = new Rect(xMax - 29f, rect.y - 3f, 25f, 13f);

        plus = GUI.Button(rect1, EditorTools.iconToolbarPlus, EditorTools.preButtonStyle);
        EditorGUI.BeginDisabledGroup(!canRemove);
        minus = GUI.Button(rect2, EditorTools.iconToolbarMinus, EditorTools.preButtonStyle);
        EditorGUI.EndDisabledGroup();
    }

    public enum ToolbarType
    {
        Plus,
        Minus
    }

    static public bool DrawSinglebar(ToolbarType type)
    {
        Rect rect = GUILayoutUtility.GetRect(4f, 13f, new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true)
        });

        float xMax = rect.xMax;
        float num = xMax - 8f;
        num -= 25f;

        Rect rect1 = new Rect(num + 4, rect.y - 3f, 25f, 13f);

        GUIContent iconContent = null;
        switch (type)
        {
            case ToolbarType.Plus:
                iconContent = EditorTools.iconToolbarPlus;
                break;
            case ToolbarType.Minus:
                iconContent = EditorTools.iconToolbarMinus;
                break;
        }
        return GUI.Button(rect1, iconContent, EditorTools.preButtonStyle);
    }

    static public bool DrawSinglebar(ToolbarType type, Rect rect)
    {
        float xMax = rect.xMax;
        float num = xMax;
        num -= 25f;
        Rect rect1 = new Rect(num + 4, rect.y - 3f, 25f, 13f);
        GUIContent iconContent = null;
        switch (type)
        {
            case ToolbarType.Plus:
                iconContent = EditorTools.iconToolbarPlus;
                break;
            case ToolbarType.Minus:
                iconContent = EditorTools.iconToolbarMinus;
                break;
        }
        return GUI.Button(rect1, iconContent, EditorTools.preButtonStyle);
    }

    static public bool DrawFoldout(Rect position, string key, string text)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (state) text = "\u25B2" + text;
        else text = "\u25BC" + text;

        state = GUI.Toggle(position, state, text, headerStyle);

        EditorPrefs.SetBool(key, state);

        return state;
    }

    static public bool DrawFoldout(string key, string text)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (state) text = "\u25B2" + text;
        else text = "\u25BC" + text;

        state = GUILayout.Toggle(state, text, headerStyle);

        EditorPrefs.SetBool(key, state);

        return state;
    }

    static public void SetVector2(string key, Vector2 pos)
    {
        string xKey = string.Format("{0}{1}", "Vector2x", key);
        EditorPrefs.SetFloat(xKey, pos.x);
        string yKey = string.Format("{0}{1}", "Vector2y", key);
        EditorPrefs.SetFloat(yKey, pos.y);
    }

    static public Vector2 GetVector2(string key)
    {
        string xKey = string.Format("{0}{1}", "Vector2x", key);
        float x = EditorPrefs.GetFloat(xKey, 0f);
        string yKey = string.Format("{0}{1}", "Vector2y", key);
        float y = EditorPrefs.GetFloat(yKey, 0f);
        return new Vector2(x, y);
    }
    
}
