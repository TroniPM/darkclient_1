using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
//using Mogo.Game;
using System.Text;

public class InstanceLevelGrid : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public int id;

    Transform m_myTransform;

    UISprite m_spBGUp;
    UISprite m_spBGDown;

    UILabel m_starNumber;
    UISprite m_spStar;



    UISprite m_spGridIcon;
    UISprite m_spGridBG;
    UISprite m_spGridStar0Up;
    UISprite m_spGridStar1Up;
    UISprite m_spGridStar2Up;
    UISprite m_spGridStar0Down;
    UISprite m_spGridStar1Down;
    UISprite m_spGridStar2Down;
    UISprite m_spRewardBG;
    UISprite m_spReward0;
    UISprite m_spReward1;
    UISprite m_spDiamondBtn;
    UILabel m_lblSuggestLevelText;
    UILabel m_lblSuggestLevel;
    UILabel m_lblChallengeText;
    UILabel m_lblChallengeNum;

    void Awake()
    {
        m_myTransform = transform;

        m_spBGUp = m_myTransform.Find("InstanceLevel" + id + "BG/" + "InstanceLevel" + id + "BGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spBGDown = m_myTransform.Find("InstanceLevel" + id + "BG/" + "InstanceLevel" + id + "BGDown").GetComponentsInChildren<UISprite>(true)[0];

        m_spGridIcon = m_myTransform.Find("InstanceLevel" + id + "Icon").GetComponentsInChildren<UISprite>(true)[0];

        Mogo.Util.LoggerHelper.Debug("InstanceLevel" + id + "StarList/" +
            "InstanceLevel" + id + "Star0/" + "InstanceLevel" + id + "Star0BGUp");
        m_spGridStar0Up = m_myTransform.Find("InstanceLevel" + id + "StarList/" + 
            "InstanceLevel" + id + "Star0/" + "InstanceLevel" + id + "Star0BGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_spGridStar1Up = m_myTransform.Find("InstanceLevel" + id + "StarList/" +
            "InstanceLevel" + id + "Star1/" + "InstanceLevel" + id + "Star1BGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_spGridStar2Up = m_myTransform.Find("InstanceLevel" + id + "StarList/" +
            "InstanceLevel" + id + "Star2/" + "InstanceLevel" + id + "Star2BGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_spGridStar0Down = m_myTransform.Find("InstanceLevel" + id + "StarList/" +
            "InstanceLevel" + id + "Star0/" + "InstanceLevel" + id + "Star0BGDown").GetComponentsInChildren<UISprite>(true)[0];

        m_spGridStar1Down = m_myTransform.Find("InstanceLevel" + id + "StarList/" +
            "InstanceLevel" + id + "Star1/" + "InstanceLevel" + id + "Star1BGDown").GetComponentsInChildren<UISprite>(true)[0];

        m_spGridStar2Down = m_myTransform.Find("InstanceLevel" + id + "StarList/" +
            "InstanceLevel" + id + "Star2/" + "InstanceLevel" + id + "Star2BGDown").GetComponentsInChildren<UISprite>(true)[0];

        m_spRewardBG = m_myTransform.Find("InstanceLevel" + id + "Thing1").GetComponentsInChildren<UISlicedSprite>(true)[0];

        m_spReward0 = m_myTransform.Find("InstanceLevel" + id + "RewardList/" + "InstanceLevel" + id + "Reward0").GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_spReward1 = m_myTransform.Find("InstanceLevel" + id + "RewardList/" + "InstanceLevel" + id + "Reward1").GetComponentsInChildren<UISlicedSprite>(true)[0];

        if (id == 2)
        {
            m_spDiamondBtn = m_myTransform.Find("InstanceLevel2Diamond/InstanceLevel2DiamondUp").GetComponentsInChildren<UISlicedSprite>(true)[0];
 
        }

        m_lblChallengeNum = m_myTransform.Find("InstanceLevel" + id + "ChallengeTimesNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblSuggestLevel = m_myTransform.Find("InstanceLevel" + id + "SuggestLevelNum").GetComponentsInChildren<UILabel>(true)[0];
    }

    public void ShowStars(int num)
    {
        Mogo.Util.LoggerHelper.Debug("ShowStars: " + num);
        if (num == 1)
        {
            m_spGridStar0Up.gameObject.SetActive(true);
            m_spGridStar1Up.gameObject.SetActive(false);
            m_spGridStar2Up.gameObject.SetActive(false);
        }
        else if (num == 2)
        {
            m_spGridStar0Up.gameObject.SetActive(true);
            m_spGridStar1Up.gameObject.SetActive(true);
            m_spGridStar2Up.gameObject.SetActive(false);
        }
        else if (num == 3)
        {
            m_spGridStar0Up.gameObject.SetActive(true);
            m_spGridStar1Up.gameObject.SetActive(true);
            m_spGridStar2Up.gameObject.SetActive(true);
        }
        else
        {
            m_spGridStar0Up.gameObject.SetActive(false);
            m_spGridStar1Up.gameObject.SetActive(false);
            m_spGridStar2Up.gameObject.SetActive(false);
        }
    }

    public void SetChoose(bool isChoose)
    {
        if (isChoose)
            gameObject.transform.parent.GetComponent<MogoSingleButtonList>().SetCurrentDownButton(id);
    }

    public void SetEnable(bool enable)
    {
        //MaiFeo Dosometing Here
        //if (enable)
        //{
        //    //m_spBGUp.color = new Color32(255, 255, 255, 255);
        //    m_spBGUp.ShowAsWhiteBlack(true);
        //}
        //else
        //{
        //    m_spBGUp.color = new Color32(128, 128, 128, 255);
        //}

        m_spBGUp.ShowAsWhiteBlack(!enable);
        //m_spBGDown.ShowAsWhiteBlack(!enable);
        m_spGridStar0Up.ShowAsWhiteBlack(!enable);
        m_spGridStar1Up.ShowAsWhiteBlack(!enable);
        m_spGridStar2Up.ShowAsWhiteBlack(!enable);
        m_spGridStar0Down.ShowAsWhiteBlack(!enable);
        m_spGridStar1Down.ShowAsWhiteBlack(!enable);
        m_spGridStar2Down.ShowAsWhiteBlack(!enable);
        m_spRewardBG.ShowAsWhiteBlack(!enable);
        m_spReward0.ShowAsWhiteBlack(!enable);
        m_spReward1.ShowAsWhiteBlack(!enable);
        m_spGridIcon.ShowAsWhiteBlack(!enable);

        

        if (enable)
        {
            m_lblChallengeNum.color = new Color(172/255f, 227/255f, 5/255f);
            m_lblSuggestLevel.color = new Color(255/255f, 176/255f, 7/255f);
            m_lblChallengeNum.effectStyle = UILabel.Effect.Outline;
            m_lblSuggestLevel.effectStyle = UILabel.Effect.Outline;
        }
        else
        {
            m_lblChallengeNum.color = new Color(0, 0, 0);
            m_lblSuggestLevel.color = new Color(0, 0, 0);
            m_lblChallengeNum.effectStyle = UILabel.Effect.None;
            m_lblSuggestLevel.effectStyle = UILabel.Effect.None;
            m_spBGDown.gameObject.SetActive(false);
        }

        if (id == 2)
        {
            m_spDiamondBtn.ShowAsWhiteBlack(!enable);
            m_spDiamondBtn.transform.parent.GetComponentsInChildren<BoxCollider>(true)[0].enabled = enable;
        }

        gameObject.GetComponentsInChildren<BoxCollider>(true)[0].enabled = enable;

    }

    public void SetDateTimes(string dayTimes, string maxDayTimes)
    {
        m_lblChallengeNum.text = dayTimes + "/" + maxDayTimes;
    }

    public void SetRecommendLevel(string recommendLevel)
    {
        m_lblSuggestLevel.text = recommendLevel;
    }
}
