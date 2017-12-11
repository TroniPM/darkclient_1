#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：胜利结算界面
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;
using Mogo.GameData;

public enum FriendState
{
    NoFriend = 1, // 没有好友
    IsFriend = 2, // 已经是好友，增加好友度
    MakeFriend = 3,// 不是好友，加为好友
}

public class InstancePassUIViewManager : MonoBehaviour {

    private static InstancePassUIViewManager m_instance;

    public static InstancePassUIViewManager Instance
    {
        get
        {
            return InstancePassUIViewManager.m_instance;
        }
    }

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();    

    const float REWARDITEMSPACE = 0.153f;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        if (m_widgetToFullName.ContainsKey(widgetName))
            LoggerHelper.Debug(widgetName);
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

    private Transform m_myTransform;

    UILabel m_lblInstancePassUIPassTime;
    UILabel m_lblInstancePassUIMaxBatter;
    UILabel m_lblInstancePassUIScore;
    UILabel m_lblInstancePassUIFriend;
    UILabel m_lblInstancePassUIMakeFriend;

    GameObject m_goInstancePassUIMakeFriend;
    GameObject m_goInstancePassUIOK;

    UITexture m_texInstancePassUIStarType;

    GameObject m_goInstancePassUIPassTime;
    GameObject m_goInstancePassUIMaxBatter;
    GameObject m_goInstancePassUIScore;
    GameObject m_goInstancePassUIStarType;
    GameObject m_goInstancePassUIStarTypeBG;
    GameObject m_goInstancePassUINormal;
    GameObject m_goInstancePassUIFriend;

    // pos
    Transform m_tranInstancePassUIPassTimePosFrom;
    Transform m_tranInstancePassUIPassTimePosTo;
    Transform m_tranInstancePassUIMaxBatterPosFrom;
    Transform m_tranInstancePassUIMaxBatterPosTo;
    Transform m_tranInstancePassUIScorePosFrom;
    Transform m_tranInstancePassUIScorePosTo;
    Transform m_tranInstancePassUIStarTypeBGPosFrom;
    Transform m_tranInstancePassUIStarTypeBGPosTo;
   

    void Awake()
    {
        m_instance = gameObject.GetComponent<InstancePassUIViewManager>();
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblInstancePassUIPassTime = m_myTransform.Find(m_widgetToFullName["InstancePassUIPassTimeNum"]).GetComponent<UILabel>();
        m_lblInstancePassUIMaxBatter = m_myTransform.Find(m_widgetToFullName["InstancePassUIMaxBatterNum"]).GetComponent<UILabel>();
        m_lblInstancePassUIScore = m_myTransform.Find(m_widgetToFullName["InstancePassUIScoreNum"]).GetComponent<UILabel>();
        m_lblInstancePassUIFriend = m_myTransform.Find(m_widgetToFullName["InstancePassUIFriendText"]).GetComponent<UILabel>();
        m_lblInstancePassUIMakeFriend= m_myTransform.Find(m_widgetToFullName["InstancePassUIMakeFriendText"]).GetComponent<UILabel>();

        m_goInstancePassUIMakeFriend = m_myTransform.Find(m_widgetToFullName["InstancePassUIMakeFriend"]).gameObject;
        m_goInstancePassUIOK = m_myTransform.Find(m_widgetToFullName["InstancePassUIOK"]).gameObject;

        m_texInstancePassUIStarType = m_myTransform.Find(m_widgetToFullName["InstancePassUIStarType"]).GetComponent<UITexture>();

        m_goInstancePassUIPassTime = m_myTransform.Find(m_widgetToFullName["GOInstancePassUIPassTime"]).gameObject;
        m_goInstancePassUIMaxBatter = m_myTransform.Find(m_widgetToFullName["GOInstancePassUIMaxBatter"]).gameObject;
        m_goInstancePassUIScore = m_myTransform.Find(m_widgetToFullName["GOInstancePassUIScore"]).gameObject;
        m_goInstancePassUIStarType = m_myTransform.Find(m_widgetToFullName["InstancePassUIStarType"]).gameObject;
        m_goInstancePassUIStarTypeBG = m_myTransform.Find(m_widgetToFullName["InstancePassUIStarTypeBG"]).gameObject;
        m_goInstancePassUINormal = m_myTransform.Find(m_widgetToFullName["GOInstancePassUINormal"]).gameObject;
        m_goInstancePassUIFriend = m_myTransform.Find(m_widgetToFullName["InstancePassUIFriend"]).gameObject;       

        m_tranInstancePassUIPassTimePosFrom = m_myTransform.Find(m_widgetToFullName["InstancePassUIPassTimePosFrom"]);
        m_tranInstancePassUIPassTimePosTo = m_myTransform.Find(m_widgetToFullName["InstancePassUIPassTimePosTo"]);
        m_tranInstancePassUIMaxBatterPosFrom = m_myTransform.Find(m_widgetToFullName["InstancePassUIMaxBatterPosFrom"]);
        m_tranInstancePassUIMaxBatterPosTo = m_myTransform.Find(m_widgetToFullName["InstancePassUIMaxBatterPosTo"]);
        m_tranInstancePassUIScorePosFrom = m_myTransform.Find(m_widgetToFullName["InstancePassUIScorePosFrom"]);
        m_tranInstancePassUIScorePosTo = m_myTransform.Find(m_widgetToFullName["InstancePassUIScorePosTo"]);
        m_tranInstancePassUIStarTypeBGPosFrom = m_myTransform.Find(m_widgetToFullName["InstancePassUIStarTypeBGPosFrom"]);
        m_tranInstancePassUIStarTypeBGPosTo = m_myTransform.Find(m_widgetToFullName["InstancePassUIStarTypeBGPosTo"]);

        Initialize();
    }

