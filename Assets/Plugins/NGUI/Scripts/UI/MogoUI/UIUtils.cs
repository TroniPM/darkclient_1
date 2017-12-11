using System;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtils
{
    /// <summary>
    /// 美术出图尺寸
    /// </summary>
    public const float UI_SIZE = 1280;

    /// <summary>
    /// 1 / 1280
    /// 1 / camera.size = UI_SCALE 
    /// 配合美术出图尺寸
    /// vertex from localSpace to PerspectiveSpace / OrthicSpace fixed to 0 - 1
    /// 顶点从local坐标系转换到投影坐标系 尺寸规范化到 0 - 1
    /// </summary>
    public const float UI_SCALE = 1 / UI_SIZE;


    private static string GetFullName(this Transform myTransform, Transform currentTransform)
    {
        string fullName = String.Empty;

        while (currentTransform != myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != myTransform)
            {
                fullName = string.Concat("/", fullName);
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    public static void FillFullNameData(this Dictionary<string, string> dic, Transform myTransform, Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            dic.Add(rootTransform.GetChild(i).name, myTransform.GetFullName(rootTransform.GetChild(i)));
            dic.FillFullNameData(myTransform, rootTransform.GetChild(i));
        }
    }

    public static void FillFullNameData(this Dictionary<string, string> dic, Transform myTransform)
    {
        dic.FillFullNameData(myTransform, myTransform);
    }

    public static T GetFirstOrDefault<T>(this T[] list)
    {
        if (list == null || list.Length == 0)
            return default(T);
        else
            return list[0];
    }
}