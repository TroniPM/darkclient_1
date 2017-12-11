/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DebugFilter
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using Mogo.Util;
using System;
public class DebugFilter : EditorWindow
{

    [MenuItem("DebugMsg/filter")]
    static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(100,50, 500, 100);
        DebugFilter window = (DebugFilter)EditorWindow.GetWindowWithRect(typeof(DebugFilter), wr, true, "DebugFilter");
        window.Show();
    }

    static private string m_filterStr = string.Empty;

    //绘制窗口时调用
    void OnGUI()
    {
        //输入框控件
        m_filterStr = EditorGUILayout.TextField("过滤标记:", m_filterStr);

        if (GUILayout.Button("设置", GUILayout.Width(100)))
        {
            //设置
            LoggerHelper.DebugFilterStr = m_filterStr;
            this.ShowNotification(new GUIContent("设置成功！"));
            this.Close();
        }

   

    }


    void OnFocus()
    {
        //Mogo.Util.LoggerHelper.Debug("当窗口获得焦点时调用一次");
    }

    void OnLostFocus()
    {
        //Mogo.Util.LoggerHelper.Debug("当窗口丢失焦点时调用一次");
    }

    void OnHierarchyChange()
    {
        //Mogo.Util.LoggerHelper.Debug("当Hierarchy视图中的任何对象发生改变时调用一次");
    }

    void OnProjectChange()
    {
        //Mogo.Util.LoggerHelper.Debug("当Project视图中的资源发生改变时调用一次");
    }

    void OnInspectorUpdate()
    {
        //Mogo.Util.LoggerHelper.Debug("窗口面板的更新");
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    void OnSelectionChange()
    {
        //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
        //foreach (Transform t in Selection.transforms)
        //{
        //    //有可能是多选，这里开启一个循环打印选中游戏对象的名称
        //    //Mogo.Util.LoggerHelper.Debug("OnSelectionChange" + t.name);
        //}
    }

    void OnDestroy()
    {
        //Mogo.Util.LoggerHelper.Debug("当窗口关闭时调用");
    }
}