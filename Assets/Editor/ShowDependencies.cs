using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ShowDependencies : EditorWindow
{
    private Vector2 scrollPos;

    [MenuItem("Assets/Show Dependencies")]
    public static void Init()
    {
        GetWindow<ShowDependencies>();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();
        DrawDependencies();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void DrawDeepHierarchy()
    {
        GUILayout.Label("DeepHierarchy:");
        var dependencies = EditorUtility.CollectDeepHierarchy(new[] { Selection.activeObject });
        foreach (var obj in dependencies)
        {
            GUILayout.Label(AssetDatabase.GetAssetPath(obj) + "  :  " + obj);
        }
    }

    private void DrawDependencies()
    {
        GUILayout.Label("Dependencies:");
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var dependencies =AssetDatabase.GetDependencies(new string[] { path }).Where(x=>x.EndsWith(".cs")==false);
        foreach (var obj in dependencies)
            GUILayout.Label(obj);
    }
}
