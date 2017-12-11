/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InstanceLevelGrid
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class InstanceGrid : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public int id;

    Transform m_myTransform;
    GameObject enableGo;

    UILabel m_starNumber;
    UISprite m_spStar;
    UISprite m_spBGUp;

    UISprite m_spStarDown;

    UISprite m_spGridIcon;
    UISprite m_spStarBG;

    GameObject m_goLockPart;

    GameObject m_lblBGText;
    GameObject m_lblStarNum;

    //void OnPress(bool isOver)
    //{
    //    if (!isOver)
    //    {
    //        EventDispatcher.TriggerEvent("INSTANCECHOOSEGRIDUP", id);
    //        // InstanceUIViewManager.Instance.INSTANCECHOOSEGRIDUP(id);
    //    }
    //}

    void OnClick()
    {
        EventDispatcher.TriggerEvent("INSTANCECHOOSEGRIDUP", id);
    }

    void Awake()
    {
        m_myTransform = transform;
        enableGo = transform.parent.gameObject;

        m_starNumber = m_myTransform.parent.Find("InstanceChooseBodyGridStarList/" + "InstanceChooseBodyGridStarNumber").GetComponentsInChildren<UILabel>(true)[0];

        m_spStar = m_myTransform.parent.Find("InstanceChooseBodyGridStarList").Find("InstanceChooseBodyGridStar").Find("InstanceChooseBodyGridQuadStarBGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_spStarDown = m_myTransform.parent.Find("InstanceChooseBodyGridStarList").Find("InstanceChooseBodyGridStar").Find("InstanceChooseBodyGridQuadStarBGDown").GetComponentsInChildren<UISprite>(true)[0];

        m_spGridIcon = m_myTransform.parent.Find("InstanceChooseBodyGridFG").GetComponentsInChildren<UISprite>(true)[0];
        m_spStarBG = m_myTransform.Find("InstanceChooseBodyGridStarBG").GetComponentsInChildren<UISprite>(true)[0];
        m_spBGUp = m_myTransform.Find("InstanceChooseBodyGridBGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_goLockPart = m_myTransform.parent.Find("InstanceChooseBodyGridLock").gameObject;

        m_lblStarNum = m_myTransform.parent.Find("InstanceChooseBodyGridStarList/InstanceChooseBodyGridStarNumber").gameObject;
        m_lblBGText = m_myTransform.Find("InstanceChooseBodyGridBGText").gameObject;
    }

    void Start()
    {

        EventDispatcher.TriggerEvent("InstanceGridAwakeEnd");
    }


    public void SetEnable(bool enable)
    {
        //if (enable)
        //{
        //    m_spBGUp.color = new Color32(255, 255, 255, 255);

        //}
        //else
        //{
        //    m_spBGUp.color = new Color32(128, 128, 128, 255);
        //}

        //enableGo.SetActive(enable);

        //m_spBGUp.ShowAsWhiteBlack(!enable);
        //m_spGridIcon.ShowAsWhiteBlack(!enable);
        //m_spStar.ShowAsWhiteBlack(!enable);
        //m_spStarDown.ShowAsWhiteBlack(!enable);
        //m_spStarBG.ShowAsWhiteBlack(!enable);

        m_spGridIcon.gameObject.SetActive(enable);
        m_spStar.gameObject.SetActive(enable);
        m_spStarDown.gameObject.SetActive(enable);
        m_spStarBG.gameObject.SetActive(enable);
        m_lblBGText.SetActive(enable);
        m_lblStarNum.SetActive(enable);
        m_goLockPart.SetActive(!enable);


        gameObject.GetComponentsInChildren<BoxCollider>(true)[0].enabled = enable;
    }

    public void ShowStars(int num)
    {
        m_starNumber.text = num.ToString();
        if (m_goLockPart.activeSelf== false)
        {
            
            // m_spStar.gameObject.SetActive(num != 0);
            m_spStarDown.gameObject.SetActive(num == 0);
        }
    }
}
