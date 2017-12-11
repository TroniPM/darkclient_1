using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
public class RewardGridTip : MonoBehaviour
{
    public int TipNum;
    Transform m_myTransform;

    UILabel[] m_arrLblReward;

    UILabel m_lblTipTitle;

    //string[] m_arrStrReward = new string[4];

    string m_strTipTitle;

    public string TipTitle
    {
        get { return m_strTipTitle; }
        set
        {
            m_strTipTitle = value;

            if (m_lblTipTitle != null)
            {
                m_lblTipTitle.text = m_strTipTitle;
            }
        }
    }

    private List<string> m_listReward = new List<string>();

    public List<string> ListReward
    {
        get { return m_listReward; }
        set
        {
            m_listReward = value;
            if (m_arrLblReward == null)
            {
                m_arrLblReward = new UILabel[TipNum];
            }
            if (m_lblTipTitle != null)
            {
                for (int i = 0; i < TipNum; ++i)
                {
                    if (i < m_listReward.Count)
                    {
                        m_arrLblReward[i].transform.parent.gameObject.SetActive(true);
                        m_arrLblReward[i].text = m_listReward[i];
                    }
                    else
                    {
                        m_arrLblReward[i].transform.parent.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    void OnClick()
    {
        gameObject.SetActive(false);
    }
    void Awake()
    {

        m_myTransform = transform;
        if (m_arrLblReward == null)
        {
            m_arrLblReward = new UILabel[TipNum];
        }
        for (int i = 0; i < TipNum; ++i)
        {
            m_arrLblReward[i] = m_myTransform.Find("WeekRankDialogReward" + i + "/" + i + "Text").GetComponentsInChildren<UILabel>(true)[0];
        }

        m_lblTipTitle = m_myTransform.Find("WeekRankDialogRewardTitleText").GetComponentsInChildren<UILabel>(true)[0];
    }

    void Start()
    {
        if (m_arrLblReward == null)
        {
            m_arrLblReward = new UILabel[TipNum];
        }
        for (int i = 0; i < TipNum; ++i)
        {
            if (i < m_listReward.Count)
            {
                m_arrLblReward[i].transform.parent.gameObject.SetActive(true);
                m_arrLblReward[i].text = m_listReward[i];
            }
            else
            {
                m_arrLblReward[i].transform.parent.gameObject.SetActive(false);
            }
        }

        m_lblTipTitle.text = m_strTipTitle;
    }
}
