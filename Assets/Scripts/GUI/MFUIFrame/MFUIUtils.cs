using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct MFUIResult
{
    public bool isFailed;
    public object buffer;
}

public class MFUIUtils
{
    static private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public static void MFUIDebug(string text)
    {
        //Debug.Log(string.Concat("O(∩_∩)O~ Debug By MaiFeo : ", text));
    }

    private static void AddWigetToFullNameData(string widgetName, string fullName)
    {
        if (m_widgetToFullName.ContainsKey(widgetName))
        {
            MFUIUtils.MFUIDebug("Same Key In the WidgetName to FullName Dict ,Now Replace It!");
        }
        //m_widgetToFullName.Add(widgetName, fullName);
        m_widgetToFullName[widgetName] = fullName;
    }

    private static string GetFullName(Transform currentTransform,Transform rootTransform)
    {
        string fullName = "";

        while (currentTransform != rootTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != rootTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    public static void FillFullNameData(Transform rootTransform,Transform thisTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i), thisTransform));
            FillFullNameData(rootTransform.GetChild(i), thisTransform);
        }
    }

    public static string GetFullName(string name)
    {
        if (!m_widgetToFullName.ContainsKey(name))
        {
            MFUIDebug(string.Concat(name, " can not found a fullName !"));

            return "";
        }
        else
        {
            return m_widgetToFullName[name];
        }
    }

    public static void ShowGameObject(bool isShow,GameObject go)
    {
        go.SetActive(isShow);
    }

    public static void AttachWidget(Transform child, Transform parent,bool autoSet = true)
    {
        if (!autoSet)
            return;

        child.parent = parent;
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;
        child.transform.localEulerAngles = Vector3.zero;
    }

    public static MFUIResult SafeSetValue_W(object obj, System.Action act, object buffer)
    {
        MFUIResult result = new MFUIResult();

        if (obj != null)
        {
            if (act != null)
            {
                act();
                result.isFailed = false;
            }
            else
            {
                result.isFailed = true;
            }
        }
        else
        {
            result.isFailed = true;
        }

        result.buffer = buffer;
        return result;
    }
    public static object SafeSetValue(object obj, System.Action act,object buffer)
    {
        if (obj != null)
        {
            if (act != null)
            {
                act();
            }
            else
            {
                MFUIDebug("Action is NULL !");
            }
        }

        return buffer;
    }

    public static System.Action SafeDoAction(object obj, System.Action act)
    {

        if (obj != null)
        {
            if (act != null)
            {
                act();
            }
            else
            {
                MFUIDebug("Action is NULL !");
            }
        }

        return act;
    }

    public static void SetSpriteIcon(string iconName, UISprite sp)
    {
        sp.atlas = GetAtlasByIconName();

        sp.spriteName = iconName;
    }

    public static UIAtlas GetAtlasByIconName()
    {
        return new UIAtlas();
    }
}
