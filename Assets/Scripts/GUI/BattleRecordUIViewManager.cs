/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：BattleRecordUIViewManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：湮灭之门战斗记录
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;

public class BattleRecordUIViewManager : MogoUIParent
{
    static public BattleRecordUIViewManager Instance;

    UILabel m_lblTitle;
    UILabel m_lblOutputText;
    UILabel m_lblOutputNum;
    UILabel m_lblGoldText;
    UILabel m_lblGoldNum;
    UILabel m_lblExpText;
    //UILabel m_lblExpNum;

    void Awake()
    {

        base.Init();

        Instance = this;

        m_lblTitle = GetUIChild("BattleRecordTitleText").GetComponent<UILabel>();
        //m_lblExpNum = GetUIChild("BattleRecordExpNum").GetComponent<UILabel>();
        m_lblExpText = GetUIChild("BattleRecordExpText").GetComponent<UILabel>();
        m_lblGoldNum = GetUIChild("BattleRecordGoldNum").GetComponent<UILabel>();
        m_lblGoldText = GetUIChild("BattleRecordGoldText").GetComponent<UILabel>();
        m_lblOutputNum = GetUIChild("BattleRecordOutputNum").GetComponent<UILabel>();
        m_lblOutputText = GetUIChild("BattleRecordOutputText").GetComponent<UILabel>();

        GetUIChild("BattleRecordOKBtn").gameObject.AddComponent<MogoUIListener>().MogoOnClick = (() => 
        {
            EventDispatcher.TriggerEvent(Events.InstanceEvent.ReturnHome);
            transform.gameObject.SetActive(false); 
            

        });

        GetUIChild("BattleRecordTipText").GetComponent<UILabel>().text = Mogo.GameData.LanguageData.GetContent(180002); 

        gameObject.SetActive(false);
    }

    public void SetBattleRecordData(BattleRecordData data)
    {
        //m_lblExpNum.text = data.ExpNum;
        m_lblExpText.text = data.ExpText;
        m_lblGoldNum.text = data.GoldNum;
        m_lblGoldText.text = data.GoldText;
        m_lblOutputNum.text = data.OutputNum;
        m_lblOutputText.text = data.OutputText;
        m_lblTitle.text = data.Title;
    }

    public void ShowBattleRecord(bool isShow)
    {
        transform.gameObject.SetActive(isShow);
    }

}