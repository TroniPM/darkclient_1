using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System;
using System.Collections.Generic;

public class PackWizard : EditorWindow
{
    string v = "0.0.0.";
    /// <summary>
    /// Refresh the window on selection.
    /// </summary>
    void OnSelectionChange() { Repaint(); }

    /// <summary>
    /// Draw the custom wizard.
    /// </summary>
    void OnGUI()
    {
        EditorGUIUtility.LookLikeControls(80f);

        GUILayout.BeginHorizontal();
        GUILayout.Label("当前版本号：", GUILayout.Width(120f));
        v = GUILayout.TextField(v);
        GUILayout.EndHorizontal();

        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        bool create = GUILayout.Button("确定", GUILayout.Width(120f));
        bool backup = GUILayout.Button("备份", GUILayout.Width(120f));
        GUILayout.EndHorizontal();
        if (create)
        {
            ExportScenesManager.PackManually(new VersionCodeInfo(v));
        }
        if (backup)
        {
            ExportScenesManager.BackupVersion(new VersionCodeInfo(v));
        }
    }
}
