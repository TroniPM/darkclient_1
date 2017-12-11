using Mogo.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PluginCallback
{
    private static PluginCallback m_instance;

    public static PluginCallback Instance
    {
        get { return m_instance; }
    }

    public Action<bool> ShowGlobleLoadingUI;
    public Action<int> SetLoadingStatus;
    public Action<string> SetLoadingStatusTip;

    public Action<string, Action<bool>> ShowRetryMsgBox;
    public Action<string, string, string, Action<bool>> ShowMsgBox;
    public Action Hide;
    public Func<bool> IsHide;

    private PluginCallback() { }

    static PluginCallback()
    {
        m_instance = new PluginCallback();
    }
}

public class DriverLib : MonoBehaviour
{
    private static DriverLib m_instance;
    public static String FileName
    {
        get
        {
            return "MogoRes";
        }
    }

    public static DriverLib Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        m_instance = this;
    }

    public static void Invoke(Action action)
    {
        TimerHeap.AddTimer(0, 0, action);
    }
}

public class MogoForwardLoadingUIData
{
    public string tip;
    public string logoImg;
    public int status;
    public string animImg;
}

public class DefaultLanguageData
{
    public int id { get; set; }
    public string content { get; set; }

    public DefaultLanguageData()
    {
        content = String.Empty;
    }
}

public class DefaulSoundData
{
    public int id { get; set; }
    public string path { get; set; }

    public DefaulSoundData()
    {
        path = String.Empty;
    }
}

public class DefaultUI
{
    public static Dictionary<int, DefaultLanguageData> dataMap = new Dictionary<int, DefaultLanguageData>();
    public static Dictionary<int, DefaulSoundData> soundDataMap = new Dictionary<int, DefaulSoundData>();

    public static void InitLanguageInfo()
    {
        var textAsset = Resources.Load("Default/DefaultXml/DefaultChineseData") as TextAsset;
        if (textAsset)
        {
            var xml = XMLParser.LoadXML(textAsset.text);
            if (xml != null)
            {
                var map = XMLParser.LoadIntMap(xml, "DefaultLanguage");
                foreach (var item in map)
                {
                    var t = new DefaultLanguageData();
                    t.id = item.Key;
                    t.content = item.Value["content"];
                    dataMap.Add(item.Key, t);
                }
            }
            else
                LoggerHelper.Error("Default Language xml null.");
        }
        else
            LoggerHelper.Error("Default Language textAsset null.");
    }

    public static void InitSoundInfo()
    {
        var textAsset = Resources.Load("Default/DefaultSoundData") as TextAsset;
        if (textAsset)
        {
            var xml = XMLParser.LoadXML(textAsset.text);
            if (xml != null)
            {
                var map = XMLParser.LoadIntMap(xml, "DefaultLanguage");
                foreach (var item in map)
                {
                    var t = new DefaulSoundData();
                    t.id = item.Key;
                    t.path = item.Value["path"];
                    soundDataMap.Add(item.Key, t);
                }
            }
            else
                LoggerHelper.Error("Default Sound xml null.");
        }
        else
            LoggerHelper.Error("Default Sound textAsset null.");
    }

    public static void ShowLoading()
    {
        if (PluginCallback.Instance.ShowGlobleLoadingUI != null)
            PluginCallback.Instance.ShowGlobleLoadingUI(true);
        //if (MogoForwardLoadingUIManager.Instance)
        //    MogoForwardLoadingUIManager.Instance.ShowGlobleLoadingUI(true);
    }


    public static void Loading(int progress)
    {
        if (PluginCallback.Instance.ShowGlobleLoadingUI != null)
            PluginCallback.Instance.SetLoadingStatus(progress);
        //if (MogoForwardLoadingUIManager.Instance)
        //    MogoForwardLoadingUIManager.Instance.SetLoadingStatus(progress);
    }

    public static void HideLoading()
    {
        if (PluginCallback.Instance.ShowGlobleLoadingUI != null)
            PluginCallback.Instance.ShowGlobleLoadingUI(false);
        //if (MogoForwardLoadingUIManager.Instance)
        //    MogoForwardLoadingUIManager.Instance.ShowGlobleLoadingUI(false);
    }

    public static void SetLoadingStatusTip(object tip, params object[] args)
    {
        if (PluginCallback.Instance.SetLoadingStatusTip != null)
            PluginCallback.Instance.SetLoadingStatusTip(String.Format(tip.ToString(), args));
        //if (MogoForwardLoadingUIManager.Instance)
        //    MogoForwardLoadingUIManager.Instance.SetLoadingStatusTip(String.Format(tip.ToString(), args));
    }
}

public class ForwardLoadingMsgBoxLib
{
    private static ForwardLoadingMsgBoxLib m_instance;

    public static ForwardLoadingMsgBoxLib Instance
    {
        get { return ForwardLoadingMsgBoxLib.m_instance; }
    }

    private ForwardLoadingMsgBoxLib() { }

    static ForwardLoadingMsgBoxLib()
    {
        m_instance = new ForwardLoadingMsgBoxLib();
    }

    public void ShowRetryMsgBox(string content, Action<bool> onClick)
    {
        if (PluginCallback.Instance.ShowRetryMsgBox != null)
            PluginCallback.Instance.ShowRetryMsgBox(content, onClick);
    }

    public void ShowMsgBox(string okText, string cancelText, string content, Action<bool> onClick)
    {
        if (PluginCallback.Instance.ShowMsgBox != null)
            PluginCallback.Instance.ShowMsgBox(okText, cancelText, content, onClick);
        //Debug.LogError("ShowMsgBox");
        //m_msgBox.gameObject.SetActive(true);
        //m_msgBox.SetBoxText(content);
        //m_msgBox.SetOKBtnText(okText);
        //m_msgBox.SetCancelBtnText(cancelText);
        //m_msgBox.SetCallback(onClick);
        //ishide = false;
    }
    public void Hide()
    {
        if (PluginCallback.Instance.Hide != null)
            PluginCallback.Instance.Hide();
        //m_msgBox.gameObject.SetActive(false);
        //ishide = true;
    }
    public bool IsHide()
    {
        if (PluginCallback.Instance.IsHide != null)
            return PluginCallback.Instance.IsHide();
        else
            return false;
    }
}