    #region 事件
    public Action INSTANCEPASSOKUP;
    public Action INSTANCEPASSMAKEFRIENDUP;

    public void Initialize()
    {
        //ResetFirst();

        InstanceUIDict.ButtonTypeToEventUp.Add("InstancePassUIOK", OnPassUIOK);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstancePassUIMakeFriend", OnPassUIMakeFriendUp);

        EventDispatcher.TriggerEvent("InstanceUILoadPartEnd");
    }

    public void Release()
    {
     
    }

    void OnPassUIOK(int i)
    {
        if (INSTANCEPASSOKUP != null)
            INSTANCEPASSOKUP();

        // 显示玩家名字
        BillboardViewManager.Instance.ShowBillboard(MogoWorld.thePlayer.ID, true);
        BillboardViewManager.Instance.ShowBillboard(MogoWorld.theLittleGuyID, true);
    }

    void OnPassUIMakeFriendUp(int i)
    {
        if (INSTANCEPASSMAKEFRIENDUP != null)
        {
            ShowInstancePassUIMakeFriendBtn(false);
            INSTANCEPASSMAKEFRIENDUP();
        }
    }

    #endregion

    /// <summary>
    /// 隐藏所有，方便之后动画播放控制
    /// </summary>
    public void HideAll()
    {
        // 通关时间
        m_goInstancePassUIPassTime.SetActive(false);      
        // 最高连击
        m_goInstancePassUIMaxBatter.SetActive(false);
        // 得分
        m_goInstancePassUIScore.SetActive(false);
        // 其他UI
        m_goInstancePassUINormal.SetActive(false);
        // 评价SABC
        m_goInstancePassUIStarType.SetActive(false);      
        // 评价SABC背景
        m_goInstancePassUIStarTypeBG.SetActive(false);
    }

    /// <summary>
    /// 设置通关时间
    /// </summary>
    /// <param name="minutes"></param>
    /// <param name="second"></param>
    public void SetPassTime(int minutes, int second)
    {
        m_lblInstancePassUIPassTime.text = minutes.ToString("d2") + " : " + second.ToString("d2");
    }

    /// <summary>
    /// 设置最高连击
    /// </summary>
    /// <param name="count"></param>
    public void SetMaxBatter(int count)
    {
        m_lblInstancePassUIMaxBatter.text = count.ToString();
    }

