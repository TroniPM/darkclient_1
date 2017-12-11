using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.IO;
using System.Linq;

public class FindMissingScriptsRecursively : EditorWindow
{
    string info = "";
    Object[] selection;
    MissingScriptsCounter counter;
    Action<Component, int, GameObject> OnDoTask;
    Action OnTaskFinished;
    bool gap;
    System.Text.StringBuilder sb;

    [MenuItem("Window/Find Scripts Info Recursively")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
        ExportScenesManager.GetFindScriptIngoreList();
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            OnDoTask = (com, i, g) =>
            {
                if (com == null)
                {
                    var fullName = GetFullName(g);
                    counter.missingCount++;
                    //Mogo.Util.LoggerHelper.Debug(fullName + " has an empty script attached in position: " + i, g);
                    Debug.Log(fullName + " has an empty script attached in position: " + i, g);
                }
            };
            OnTaskFinished = () => { Mogo.Util.LoggerHelper.Debug(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", counter.goCount, counter.componentsCount, counter.missingCount)); };
            FindInSelected();
        }
        if (GUILayout.Button("Find Scripts in selected GameObjects"))
        {
            sb = new System.Text.StringBuilder();
            OnDoTask = (com, i, g) =>
            {
                var typeName = com.GetType().Name;
                if (com is MonoBehaviour && !ExportScenesManager.IngoreFiles.Contains(typeName))
                {
                    var fullName = GetFullName(g);
                    counter.missingCount++;
                    sb.AppendLine(String.Concat(fullName, ": ", typeName));
                }
            };
            OnTaskFinished = () =>
            {
                var path = ExportScenesManager.GetFolderPath("") + "//ScriptUsingInfo.txt";
                Mogo.Util.XMLParser.SaveText(path.Replace("\\", "/"), sb.ToString());
                Mogo.Util.LoggerHelper.Debug("Find finished, total script using is " + counter.missingCount + ", please check 'ScriptUsingInfo.txt' in project folder.");
            };
            FindInSelected();
        }
        if (GUILayout.Button("Find Null UISprite Scripts"))
        {
            sb = new System.Text.StringBuilder();
            OnDoTask = (com, i, g) =>
            {
                var typeName = com.GetType().Name;
                var spri = com as UISprite;
                if (spri != null)
                {
                    var fullName = GetFullName(g);
                    if (spri.atlas == null)
                    {
                        sb.AppendLine(String.Concat("atlas null: ", fullName, ": ", typeName));
                    }
                    else
                    {
                        if (spri.atlas.GetSprite(spri.spriteName) == null)
                        {
                            counter.missingCount++;
                            sb.AppendLine(String.Concat(fullName, ": ", typeName));
                        }
                    }
                }
            };
            OnTaskFinished = () =>
            {
                var path = ExportScenesManager.GetFolderPath("") + "//NullUISprite.txt";
                Mogo.Util.XMLParser.SaveText(path.Replace("\\", "/"), sb.ToString());
                Mogo.Util.LoggerHelper.Debug("Find finished, total Null UISprite is " + counter.missingCount + ", please check 'NullUISprite.txt' in project folder.");
            };
            FindInSelected();
        }

        GUILayout.Label(info);
    }

    void Update()
    {
        if (gap)
        {
            gap = false;
            return;
        }
        else
        {
            gap = true;
        }
        UpdateFindMissing();
    }

    private void UpdateFindMissing()
    {
        if (counter != null)
        {
            if (counter.currentIndex < selection.Length)
            {
                var g = selection[counter.currentIndex];
                FindInGO(g, counter);
                counter.currentIndex++;
                info = string.Format("{0}/{1}", counter.currentIndex, selection.Length);
            }
            else
            {
                OnTaskFinished();
                counter = null;
            }
            Repaint();
        }
    }

    private void FindInSelected()
    {
        selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        counter = new MissingScriptsCounter();
    }

    private void FindInGO(Object go, MissingScriptsCounter counter)
    {
        var g = go as GameObject;
        if (g == null)
            return;
        counter.goCount++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            counter.componentsCount++;
            OnDoTask(components[i], i, g);
            //if (components[i] == null)
            //{
            //    var fullName = GetFullName(g);
            //    counter.missingCount++;
            //    Mogo.Util.LoggerHelper.Debug(fullName + " has an empty script attached in position: " + i, g);
            //}
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Mogo.Util.LoggerHelper.Debug("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject, counter);
        }
    }

    public static string GetFullName(GameObject go)
    {
        string name = go.name;
        var tempGo = go.transform;

        while (tempGo.parent != null)
        {
            name = string.Concat(tempGo.parent.name, ".", name);
            tempGo = tempGo.parent;
        }
        return name;
    }
}

public class MissingScriptsCounter
{
    public int goCount { get; set; }
    public int componentsCount { get; set; }
    public int missingCount { get; set; }
    public int currentIndex { get; set; }
}