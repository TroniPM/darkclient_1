#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.GameData;

public class LevelNoEnoughUIGrid : MogoUIBehaviour 
{
    private GameObject m_goLevelNoEnoughUIGridBtn;
    private UILabel m_lblLevelNoEnoughUIGridBtnText;
    private UILabel m_lblLevelNoEnoughUIGridTitle;
    private UISprite m_spLevelNoEnoughUIGridIconFG;

    private GameObject m_goGOLevelNoEnoughUIGridDesc;
    private UILabel m_lblDescText;
    private UILabel m_lblDescRequestLevel;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goLevelNoEnoughUIGridBtn = FindTransform("LevelNoEnoughUIGridBtn").gameObject;
        m_lblLevelNoEnoughUIGridBtnText = FindTransform("LevelNoEnoughUIGridBtnText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLevelNoEnoughUIGridTitle = FindTransform("LevelNoEnoughUIGridTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_spLevelNoEnoughUIGridIconFG = FindTransform("LevelNoEnoughUIGridIconFG").GetComponentsInChildren<UISprite>(true)[0];

        m_goGOLevelNoEnoughUIGridDesc = FindTransform("GOLevelNoEnoughUIGridDesc").gameObject;
        m_lblDescText = FindTransform("DescText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDescRequestLevel = FindTransform("DescRequestLevel").GetComponentsInChildren<UILabel>(true)[0];
    }

    private int m_index;
    public int Index
    {
        get { return m_index; }
        set
        {
            m_index = value;

            if (m_goLevelNoEnoughUIGridBtn != null)
            {
                LevelNoEnoughUIGridButton button = m_goLevelNoEnoughUIGridBtn.GetComponent<LevelNoEnoughUIGridButton>();
                if (button == null)
                    button = m_goLevelNoEnoughUIGridBtn.AddComponent<LevelNoEnoughUIGridButton>();

                button.Index = m_index;
            }
        }
    }
 
    public void SetLevelUpgradeGuideDetail(string title, string icon, string buttonName, string desc, int requestLevel)
    {
        if (MogoWorld.thePlayer.level >= requestLevel)
        {
            SetGridRequeseLevel("");
            ShowGridBtn(true);
        }
        else
        {
            SetGridRequeseLevel(string.Format(LanguageData.GetContent(47809), requestLevel));
            ShowGridBtn(false);
        }

        SetGridTitle(title);        
        SetGridIcon(icon);
        SetGridButtonName(buttonName);
        SetGridDesc(desc);
    }

    /// <summary>
    /// 是否显示按钮
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowGridBtn(bool isEnable)
    {
        if (m_goLevelNoEnoughUIGridBtn == null)
            m_goLevelNoEnoughUIGridBtn = FindTransform("LevelNoEnoughUIGridBtn").gameObject;


        UISprite spFG = m_goLevelNoEnoughUIGridBtn.transform.Find("LevelNoEnoughUIGridBtnBGUp").GetComponentsInChildren<UISprite>(true)[0];
        if (isEnable)
            spFG.spriteName = "btn_03up";
        else
            spFG.spriteName = "btn_03grey";

        m_goLevelNoEnoughUIGridBtn.transform.GetComponentsInChildren<BoxCollider>(true)[0].enabled = isEnable;
    }

    /// <summary>
    /// 设置按钮Text
    /// </summary>
    /// <param name="name"></param>
    private void SetGridButtonName(string name)
    {
        if (m_lblLevelNoEnoughUIGridBtnText == null)
            m_lblLevelNoEnoughUIGridBtnText = FindTransform("LevelNoEnoughUIGridBtnText").GetComponentsInChildren<UILabel>(true)[0];

        m_lblLevelNoEnoughUIGridBtnText.text = name;
    }

    /// <summary>
    /// 设置title
    /// </summary>
    /// <param name="title"></param>
    public void SetGridTitle(string title)
    {
        if (m_lblLevelNoEnoughUIGridTitle == null)
            m_lblLevelNoEnoughUIGridTitle = FindTransform("LevelNoEnoughUIGridTitle").GetComponentsInChildren<UILabel>(true)[0];

        m_lblLevelNoEnoughUIGridTitle.text = title;
    }

    /// <summary>
    /// 设置图标
    /// </summary>
    /// <param name="icon"></param>
    private void SetGridIcon(string icon)
    {
        if (m_spLevelNoEnoughUIGridIconFG == null)
            m_spLevelNoEnoughUIGridIconFG = FindTransform("LevelNoEnoughUIGridIconFG").GetComponentsInChildren<UISprite>(true)[0];

        MogoUIManager.Instance.TryingSetSpriteName(icon, m_spLevelNoEnoughUIGridIconFG);
        //m_spLevelNoEnoughUIGridIconFG.transform.localScale = new Vector3(150, 150, 1);
    }  

    /// <summary>
    /// 设置描述
    /// </summary>
    /// <param name="desc"></param>
    private void SetGridDesc(string desc)
    {
        if (m_lblDescText == null)
            m_lblDescText = FindTransform("DescText").GetComponentsInChildren<UILabel>(true)[0];

        m_lblDescText.text = desc;
    }

    public void SetGridRequeseLevel(string level)
    {
        if (m_lblDescRequestLevel == null)
            m_lblDescRequestLevel = FindTransform("DescRequestLevel").GetComponentsInChildren<UILabel>(true)[0];

        m_lblDescRequestLevel.text = level;
    }    
}
