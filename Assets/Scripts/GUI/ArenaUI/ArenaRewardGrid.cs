using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
public class ArenaRewardGrid : MogoParentUI
{
    public Action<int> clickHandler;
    public int RewardID = -1;

    private UISlicedSprite[] icon = new UISlicedSprite[3];
    private Transform m_Transform;

    private GameObject m_nameObj;
    private GameObject m_scoreObj;
    private GameObject m_progressObj;

    private GameObject m_alreadyGet;
    private GameObject m_goArenaUIListGridBtnOk;
    private UISprite m_spArenaUIListGridBtnOkBGUp;
    private GameObject m_goArenaUIListGridNOFinishText;

    void Awake()
    {
        m_Transform = transform;        
        m_nameObj = m_Transform.Find("ArenaUIListGridName").gameObject;
        m_scoreObj = m_Transform.Find("ArenaUIListGridNum").gameObject;
        m_progressObj = m_Transform.Find("ArenaUIListGridProgress").gameObject;
        icon = m_Transform.Find("item/itemIcon1").GetComponentsInChildren<UISlicedSprite>(true);

        m_alreadyGet = m_Transform.Find("ArenaUIListGridGet").gameObject;
        m_goArenaUIListGridBtnOk = m_Transform.Find("ArenaUIListGridBtnOk").gameObject;
        m_spArenaUIListGridBtnOkBGUp = m_Transform.Find("ArenaUIListGridBtnOk/ArenaUIListGridBtnOkBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_goArenaUIListGridNOFinishText = m_Transform.Find("ArenaUIListGridNOFinishText").gameObject;

        AddButtonListener("OnClicked", "ArenaUIListGridBtnOk", OnSingleButtonClicked);
    }

    void Start()
    {
        SetArenaUIListGridBtnState(m_isBtnEnable, m_isAlreadyGet);

        Name = Name;
        Icon = Icon;
        Color = Color;
    }

    #region 事件

    void OnSingleButtonClicked()
    {
        if (clickHandler != null)
        {
            clickHandler(RewardID);
        }
    }

    #endregion

    #region 界面信息

    /// <summary>
    /// 积分奖励名称
    /// </summary>
    private string m_name;
    public string Name
    {
        get { return m_name; }
        set
        {
            m_name = value;
            if (m_nameObj != null)
            {
                m_nameObj.GetComponent<UILabel>().text = value;
            }
        }
    }

    /// <summary>
    /// 设置积分奖励获得值，积分奖励进度
    /// </summary>
    /// <param name="rewardType"></param>
    /// <param name="needSocre"></param>
    /// <param name="obtainDayScore"></param>
    /// <param name="obtainWeekScore"></param>
    public void SetDetailInfo(int rewardType, int needSocre, int obtainDayScore, int obtainWeekScore, bool isAlreadyGet)
    {
        string obtainScoreStr = "";
        string progressStr = "";
        switch ((ArenaRewardType)rewardType)
        {
            case ArenaRewardType.RewardDay:
                {
                    obtainScoreStr = string.Format(LanguageData.GetContent(46750), needSocre);
                    progressStr = LanguageData.GetContent(46752) + obtainDayScore + "/" + needSocre;

                    if (obtainDayScore >= needSocre)
                        SetArenaUIListGridBtnState(true, isAlreadyGet);
                    else
                        SetArenaUIListGridBtnState(false, isAlreadyGet);
                } break;
            case ArenaRewardType.RewardWeek:
                {
                    obtainScoreStr = string.Format(LanguageData.GetContent(46751), needSocre);
                    progressStr = LanguageData.GetContent(46752) + obtainWeekScore + "/" + needSocre;

                    if (obtainWeekScore >= needSocre)
                        SetArenaUIListGridBtnState(true, isAlreadyGet);
                    else
                        SetArenaUIListGridBtnState(false, isAlreadyGet);
                } break;
        }

        m_scoreObj.GetComponent<UILabel>().text = obtainScoreStr;
        m_progressObj.GetComponent<UILabel>().text = progressStr;
    }  

    private List<string> m_icon = new List<string>();
    public List<string> Icon
    {
        get { return m_icon; }
        set
        {
            m_icon = value;
            for (int i = 0; i < icon.Length; i++)
            {
                if (i < m_icon.Count)
                {
                    if (icon[i] != null)
                    {
                        icon[i].atlas = MogoUIManager.Instance.GetAtlasByIconName(m_icon[i]);
                        icon[i].spriteName = m_icon[i];
                    }
                }
                else
                {
                    if (icon[i] != null)
                    {
                        icon[i].enabled = false;
                    }
                }
            }
        }
    }

    private List<int> m_color = new List<int>();
    public List<int> Color
    {
        get { return m_color; }
        set
        {
            m_color = value;

            if (m_color.Count > 0 && icon.Length > 0 && icon[0] != null)
                MogoUtils.SetImageColor(icon[0], m_color[0]);
        }
    }

    #endregion

    #region 设置领取按钮状态

    private bool m_isBtnEnable = false;
    private bool m_isAlreadyGet = false;
    /// <summary>
    /// 设置领取按钮状态
    /// </summary>
    /// <param name="isEnable"></param>
    private void SetArenaUIListGridBtnState(bool isEnable, bool isAlreadyGet)
    {
        m_isBtnEnable = isEnable;
        m_isAlreadyGet = isAlreadyGet;

        if (isEnable)
        {
            if (isAlreadyGet)
            {                
                ShowArenaUIListGridBtnOk(false);
                ShowArenaUIListGridNOFinishText(false);
                ShowArenaUIListGridAlreadyGet(true);
            }
            else
            {
                ShowArenaUIListGridAlreadyGet(false);
                ShowArenaUIListGridNOFinishText(false);
                ShowArenaUIListGridBtnOk(true);                
            }         
        }
        else
        {
            ShowArenaUIListGridAlreadyGet(false);
            ShowArenaUIListGridBtnOk(false);
            ShowArenaUIListGridNOFinishText(true);
        }
    }

    private void ShowArenaUIListGridAlreadyGet(bool isShow)
    {
        if (m_alreadyGet != null)
            m_alreadyGet.SetActive(isShow);       
    }

    private void ShowArenaUIListGridNOFinishText(bool isShow)
    {
        if (m_goArenaUIListGridNOFinishText != null)
            m_goArenaUIListGridNOFinishText.SetActive(isShow);
    }

    private void ShowArenaUIListGridBtnOk(bool isShow)
    {
        if (m_goArenaUIListGridBtnOk != null)
            m_goArenaUIListGridBtnOk.SetActive(isShow);
    }

    #endregion
}
