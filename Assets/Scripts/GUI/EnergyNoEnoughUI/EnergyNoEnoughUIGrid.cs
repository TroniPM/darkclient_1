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

public class EnergyNoEnoughUIGrid : MogoUIBehaviour 
{
    private GameObject m_goEnergyNoEnoughUIGridBtn;
    private UILabel m_lblEnergyNoEnoughUIGridBtnText;
    private UILabel m_lblEnergyNoEnoughUIGridTitle;
    private UISprite m_spEnergyNoEnoughUIGridIconFG;

    // 购买体力
    private GameObject m_goGOEnergyNoEnoughUIGridDescBuy;
    private UILabel m_lblBuyText2;

    // 限时活动
    private GameObject m_goGOEnergyNoEnoughUIGridDescLimit;
    private UILabel m_lblLimitDesc;

    // 其他玩法
    private GameObject m_goGOEnergyNoEnoughUIGridDescOther;
    private UILabel m_lblOtherDesc;
    private UILabel m_lblOtherRequestLevel;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goEnergyNoEnoughUIGridBtn = FindTransform("EnergyNoEnoughUIGridBtn").gameObject;        
        m_lblEnergyNoEnoughUIGridBtnText = FindTransform("EnergyNoEnoughUIGridBtnText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblEnergyNoEnoughUIGridTitle = FindTransform("EnergyNoEnoughUIGridTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_spEnergyNoEnoughUIGridIconFG = FindTransform("EnergyNoEnoughUIGridIconFG").GetComponentsInChildren<UISprite>(true)[0];

        m_goGOEnergyNoEnoughUIGridDescBuy = FindTransform("GOEnergyNoEnoughUIGridDescBuy").gameObject;
        m_lblBuyText2 = FindTransform("BuyText2").GetComponentsInChildren<UILabel>(true)[0];

        m_goGOEnergyNoEnoughUIGridDescLimit = FindTransform("GOEnergyNoEnoughUIGridDescLimit").gameObject;
        m_lblLimitDesc = FindTransform("LimitDesc").GetComponentsInChildren<UILabel>(true)[0];

        m_goGOEnergyNoEnoughUIGridDescOther = FindTransform("GOEnergyNoEnoughUIGridDescOther").gameObject;
        m_lblOtherDesc = FindTransform("OtherDesc").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOtherRequestLevel = FindTransform("OtherRequestLevel").GetComponentsInChildren<UILabel>(true)[0];
    }

    private int m_index;
    public int Index
    {
        get { return m_index; }
        set
        {
            m_index = value;

            if (m_goEnergyNoEnoughUIGridBtn != null)
            {
                EnergyNoEnoughUIGridButton button = m_goEnergyNoEnoughUIGridBtn.GetComponent<EnergyNoEnoughUIGridButton>();
                if(button == null)
                    button = m_goEnergyNoEnoughUIGridBtn.AddComponent<EnergyNoEnoughUIGridButton>();

                button.Index = m_index;
            }
        }
    }

    public void ShowGridBtn(bool isEnable)
    {
        if(m_goEnergyNoEnoughUIGridBtn == null)
            m_goEnergyNoEnoughUIGridBtn = FindTransform("EnergyNoEnoughUIGridBtn").gameObject;

        UISprite spFG = m_goEnergyNoEnoughUIGridBtn.transform.Find("EnergyNoEnoughUIGridBtnBGUp").GetComponentsInChildren<UISprite>(true)[0];
        if (isEnable)
            spFG.spriteName = "btn_03up";
        else
            spFG.spriteName = "btn_03grey";

        m_goEnergyNoEnoughUIGridBtn.transform.GetComponentsInChildren<BoxCollider>(true)[0].enabled = isEnable;
    }

    public void ShowByType(EnergyNoEnoughUITab type, bool isShow = true)
    {
        if (m_goGOEnergyNoEnoughUIGridDescBuy == null)
            m_goGOEnergyNoEnoughUIGridDescBuy = FindTransform("GOEnergyNoEnoughUIGridDescBuy").gameObject;
        if (m_goGOEnergyNoEnoughUIGridDescLimit == null)
            m_goGOEnergyNoEnoughUIGridDescLimit = FindTransform("GOEnergyNoEnoughUIGridDescLimit").gameObject;
        if (m_goGOEnergyNoEnoughUIGridDescOther == null)
            m_goGOEnergyNoEnoughUIGridDescOther = FindTransform("GOEnergyNoEnoughUIGridDescOther").gameObject;

        m_goGOEnergyNoEnoughUIGridDescBuy.SetActive(false);
        m_goGOEnergyNoEnoughUIGridDescLimit.SetActive(false);
        m_goGOEnergyNoEnoughUIGridDescOther.SetActive(false);

        switch (type)
        {
            case EnergyNoEnoughUITab.BuyEnergyTab:
                {
                    m_goGOEnergyNoEnoughUIGridDescBuy.SetActive(isShow);
                    ShowGridBtn(true);
                }break;
            case EnergyNoEnoughUITab.LimitActivityTab:
                {
                    m_goGOEnergyNoEnoughUIGridDescLimit.SetActive(isShow);
                    ShowGridBtn(true);
                }break;
            case EnergyNoEnoughUITab.OtherSystemTab:
                {
                    m_goGOEnergyNoEnoughUIGridDescOther.SetActive(isShow);
                }break;
            default:
                break;
        }
    }

