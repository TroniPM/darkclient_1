using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BattleMenuUIViewManager : MonoBehaviour 
{
    private static BattleMenuUIViewManager m_instance;

    public static BattleMenuUIViewManager Instance
    {
        get
        {
          

            return BattleMenuUIViewManager.m_instance;
        }
    }

    private Transform m_myTransform;

    //public static Dictionary<string, Action> ButtonTypeToEventUp = new Dictionary<string, Action>();

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public Action QUITINSTANCEBUTTONUP;
    public Action BATTLEMENUUICLOSEUP;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    void OnQuitInstanceButtonUp()
    {
        if (QUITINSTANCEBUTTONUP != null)
            QUITINSTANCEBUTTONUP();

        MainUIViewManager.Instance.ResetUIData();
    }

    void OnBattleMenuUICloseUp()
    {
        if (BATTLEMENUUICLOSEUP != null)
            BATTLEMENUUICLOSEUP();

        MogoUIManager.Instance.ShowMogoBattleMainUI();
        //if (MainUILogicManager.Instance.hasShowBossBlood)
        //    MainUIViewManager.Instance.ShowBossTarget(true);
    }

    void Awake()
    {

     

        m_myTransform = transform;

        m_instance = m_myTransform.GetComponentsInChildren<BattleMenuUIViewManager>(true)[0];

        m_myTransform.Find("TopRightBattleMenuUI").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        FillFullNameData(m_myTransform);

        Initialize();
    }

    void Initialize()
    {

        BattleMenuUILogicManager.Instance.Initialize();

        BattleMenuUIDict.ButtonTypeToEventUp.Add("QuitInstanceButton", OnQuitInstanceButtonUp);
        BattleMenuUIDict.ButtonTypeToEventUp.Add("BattleMenuUIClose", OnBattleMenuUICloseUp);
    }

    public void Release()
    {
        BattleMenuUILogicManager.Instance.Release();

        BattleMenuUIDict.ButtonTypeToEventUp.Clear();
    }

}