    /// <summary>
    /// 设置最终得分
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
        m_lblInstancePassUIScore.text = score.ToString();
    }

    bool m_bShowStar = false;
    /// <summary>
    /// 设置副本评价
    /// </summary>
    /// <param name="num"></param>
    public void ShowPassStar(int num)
    {
        Mogo.Util.LoggerHelper.Debug("ShowPassStar: " + num);
        string texName = "";
        switch ((InstanceStar)num)
        {
            case InstanceStar.InstanceStarS:
                {
                    texName = "fb-ds.png";
                }
                break;
            case InstanceStar.InstanceStarA:
                {
                    texName = "fb-da.png";
                }
                break;
            case InstanceStar.InstanceStarB:
                {
                    texName = "fb-db.png";
                }
                break;
            case InstanceStar.InstanceStarC:
                {
                    texName = "fb-dc.png";
                }
                break;
            default:
                {
                    texName = "";
                }
                break;
        }        

        if (m_texInstancePassUIStarType != null)
        {
            if (m_texInstancePassUIStarType.mainTexture != null)
                AssetCacheMgr.ReleaseResource(m_texInstancePassUIStarType.mainTexture);

            if (!string.IsNullOrEmpty(texName))
            {
                AssetCacheMgr.GetUIResource(texName, (obj) => 
                {
                    if (obj != null)
                    {
                        m_texInstancePassUIStarType.mainTexture = obj as Texture; 
                    }                    
                });
                m_texInstancePassUIStarType.gameObject.SetActive(true);
                m_bShowStar = true;
            }
            else
            {
                m_texInstancePassUIStarType.gameObject.SetActive(false);
                m_bShowStar = false;
            }
        }
    }

    /// <summary>
    /// 设置好友信息
    /// </summary>
    /// <param name="friendName"></param>
    public void SetFriendState(int state, string friendName = "")
    {
        switch ((FriendState)state)
        {
            case FriendState.NoFriend:
                {
                    m_goInstancePassUIFriend.SetActive(false);
                    m_lblInstancePassUIFriend.gameObject.SetActive(false);
                    ShowInstancePassUIMakeFriendBtn(false);
                }break;
            case FriendState.IsFriend:
                {
                    m_goInstancePassUIFriend.SetActive(true);
                    m_lblInstancePassUIFriend.gameObject.SetActive(true);
                    ShowInstancePassUIMakeFriendBtn(true);
                    m_lblInstancePassUIMakeFriend.text = LanguageData.GetContent(46989);
                    SetFriend(friendName);
                }break;
            case FriendState.MakeFriend:
                {
                    m_goInstancePassUIFriend.SetActive(true);
                    m_lblInstancePassUIFriend.gameObject.SetActive(true);
                    ShowInstancePassUIMakeFriendBtn(true);
                    m_lblInstancePassUIMakeFriend.text = LanguageData.GetContent(46990);
                    SetFriend(friendName);
                }break;
            default: break;
        }
    }

    private void SetFriend(string friendName)
    {
        m_lblInstancePassUIFriend.text = friendName + LanguageData.GetContent(46991);
    }

    void ShowInstancePassUIMakeFriendBtn(bool isShow)
    {
        m_goInstancePassUIMakeFriend.SetActive(isShow);
    }

    #region 动画

    readonly static float DelayTime = 0.1f;
    readonly static float PassTimeDuration = 0.3f;//0.5f;
    readonly static float MaxBatterDuration = 0.3f;//0.5f;
    readonly static float ScoreDuration = 0.3f;//0.5f;
    // 为去除警告暂时屏蔽以下代码
    //readonly static float StarBGDuration = 0.3f;//0.5f;
    readonly static float NormationDuration = 0.1f;
    readonly static float StarTypeDuration = 0.1f;//0.3f;

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayResultOutside()
    {
        ResetAnimation();
        StartCoroutine(PlayAnimation());
    }

    /// <summary>
    /// 播放胜利结算，所有设置完成之后调用
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    IEnumerator PlayAnimation(float delayTime = 1.2f)
    {
        yield return new WaitForSeconds(0.01f);

        // 通关时间
        PlayInstancePassUIPassTime(true);     
        yield return new WaitForSeconds(PassTimeDuration + DelayTime);
		
        // 最高连击
        PlayInstancePassUIMaxBatter(true);
        yield return new WaitForSeconds(MaxBatterDuration + DelayTime);      
		
        // 得分
        PlayInstancePassUIScore(true);
        if (m_bShowStar)
        {
            // 评价SABC背景
            PlayInstancePassUIStarTypeBG(true);
        }
        yield return new WaitForSeconds(ScoreDuration + DelayTime);        

        // 其他UI
        m_goInstancePassUINormal.SetActive(true);
        yield return new WaitForSeconds(NormationDuration);
		
        // 评价SABC
        if (m_bShowStar)
        {
            PlayInstancePassUIStarType(true);
            //yield return new WaitForSeconds(StarTypeDuration + DelayTime);  
        }                
    }

    /// <summary>
    /// 动画播放前，重置动画
    /// </summary>
    void ResetAnimation()
    {
        HideAll();

        // 通关时间
        PlayInstancePassUIPassTime(false);

        // 最高连击
        PlayInstancePassUIMaxBatter(false);

        // 得分
        PlayInstancePassUIScore(false);

        // 评价SABC背景
        PlayInstancePassUIStarTypeBG(false);

        // 评价SABC
        PlayInstancePassUIStarType(false);
    }

    /// <summary>
    /// 通关时间
    /// </summary>
    /// <param name="isPlay"></param>
    void PlayInstancePassUIPassTime(bool isPlay)
    {
        if (isPlay)
        {
            m_goInstancePassUIPassTime.SetActive(true);
            TweenPosition passTimeTP = m_goInstancePassUIPassTime.GetComponent<TweenPosition>();
            if (passTimeTP != null)
                passTimeTP.enabled = true;
        }
        else
        {
            TweenPosition passTimeTP = m_goInstancePassUIPassTime.GetComponent<TweenPosition>();
            if (passTimeTP != null)
                Destroy(passTimeTP); //销毁该脚本       
            passTimeTP = m_goInstancePassUIPassTime.AddComponent<TweenPosition>();
            passTimeTP.from = m_tranInstancePassUIPassTimePosFrom.localPosition;
            passTimeTP.to = m_tranInstancePassUIPassTimePosTo.localPosition;
            passTimeTP.duration = PassTimeDuration;
            passTimeTP.style = UITweener.Style.Once;
            passTimeTP.eventReceiver = gameObject;
            passTimeTP.callWhenFinished = "OnTPEnd";
            passTimeTP.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
            passTimeTP.enabled = false;
        }
    }

    /// <summary>
    /// 最高连击
    /// </summary>
    /// <param name="isPlay"></param>
    void PlayInstancePassUIMaxBatter(bool isPlay)
    {
        if (isPlay)
        {
            m_goInstancePassUIMaxBatter.SetActive(true);
            TweenPosition maxBatterTP = m_goInstancePassUIMaxBatter.GetComponent<TweenPosition>();
            if (maxBatterTP != null)
                maxBatterTP.enabled = true;
        }
        else
        {
            TweenPosition maxBatterTP = m_goInstancePassUIMaxBatter.GetComponent<TweenPosition>();
            if (maxBatterTP != null)
                Destroy(maxBatterTP); //销毁该脚本 
            maxBatterTP = m_goInstancePassUIMaxBatter.AddComponent<TweenPosition>();
            maxBatterTP.from = m_tranInstancePassUIMaxBatterPosFrom.localPosition;
            maxBatterTP.to = m_tranInstancePassUIMaxBatterPosTo.localPosition;
            maxBatterTP.duration = MaxBatterDuration;
            maxBatterTP.style = UITweener.Style.Once;
            maxBatterTP.eventReceiver = gameObject;
            maxBatterTP.callWhenFinished = "OnTPEnd";
            maxBatterTP.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
            maxBatterTP.enabled = false;
        }
    }

    /// <summary>
    /// 得分
    /// </summary>
    /// <param name="isPlay"></param>
    void PlayInstancePassUIScore(bool isPlay)
    {
        if (isPlay)
        {
            m_goInstancePassUIScore.SetActive(true);
            TweenPosition scoreTP = m_goInstancePassUIScore.GetComponent<TweenPosition>();
            if (scoreTP != null)
                scoreTP.enabled = true;
        }
        else
        {
            TweenPosition scoreTP = m_goInstancePassUIScore.GetComponent<TweenPosition>();
            if (scoreTP != null)
                Destroy(scoreTP); //销毁该脚本   
            scoreTP = m_goInstancePassUIScore.AddComponent<TweenPosition>();
            scoreTP.from = m_tranInstancePassUIScorePosFrom.localPosition;
            scoreTP.to = m_tranInstancePassUIScorePosTo.localPosition;
            scoreTP.duration = ScoreDuration;
            scoreTP.style = UITweener.Style.Once;
            scoreTP.eventReceiver = gameObject;
            scoreTP.callWhenFinished = "OnTPEnd";
            scoreTP.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
            scoreTP.enabled = false;
        }
    }

    /// <summary>
    /// 评价SABC背景
    /// </summary>
    /// <param name="isPlay"></param>
    void PlayInstancePassUIStarTypeBG(bool isPlay)
    {
        if (isPlay)
        {
            m_goInstancePassUIStarTypeBG.SetActive(true);
            TweenPosition starBgTP = m_goInstancePassUIStarTypeBG.GetComponent<TweenPosition>();
            if (starBgTP != null)
                starBgTP.enabled = true;
        }
        else
        {
            TweenPosition starBgTP = m_goInstancePassUIStarTypeBG.GetComponent<TweenPosition>();
            if (starBgTP != null)
                Destroy(starBgTP); //销毁该脚本   
            starBgTP = m_goInstancePassUIStarTypeBG.AddComponent<TweenPosition>();
            starBgTP.from = m_tranInstancePassUIStarTypeBGPosFrom.localPosition;
            starBgTP.to = m_tranInstancePassUIStarTypeBGPosTo.localPosition;
            starBgTP.duration = ScoreDuration;
            starBgTP.style = UITweener.Style.Once;
            starBgTP.eventReceiver = gameObject;
            starBgTP.callWhenFinished = "OnTPEnd";
            starBgTP.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
            starBgTP.enabled = false;
        }
    }

    /// <summary>
    /// 评价SABC
    /// </summary>
    /// <param name="isPlay"></param>
    void PlayInstancePassUIStarType(bool isPlay)
    {
        if (isPlay)
        {
            m_goInstancePassUIStarType.SetActive(true);
            TweenScale starTypeTS = m_goInstancePassUIStarType.GetComponent<TweenScale>();
            if (starTypeTS != null)
                starTypeTS.enabled = true;
        }
        else
        {
            TweenScale starTypeTS = m_goInstancePassUIStarType.GetComponent<TweenScale>();
            if (starTypeTS != null)
                Destroy(starTypeTS); //销毁该脚本  
            starTypeTS = m_goInstancePassUIStarType.AddComponent<TweenScale>();
            if (m_texInstancePassUIStarType != null && m_texInstancePassUIStarType.mainTexture != null)
            {
                int width = m_texInstancePassUIStarType.mainTexture.width;
                int height = m_texInstancePassUIStarType.mainTexture.height;
                starTypeTS.from = new Vector3(2*width, 2*height, 2);
                starTypeTS.to = new Vector3(1*width, 1*height, 1);
            }          
            starTypeTS.duration = StarTypeDuration;
            starTypeTS.style = UITweener.Style.Once;
            starTypeTS.eventReceiver = gameObject;
            starTypeTS.callWhenFinished = "OnTSEnd";
            starTypeTS.enabled = false;
        }
    }

    /// <summary>
    /// TweenScale播放动画结束调用
    /// </summary>
    /// <param name="ts"></param>
    void OnTSEnd(TweenScale ts)
    {
        if (ts != null)
        {
            ts.enabled = false;         

            //销毁该脚本   
            UnityEngine.Object Script = ts.gameObject.GetComponentsInChildren<TweenScale>(true)[0];
            Destroy(Script);
        }
    }

    /// <summary>
    /// TweenPosition播放动画结束调用
    /// </summary>
    /// <param name="tp"></param>
    void OnTPEnd(TweenPosition tp)
    {
        if (tp != null)
        {
            tp.enabled = false;

            //销毁该脚本   
            UnityEngine.Object Script = tp.gameObject.GetComponentsInChildren<TweenPosition>(true)[0];
            Destroy(Script);
        }
    }

    #endregion

    /// <summary>
    /// Test
    /// </summary>   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayResultOutside();
        }
    }   


    void OnDisabel()
    {
        
    }
}