    /// <summary>
    /// 体力购买
    /// </summary>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    /// <param name="buttonName"></param>
    /// <param name="diamond"></param>
    /// <param name="addEnergy"></param>
    public void SetBuyEnergyDetail(string title, string icon, string buttonName, int diamond, int addEnergy)
    {
        if (m_lblBuyText2 == null)
            m_lblBuyText2 = FindTransform("BuyText2").GetComponentsInChildren<UILabel>(true)[0];        

        SetGridIcon(icon);
        SetGridTitle(title);
        SetGrildButtonName(buttonName);

        if (addEnergy > 0)
        {
            ShowByType(EnergyNoEnoughUITab.BuyEnergyTab);
            m_lblBuyText2.text = string.Format(LanguageData.GetContent(47808), diamond, addEnergy);
        }
        else
        {
            ShowByType(EnergyNoEnoughUITab.BuyEnergyTab, false);
        }
    }

    /// <summary>
    /// 限时活动
    /// </summary>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    /// <param name="buttonName"></param>
    /// <param name="limitDesc"></param>
    /// <param name="finished"></param>
    public void SetLimitActivityDetail(string title, string icon, string buttonName, string limitDesc, bool finished)
    {
        if (m_lblLimitDesc == null)
            m_lblLimitDesc = FindTransform("LimitDesc").GetComponentsInChildren<UILabel>(true)[0];

        ShowByType(EnergyNoEnoughUITab.LimitActivityTab);    
        ShowGridBtn(finished);

        SetGridIcon(icon);
        SetGridTitle(title);
        SetGrildButtonName(buttonName);

        m_lblLimitDesc.text = limitDesc;
    }

    /// <summary>
    /// 其他玩法
    /// </summary>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    /// <param name="buttonName"></param>
    /// <param name="desc"></param>
    /// <param name="requestLevel"></param>
    public void SetNoNeedEnergyDetail(string title, string icon, string buttonName, string desc, int requestLevel)
    {
        if (m_lblOtherRequestLevel == null)
            m_lblOtherRequestLevel = FindTransform("OtherRequestLevel").GetComponentsInChildren<UILabel>(true)[0];
        if (m_lblOtherDesc == null)
            m_lblOtherDesc = FindTransform("OtherDesc").GetComponentsInChildren<UILabel>(true)[0];

        ShowByType(EnergyNoEnoughUITab.OtherSystemTab);
        if (MogoWorld.thePlayer.level >= requestLevel)
        {           
            m_lblOtherRequestLevel.text = "";
            ShowGridBtn(true);
        }
        else
        {
            m_lblOtherRequestLevel.text = string.Format(LanguageData.GetContent(47809), requestLevel);
            ShowGridBtn(false);
        }

        SetGridIcon(icon);
        SetGridTitle(title);
        SetGrildButtonName(buttonName);

        m_lblOtherDesc.text = desc;
    }

    /// <summary>
    /// 设置活动ICON
    /// </summary>
    /// <param name="icon"></param>
    private void SetGridIcon(string icon)
    {
        if (m_spEnergyNoEnoughUIGridIconFG == null)
            m_spEnergyNoEnoughUIGridIconFG = FindTransform("EnergyNoEnoughUIGridIconFG").GetComponentsInChildren<UISprite>(true)[0];

        if (m_spEnergyNoEnoughUIGridIconFG.atlas != null && m_spEnergyNoEnoughUIGridIconFG.atlas.GetSprite(icon) != null)
        {
            m_spEnergyNoEnoughUIGridIconFG.spriteName = icon;
            m_spEnergyNoEnoughUIGridIconFG.transform.localScale = new Vector3(128, 128, 1);
        }
        else
        {
            m_spEnergyNoEnoughUIGridIconFG.atlas = MogoUIManager.Instance.GetAtlasByIconName(icon);
            m_spEnergyNoEnoughUIGridIconFG.spriteName = icon;
            m_spEnergyNoEnoughUIGridIconFG.transform.localScale = new Vector3(150, 150, 1);
        }        
    }

    /// <summary>
    /// 设置活动title
    /// </summary>
    /// <param name="title"></param>
    private void SetGridTitle(string title)
    {
        if (m_lblEnergyNoEnoughUIGridTitle == null)
            m_lblEnergyNoEnoughUIGridTitle = FindTransform("EnergyNoEnoughUIGridTitle").GetComponentsInChildren<UILabel>(true)[0];
       
        m_lblEnergyNoEnoughUIGridTitle.text = title;    
    }

    /// <summary>
    /// 设置按钮名称
    /// </summary>
    /// <param name="name"></param>
    private void SetGrildButtonName(string name)
    {
        if (m_lblEnergyNoEnoughUIGridBtnText == null)
            m_lblEnergyNoEnoughUIGridBtnText = FindTransform("EnergyNoEnoughUIGridBtnText").GetComponentsInChildren<UILabel>(true)[0];

        m_lblEnergyNoEnoughUIGridBtnText.text = name;
    }
}
