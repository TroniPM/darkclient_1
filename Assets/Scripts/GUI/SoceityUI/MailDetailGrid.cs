#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：邮件详细信息
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using TDBID = System.UInt64;
public class MailDetailGrid : MonoBehaviour 
{
    public TDBID m_MailId;
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

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();
    Transform m_myTransform;

    private UILabel m_lblMailDetailInfoText;
    private UILabel m_lblMailDetailReciverName;
    private UILabel m_lblMailDetailSenderName;
    private UILabel m_lblMailDetailTimeText;
    private UILabel m_lblMailDetailTitleText;

    private UISprite m_spMailItemGetSign;
    private GameObject m_goOneKeyGetMailItem;

    private UISprite[] m_arrSpMailDetailItem = new UISprite[5];
    private UISprite[] m_arrSpMailDetialItemBG = new UISprite[5];
    private UILabel[] m_arrLblMailDetailItemNum = new UILabel[5];

    void Awake()
    {
        m_myTransform = transform;
		FillFullNameData(m_myTransform);

        m_lblMailDetailInfoText = m_myTransform.Find(m_widgetToFullName["MailDetailInfoText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailReciverName = m_myTransform.Find(m_widgetToFullName["MailDetailRecvierName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailSenderName = m_myTransform.Find(m_widgetToFullName["MailDetailSenderName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailTimeText = m_myTransform.Find(m_widgetToFullName["MailDetailTimeText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailTitleText = m_myTransform.Find(m_widgetToFullName["MailDetailTitleText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_spMailItemGetSign = m_myTransform.Find(m_widgetToFullName["MailDetailItemGetSign"]).GetComponentsInChildren<UISprite>(true)[0];
        m_goOneKeyGetMailItem = m_myTransform.Find(m_widgetToFullName["MailDetailOneKeyGetBtn"]).gameObject;

        for (int i = 0; i < 5; ++i)
        {
            m_arrSpMailDetailItem[i] = m_myTransform.Find(m_widgetToFullName["MailDetailItem" + i + "FG"]).GetComponentsInChildren<UISprite>(true)[0];
            m_arrSpMailDetialItemBG[i] = m_myTransform.Find(m_widgetToFullName["MailDetailItem" + i + "BG"]).GetComponentsInChildren<UISprite>(true)[0];
            m_arrLblMailDetailItemNum[i] = m_myTransform.Find(m_widgetToFullName["MailDetailItem" + i + "Num"]).GetComponentsInChildren<UILabel>(true)[0];
        }
    }

    /// <summary>
    /// 设置邮箱详细信息
    /// </summary>
    /// <param name="md"></param>
    public void FillMailDetailData(MailDetailData md)
    {
        m_lblMailDetailInfoText.text = md.infoText;
        m_lblMailDetailReciverName.text = md.reciverName;
        m_lblMailDetailSenderName.text = md.senderName;
        m_lblMailDetailTimeText.text = Utils.GetTime(int.Parse(md.time)).ToString("yyyy-MM-dd");
        m_lblMailDetailTitleText.text = md.title;

        for (int i = 0; i < 5; ++i)
        {
            if (i < md.listItemImg.Count)
            {
                //m_arrSpMailDetailItem[i].atlas = MogoUIManager.Instance.GetAtlasByIconName(md.listItemImg[i]);
                //m_arrSpMailDetailItem[i].spriteName = md.listItemImg[i];

                InventoryManager.SetIcon(md.listItemID[i], m_arrSpMailDetailItem[i],0,null,m_arrSpMailDetialItemBG[i]);
                
                if (md.listItemNum[i] == 1)
                {
                    m_arrLblMailDetailItemNum[i].gameObject.SetActive(false);
                }
                else
                {
                    m_arrLblMailDetailItemNum[i].gameObject.SetActive(true);
                }

                m_arrLblMailDetailItemNum[i].text = md.listItemNum[i].ToString();
                m_arrSpMailDetailItem[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                m_arrSpMailDetailItem[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 清空邮件信息
    /// </summary>
    public void ClearMailDetailData()
    {
        m_lblMailDetailInfoText.text = "";
        m_lblMailDetailReciverName.text = "";
        m_lblMailDetailSenderName.text = "";
        m_lblMailDetailTimeText.text = "";
        m_lblMailDetailTitleText.text = "";
    }

    public void PlayMailItemGetSignAnim()
    {
        TweenScale m_ts = m_spMailItemGetSign.GetComponentsInChildren<TweenScale>(true)[0];

        m_spMailItemGetSign.gameObject.SetActive(true);

        m_ts.enabled = true;

        m_ts.Reset();
        m_ts.from = new Vector3(256, 100, 1);
        m_ts.to = new Vector3(128, 56, 1);

        m_ts.Play(true);
    }

    public void ShowMailItemGetSign(bool isShow = true)
    {
        m_spMailItemGetSign.gameObject.SetActive(isShow);
        m_spMailItemGetSign.transform.localScale = new Vector3(128, 56, 1);
    }

    public void ShowOneKeyGetItemBtn(bool isShow)
    {
        m_goOneKeyGetMailItem.SetActive(isShow);
    }
}
