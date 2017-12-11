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
        GUILayout.Label("��ǰ�汾�ţ�", GUILayout.Width(120f));
        v = GUILayout.TextField(v);
        GUILayout.EndHorizontal();

        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        bool create = GUILayout.Button("ȷ��", GUILayout.Width(120f));
        bool backup = GUILayout.Button("����", GUILayout.Width(120f));
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
