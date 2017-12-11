#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：UILogicManager
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.5.2
// 模块描述：UI逻辑绑定管理。
//----------------------------------------------------------------*/
#endregion

using Mogo.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MogoUIBehaviour : MFUIUnit
{
    protected UILogicManager m_uiLoginManager;

    protected virtual void OnEnable()
    {
        LoggerHelper.Debug(gameObject.name + " OnEnable........");
        if (m_uiLoginManager != null)
        {
            LoggerHelper.Debug(gameObject.name + " UpdateUI........");
            m_uiLoginManager.UpdateUI();
        }
    }

    #region 控件字典

    protected Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    /// <summary>
    /// 递归遍历控件，填充控件字典
    /// </summary>
    /// <param name="rootTransform"></param>
    protected void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }    

    /// <summary>
    /// 填充控件字典
    /// </summary>
    /// <param name="widgetName"></param>
    /// <param name="fullName"></param>
    private void AddWigetToFullNameData(string widgetName, string fullName)
    {
        if (m_widgetToFullName.ContainsKey(widgetName))
            LoggerHelper.Debug(widgetName);

        m_widgetToFullName.Add(widgetName, fullName);
    }

    /// <summary>
    /// 获取该控件的完整路径名
    /// </summary>
    /// <param name="currentTransform"></param>
    /// <returns></returns>
    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;
            if (currentTransform.parent != m_myTransform)
                fullName = "/" + fullName;

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    protected Transform FindTransform(string transformName)
    {
        if (m_widgetToFullName.ContainsKey(transformName))
            return m_myTransform.Find(m_widgetToFullName[transformName]);
        return null;
    }

    #endregion   
}

/// <summary>
/// UI逻辑绑定管理。
/// </summary>
public abstract class UILogicManager
{
    private HashSet<INotifyPropChanged> m_itemSources = new HashSet<INotifyPropChanged>();
    private EventController m_eventController;

    /// <summary>
    /// 绑定数据源
    /// </summary>
    public INotifyPropChanged ItemSource
    {
        set
        {
            if (value != null && !m_itemSources.Contains(value))
            {
                m_itemSources.Add(value);
                value.SetEventController(m_eventController);
                value.AddUnloadCallback(() =>
                {
                    if (m_itemSources != null && m_itemSources.Contains(value))
                    {
                        m_itemSources.Remove(value);
                    }
                });
            }
        }
    }

    /// <summary>
    /// 默认构造函数。
    /// </summary>
    public UILogicManager()
    {
        m_eventController = new EventController();
    }

    /// <summary>
    /// 设置绑定。
    /// </summary>
    /// <typeparam name="T">绑定参数类型</typeparam>
    /// <param name="key">绑定关键字</param>
    /// <param name="action">绑定调用方法</param>
    public void SetBinding<T>(String key, Action<T> action)
    {
        if (m_eventController.ContainsEvent(key))
            return;
        m_eventController.AddEventListener(key, action);
    }

    /// <summary>
    /// 根据数据源更新UI。（效率不高，不要频繁调用）
    /// </summary>
    public void UpdateUI()
    {
        foreach (var itemSource in m_itemSources)
        {
            if (itemSource != null)
            {
                var type = itemSource.GetType();
                //获取带一个参数回调方法的TriggerEvent。
                var mTriggerEvent = m_eventController.GetType().GetMethods().FirstOrDefault(t => t.Name == "TriggerEvent" && t.IsGenericMethod && t.GetGenericArguments().Length == 1);
                foreach (var item in m_eventController.TheRouter)
                {
                    var prop = type.GetProperty(item.Key);
                    if (prop == null)
                        continue;
                    var method = mTriggerEvent.MakeGenericMethod(prop.PropertyType);//对泛型进行类型限定
                    var value = prop.GetGetMethod().Invoke(itemSource, null);//获取参数值
                    method.Invoke(m_eventController, new object[] { item.Key, value });//调用对应参数类型的触发方法
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Release()
    {
        // 不注释：清空属性监听, 需要在UI重新Load的时候重新设置属性监听ItemSource
        // 注释：保留属性监听，不需要重新设置ItemSource
        foreach (var item in m_itemSources)
        {
            if (item != null)
                item.RemoveEventController(m_eventController);
        }
        m_itemSources.Clear(); // 如果RemoveEventController，需要把m_itemSources列表同时清空，否则无法重新设置EventController

        // 需要清空，在UI重新Load的时候重新添加监听
        m_eventController.Cleanup();
    }
}

/// <summary>
/// 向客户端发出某一属性值已更改的通知。
/// </summary>
public interface INotifyPropChanged
{
    /// <summary>
    /// 设置事件控制器。
    /// </summary>
    /// <param name="controller"></param>
    void SetEventController(EventController controller);

    /// <summary>
    /// 移除事件控制器。
    /// </summary>
    /// <param name="controller"></param>
    void RemoveEventController(EventController controller);

    /// <summary>
    /// 在更改属性值时发生。
    /// </summary>
    /// <typeparam name="T">属性类型。</typeparam>
    /// <param name="propertyName">属性名称。</param>
    /// <param name="value">属性值。</param>
    void OnPropertyChanged<T>(string propertyName, T value);

    /// <summary>
    /// 监听实体资源释放回调。
    /// </summary>
    /// <param name="onUnload">回调事件处理</param>
    void AddUnloadCallback(Action onUnload);
}

public abstract class NotifyPropChanged
{
    private HashSet<EventController> m_uiBindingSet = new HashSet<EventController>();
    private Action m_onUnload;

    /// <summary>
    /// 添加UI事件绑定。
    /// </summary>
    /// <param name="controller"></param>
    public void SetEventController(EventController controller)
    {
        m_uiBindingSet.Add(controller);
    }

    /// <summary>
    /// 移除UI事件绑定。
    /// </summary>
    /// <param name="controller"></param>
    public void RemoveEventController(EventController controller)
    {
        m_uiBindingSet.Remove(controller);
    }

    /// <summary>
    /// 属性值变化处理。
    /// </summary>
    /// <typeparam name="T">属性值类型。</typeparam>
    /// <param name="propertyName">属性名称。</param>
    /// <param name="value">属性值。</param>
    public void OnPropertyChanged<T>(string propertyName, T value)
    {
        foreach (var item in m_uiBindingSet)
        {
            if (item != null)
                item.TriggerEvent(propertyName, value);
        }
    }

    /// <summary>
    /// 监听实体资源释放回调。
    /// </summary>
    /// <param name="onUnload">回调事件处理</param>
    public void AddUnloadCallback(Action onUnload)
    {
        m_onUnload = onUnload;
    }

    /// <summary>
    /// 清理绑定数据。
    /// </summary>
    protected void ClearBinding()
    {
        if (m_onUnload != null)
            m_onUnload();
        m_uiBindingSet.Clear();
    }
}