using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;
using Mogo.GameData;
using System.Text.RegularExpressions;
/// <summary>
/// 相当于在这里做一层接口的封装，外面调用时把这个当成整体
/// </summary>
public class TowerRewardUnit : MogoParentUI
{
    #region 开放属性
    public Action<int> clickHandler;
    public Action<int> towerTipHandler;
    public int RewardID = -1;
    private bool m_isAlreadyGet;
    public bool IsAlreadyGet
    {
        get { return m_isAlreadyGet; }
        set
        {
            m_isAlreadyGet = value;
            if (m_alreadyGet != null && m_notGet != null)
            {
                if (m_isAlreadyGet)
                {
                    m_alreadyGet.SetActive(true);
                    m_notGet.SetActive(false);
                }
                else
                {
                    m_alreadyGet.SetActive(false);
                    m_notGet.SetActive(true);
                }
            }

        }
    }
    void Start()
    {
        if (m_isAlreadyGet)
        {
            m_alreadyGet.SetActive(true);
            m_notGet.SetActive(false);
        }
        else
        {
            m_alreadyGet.SetActive(false);
            m_notGet.SetActive(true);
        }

        //m_alreadyGet.GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = IconData.dataMap.Get(m_icon).path.Split(new char[] { ',' })[1];
        //m_notGet.GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = IconData.dataMap.Get(m_icon).path.Split(new char[] { ',' })[0];
        m_lblRewardName.text = m_rewardname;
        lblTowerDesc.text = m_towerDesc;
        lblForce.text = m_force;
        lblTowerName.text = m_towerName;
    }
    private int m_icon;
    public int icon
    {
        set
        {
            m_icon = value;

            if (m_alreadyGet != null && m_notGet != null)
            {
                m_alreadyGet.GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = IconData.dataMap.Get(value).path.Split(new char[] { ',' })[1];
                m_notGet.GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = IconData.dataMap.Get(value).path.Split(new char[] { ',' })[0];
            }
        }
    }
    private string m_rewardname;
    public string RewardName
    {
        get { return m_lblRewardName.text; }
        set
        {
            m_rewardname = value;
            if (m_lblRewardName != null)
            {
                m_lblRewardName.text = value;
            }

        }
    }
    private string m_towerDesc;
    private UILabel lblTowerDesc;
    public string TowerDesc
    {
        get
        {
            return m_towerDesc;
        }
        set
        {
            m_towerDesc = value;
            if (lblTowerDesc != null)
            {
                lblTowerDesc.text = value;
            }
        }
    }
    private string m_force;
    private UILabel lblForce;
    public string force
    {
        set
        {
            m_force = value;
            if (lblForce != null)
            {
                lblForce.text = value;
            }
        }
    }
    private string m_towerName;
    private UILabel lblTowerName;
    public string TowerName
    {
        set
        {
            m_towerName = value;
            if (lblTowerName != null)
            {
                lblTowerName.text = value;
            }
        }
    }
    private int m_picID;
    public int PicID
    {

        set
        {
            m_picID = value;
            for (int i = 0; i < 3; i++)
            {
                if (i == value)
                {
                    SetPicVisible(i, true);
                }
                else
                {
                    SetPicVisible(i, false);
                }
            }
        }
    }
    private void SetPicVisible(int id, bool show)
    {
        string lPath = string.Format("tower/{0}/{1}_left", id + 1, id);
        string rPath = string.Format("tower/{0}/{1}_right", id + 1, id);
        var trans = m_Transform.Find(string.Format("tower/{0}", id + 1));
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, id + RewardID);
        m_Transform.Find(lPath).gameObject.SetActive(show);
        m_Transform.Find(rPath).gameObject.SetActive(show);
    }
    private UILabel lblForceCurrent;

    public string forceCurrent
    {
        set
        {
            lblForceCurrent.text = value;
        }
    }
    public bool Unlocked
    {
        set
        {
            if (value)
            {
                m_Transform.Find("tower/node").gameObject.SetActive(false);
            }
            else
            {
                m_Transform.Find("tower/node").gameObject.SetActive(true);
            }
        }
    }
    public bool HighLight
    {
        set
        {
            if (value)
            {
                lblTowerName.text = Regex.Replace(lblTowerName.text, @"\[\w*\]", "[FF0000]");
                lblTowerDesc.color = Color.red;
                m_lblRewardName.text = Regex.Replace(m_lblRewardName.text, @"\[\w*\]", "[FF0000]");
                lblForceCurrent.color = Color.red;
                m_Transform.Find("tower/TriPager").gameObject.SetActive(true);
                m_Transform.Find("boxReward/bg").GetComponentsInChildren<MogoTwoStatusButton>(true)[0].SetButtonDown(true);
            }
            else
            {
                m_Transform.Find("tower/TriPager").gameObject.SetActive(false);
                m_Transform.Find("boxReward/bg").GetComponentsInChildren<MogoTwoStatusButton>(true)[0].SetButtonDown(false);
            }
        }
    }
    public bool Grey
    {
        set
        {
            if (value)
            {
                lblForce.text = Regex.Replace(lblForce.text, @"\[\w*\]", "[FFFFFF]");
                lblTowerName.text = Regex.Replace(lblTowerName.text, @"\[\w*\]", "[FFFFFF]");
                m_lblRewardName.text = Regex.Replace(m_lblRewardName.text, @"\[\w*\]", "[FFFFFF]");
            }
            string lPath = string.Format("tower/{0}/{1}_left", m_picID + 1, m_picID);
            string rPath = string.Format("tower/{0}/{1}_right", m_picID + 1, m_picID);
            var left = m_Transform.Find(lPath).GetComponentsInChildren<UISprite>(true)[0];
            left.ShowAsWhiteBlack(value);
            var right = m_Transform.Find(rPath).GetComponentsInChildren<UISprite>(true)[0];
            right.ShowAsWhiteBlack(value);
        }
    }
    #endregion
    #region 组件定义
    GameObject m_alreadyGet;
    GameObject m_notGet;
    GameObject m_towerDescObj;
    UILabel m_lblRewardName;
    Camera m_dragCamera;
    Transform m_Transform;
    #endregion

    void OnSingleButtonClicked()
    {
        if (clickHandler != null)
        {
            clickHandler(RewardID);
        }
    }
    void OnTowerTipClicked()
    {
        if (towerTipHandler != null)
        {
            towerTipHandler(RewardID);
        }
    }
    void Close()
    {
        gameObject.SetActive(false);
    }
    void Awake()
    {
        m_Transform = transform;

        m_alreadyGet = m_Transform.Find("boxReward/btnRewardBox/btnRewardBGDown").gameObject;
        m_notGet = m_Transform.Find("boxReward/btnRewardBox/btnRewardBGUp").gameObject;
        m_lblRewardName = m_Transform.Find("boxReward/bg/txtRewardName").GetComponentsInChildren<UILabel>(true)[0];
        m_Transform.Find("boxReward/btnRewardBox").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSingleButtonClicked;
        m_Transform.Find("tower/tipArea").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnTowerTipClicked;
        lblTowerDesc = m_Transform.Find("tower/TriPager/node/lblCurrentLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_towerDescObj = m_Transform.Find("tower/TriPager").gameObject;
        lblForce = m_Transform.Find("tower/node/force").GetComponent<UILabel>();
        lblTowerName = m_Transform.Find("tower/text").GetComponent<UILabel>();
        lblForceCurrent = m_Transform.Find("tower/TriPager/node/lblForce").GetComponent<UILabel>();

    }
}
