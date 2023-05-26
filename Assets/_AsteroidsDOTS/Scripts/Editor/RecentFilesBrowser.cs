using System.IO;
using _AsteroidsDOTS.Scripts.Editor;
using UnityEditor;
using UnityEngine;

public class RecentFilesBrowser : EditorWindow
{
    private const string USER_FACING_NAME = "Recent Files Browser";

    private static Texture _favoriteTexture;
    private static SerializedObject _serializedData;

    private readonly string _subDirectory = Path.Combine("_asteroidsDOTS", "Editor", "untracked");
    private string SaveDirectory => Path.Combine("Assets", _subDirectory);
    private string SavePath => Path.Combine(SaveDirectory, "RecentFilesBrowserData.asset");

    private SerializedObject Data => InitData();

    private SerializedProperty FavoriteSelections => Data.FindProperty("favoriteSelections");
    private SerializedProperty PreviousSelections => Data.FindProperty("previousSelections");
    private SerializedProperty MaxItems => Data.FindProperty("maxItems");
    private SerializedProperty ScrollPos => Data.FindProperty("scrollPos");

    [MenuItem("Tools/" + USER_FACING_NAME)]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(RecentFilesBrowser));
        window.minSize = new Vector2(100, 80);
        window.maxSize = new Vector2(400, 800);
        window.titleContent = new GUIContent(USER_FACING_NAME);
        window.Show();
    }

    private SerializedObject InitData()
    {
        if (_serializedData?.targetObject == null)
        {
            RecentFilesBrowserData data = AssetDatabase.LoadAssetAtPath<RecentFilesBrowserData>(SavePath);
            if (data == null)
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, _subDirectory));
                data = CreateInstance<RecentFilesBrowserData>();
                AssetDatabase.CreateAsset(data, SavePath);
                AssetDatabase.SaveAssets();
            }

            if (_serializedData?.targetObject != data)
            {
                _serializedData = new SerializedObject(data);
            }

            CleanUpInvalidSelections();
        }

        return _serializedData;
    }

    private void OnGUI()
    {
        if (!_favoriteTexture)
        {
            _favoriteTexture = EditorGUIUtility.IconContent("d_Favorite").image;
        }

        ScrollPos.vector2Value = EditorGUILayout.BeginScrollView(ScrollPos.vector2Value);

        DrawFavoriteSelections();
        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        if (PreviousSelections.arraySize == 0)
        {
            DrawInstructions();
        }
        else
        {
            DrawSelections();
        }

        EditorGUILayout.EndScrollView();

        DrawControls();

        Data.ApplyModifiedProperties();
    }

    private void DrawInstructions()
    {
        EditorGUILayout.BeginVertical("HelpBox");

        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.wordWrap = true;
        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField($"Items selected in the Project window will show up here.", labelStyle);
        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        EditorGUILayout.EndVertical();
    }

    private void DrawControls()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField($"Max Items: {MaxItems.intValue}", GUILayout.MaxWidth(80));

        if (GUILayout.Button("+", GUILayout.MaxWidth(40)))
        {
            MaxItems.intValue++;
        }

        EditorGUI.BeginDisabledGroup(MaxItems.intValue == 0);

        if (GUILayout.Button("-", GUILayout.MaxWidth(40)))
        {
            MaxItems.intValue--;
            while (PreviousSelections.arraySize > MaxItems.intValue)
            {
                PreviousSelections.RemoveAt(0);
            }
        }

        EditorGUI.EndDisabledGroup();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFavoriteSelections()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Clean Up", GUILayout.Width(80)))
        {
            CleanUpInvalidSelections();
        }

        if (FavoriteSelections.arraySize > 0 && GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            ClearFavoriteSelections();
        }

        EditorGUILayout.EndHorizontal();
        for (int index = FavoriteSelections.arraySize - 1; index >= 0; index--)
        {
            Object selection = FavoriteSelections.ElementAt(index).objectReferenceValue;
            DrawSelection(selection, true);
        }
    }

    private void DrawSelections()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (PreviousSelections.arraySize > 0 && GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            ClearSelections();
        }

        EditorGUILayout.EndHorizontal();

        for (int index = PreviousSelections.arraySize - 1; index >= 0; index--)
        {
            Object selection = PreviousSelections.ElementAt(index).objectReferenceValue;
            DrawSelection(selection, false);
        }
    }

    private void DrawSelection(Object selection, bool isFavorite)
    {
        EditorGUILayout.BeginHorizontal();

        GUIStyle iconStyle = new GUIStyle(EditorStyles.label);
        iconStyle.alignment = TextAnchor.MiddleCenter;

        Color guiColor = GUI.color;
        Color baseColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        GUI.color = isFavorite ? baseColor : baseColor * 0.5f;
        if (GUILayout.Button(_favoriteTexture, EditorStyles.label, GUILayout.Width(20)))
        {
            if (isFavorite)
            {
                UnfavoriteSelection(selection);
            }
            else
            {
                FavoriteSelection(selection);
            }
        }

        GUI.color = guiColor;
        EditorGUILayout.ObjectField(selection, typeof(Object), true);

        EditorGUILayout.EndHorizontal();
    }

    private void OnSelectionChange()
    {
        if (!Selection.activeObject || !AssetDatabase.Contains(Selection.activeObject))
        {
            return;
        }

        AddPreviousSelection(Selection.activeObject);
        Data.ApplyModifiedProperties();

        Repaint();
    }

    private void AddPreviousSelection(Object selection)
    {
        PreviousSelections.Remove(selection);
        if (PreviousSelections.arraySize >= MaxItems.intValue)
        {
            PreviousSelections.RemoveAt(0);
        }

        PreviousSelections.Add(selection);
    }

    private void FavoriteSelection(Object selection)
    {
        if (selection != null)
        {
            FavoriteSelections.Add(selection);
            PreviousSelections.Remove(selection);
        }
    }

    private void UnfavoriteSelection(Object selection)
    {
        FavoriteSelections.Remove(selection);
        if (selection != null)
        {
            AddPreviousSelection(selection);
        }
    }

    private void ClearSelections()
    {
        PreviousSelections.Clear();
    }

    private void ClearFavoriteSelections()
    {
        FavoriteSelections.Clear();
    }

    private void CleanUpInvalidSelections()
    {
        int count = Mathf.Max(PreviousSelections.arraySize, FavoriteSelections.arraySize);
        for (int i = 0; i < count; i++)
        {
            if (i < PreviousSelections.arraySize && PreviousSelections.ElementAt(i).objectReferenceValue == null)
            {
                PreviousSelections.RemoveAt(i);
            }

            if (i < FavoriteSelections.arraySize && FavoriteSelections.ElementAt(i).objectReferenceValue == null)
            {
                FavoriteSelections.RemoveAt(i);
            }
        }
    }
}