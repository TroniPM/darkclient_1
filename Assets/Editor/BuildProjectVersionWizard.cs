using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System;
using System.Collections.Generic;

public class BuildProjectVersionWizard : EditorWindow
{
    public string currentVersion = "0.0.0.0";
    public string newVersion = "0.0.0.1";
    private bool selectAll = true;

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
        currentVersion = GUILayout.TextField(currentVersion);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("目标版本号：", GUILayout.Width(120f));
        newVersion = GUILayout.TextField(newVersion);
        GUILayout.EndHorizontal();

        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        GUILayout.Label("选择打包资源：", GUILayout.Width(120f));
        bool tempAll = selectAll;
        selectAll = GUILayout.Toggle(tempAll, "全选");
        GUILayout.EndHorizontal();
        if (ExportScenesManager.BuildResourcesInfoList != null)
            foreach (var item in ExportScenesManager.BuildResourcesInfoList)
            {
                NGUIEditorTools.DrawSeparator();
                GUILayout.BeginHorizontal();
                bool temp = item.check;
                bool hasChanged = false;
                if (selectAll != tempAll)
                    item.check = selectAll;
                item.check = GUILayout.Toggle(item.check, item.name);
                GUILayout.Label(item.type, GUILayout.Width(100f));
                foreach (var ex in item.extentions)
                {
                    GUILayout.Label(ex, GUILayout.Width(100f));
                }
                hasChanged = temp != item.check;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                foreach (var folder in item.folders)
                {
                    if (hasChanged)
                        folder.check = item.check;
                    folder.check = GUILayout.Toggle(folder.check, folder.path);
                }
                GUILayout.EndHorizontal();
            }

        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        GUILayout.Label("选择拷贝资源：", GUILayout.Width(120f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (ExportScenesManager.CopyResourcesInfoList != null)
            foreach (var item in ExportScenesManager.CopyResourcesInfoList)
            {
                item.check = GUILayout.Toggle(item.check, item.sourcePath);
            }
        GUILayout.EndHorizontal();

        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        bool create = GUILayout.Button("确定", GUILayout.Width(120f));
        GUILayout.EndHorizontal();
        if (create)
        {
            Export();
        }
    }

    private void Export()
    {
        ExportScenesManager.Export(currentVersion, newVersion);
    }
}

public class ChangedInfoWizard : EditorWindow
{
    public List<string> Infos = new List<string>();
    public Action OnOK;

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


        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        GUILayout.Label("变化的资源：", GUILayout.Width(120f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        foreach (var item in Infos)
        {
            GUILayout.Toggle(true, item);
        }
        GUILayout.EndHorizontal();

        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        bool ok = GUILayout.Button("确定", GUILayout.Width(120f));
        GUILayout.EndHorizontal();
        if (ok && OnOK != null)
        {
            OnOK();
        }
    }
}
