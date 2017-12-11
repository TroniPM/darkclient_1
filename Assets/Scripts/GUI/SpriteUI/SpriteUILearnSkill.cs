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

public class SpriteUILearnSkill : MogoUIBehaviour
{
    private GameObject m_goSpriteUILearnSkillFlag;
    private UILabel m_lblSpriteUILearnSkillName;
    private UISprite m_spSpriteUILearnSkillIcon;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goSpriteUILearnSkillFlag = FindTransform("SpriteUILearnSkillFlag").gameObject;
        m_lblSpriteUILearnSkillName = FindTransform("SpriteUILearnSkillName").GetComponentsInChildren<UILabel>(true)[0];
        m_spSpriteUILearnSkillIcon = FindTransform("SpriteUILearnSkillIcon").GetComponentsInChildren<UISprite>(true)[0];
    }

    #region 事件

    private int m_index;
    public int Index
    {
        get { return m_index; }
        set
        {
            m_index = value;
        }
    }

    void OnClick()
    {
        if (SpriteUIViewManager.Instance != null && SpriteUIViewManager.Instance.SPRITEUILEARNSKILLCHOOSEUP != null)
        {
            SpriteUIViewManager.Instance.SPRITEUILEARNSKILLCHOOSEUP(Index);
            SpriteUIViewManager.Instance.SpriteLearnUISpriteSkillIndex = Index;
        }
    }

    #endregion

    #region 界面信息

    /// <summary>
    /// 设置技能名称
    /// </summary>
    /// <param name="name"></param>
    public void SetLearnSkillName(string name)
    {
        m_lblSpriteUILearnSkillName.text = name;
    }

    /// <summary>
    /// 技能是否装备
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowLearnSkillFlag(bool isShow)
    {
        m_goSpriteUILearnSkillFlag.SetActive(isShow);
    }

    /// <summary>
    /// 设置接能图标
    /// </summary>
    /// <param name="spriteName"></param>
    public void SetLearnSkillIcon(string spriteName, bool hasLearned)
    {
        //MogoUIManager.Instance.TryingSetSpriteName(spriteName, m_spSpriteUILearnSkillIcon);
		//m_spSpriteUILearnSkillIcon.atlas = MogoUIManager.Instance.GetAtlasByIconName(spriteName);
		m_spSpriteUILearnSkillIcon.spriteName = spriteName;
        m_spSpriteUILearnSkillIcon.ShowAsWhiteBlack(false);
        m_spSpriteUILearnSkillIcon.ShowAsWhiteBlack(!hasLearned);
    }

    #endregion

}
