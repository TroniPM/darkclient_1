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

public class SpriteUISkillPiece : MogoUIBehaviour
{
    private UISprite m_spSpriteUIDetailSkillPieceIcon;
    private UISprite m_spSpriteUIDetailSkillPiecelBG;
    private UILabel m_lblSpriteUIDetailSkillPieceName;
    private GameObject m_goSpriteUIDetailSkillPieceNameBG;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_spSpriteUIDetailSkillPieceIcon = FindTransform("SpriteUIDetailSkillPieceIcon").GetComponentsInChildren<UISprite>(true)[0];
        m_spSpriteUIDetailSkillPiecelBG = FindTransform("SpriteUIDetailSkillPiecelBG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblSpriteUIDetailSkillPieceName = FindTransform("SpriteUIDetailSkillPieceName").GetComponentsInChildren<UILabel>(true)[0];
        m_goSpriteUIDetailSkillPieceNameBG = FindTransform("SpriteUIDetailSkillPieceNameBG").gameObject;
    }

    #region 界面信息

    private int m_index;
    public int Index
    {
        set
        {
            m_index = value;
            if (m_index == 0)
            {
                m_spSpriteUIDetailSkillPieceIcon.gameObject.SetActive(false);
                m_lblSpriteUIDetailSkillPieceName.gameObject.SetActive(false);
                m_goSpriteUIDetailSkillPieceNameBG.SetActive(false);
            }
            else if (m_index == 6)
            {
                m_spSpriteUIDetailSkillPieceIcon.gameObject.SetActive(false);
                m_spSpriteUIDetailSkillPiecelBG.gameObject.SetActive(false);
            }
        }
    }

    private bool m_isPieceAwake;
    public bool IsPieceAwake
    {
        get { return m_isPieceAwake; }
        set
        {
            m_isPieceAwake = value;

            if (m_isPieceAwake)
            {
                m_spSpriteUIDetailSkillPiecelBG.spriteName = "sxq_kq";
            }
            else
            {
                m_spSpriteUIDetailSkillPiecelBG.spriteName = "sxq_wkq";
            }
        }
    }

    public void SetSkillPiece(bool isAwake, string pieceName, string iconName)
    {
        IsPieceAwake = isAwake;
        m_lblSpriteUIDetailSkillPieceName.text = pieceName;
        //m_spSpriteUIDetailSkillPieceIcon.atlas = MogoUIManager.Instance.GetAtlasByIconName(iconName);
        m_spSpriteUIDetailSkillPieceIcon.spriteName = iconName;
    }

    #endregion
}
