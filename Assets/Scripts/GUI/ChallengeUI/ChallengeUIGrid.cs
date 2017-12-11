using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;
using Mogo.GameData;

public class ChallengeUIGrid : MonoBehaviour
{
    public int id;

    protected int timesLeft;
    protected string countdownBeginningString;

    private UILabel challengeName;
    private UILabel challengeMessage;
    private UISprite challengeImg;
    private GameObject m_goChallengeGridActiveFX;

    private float elapseTime;
    private MogoCountDown countDown;
    private GameObject m_goFlashSurface;

    void Awake()
    {
        challengeName = transform.Find("GOChallengeGridTitle/ChallengeGridName").GetComponent<UILabel>();
        challengeMessage = transform.Find("ChallengeGridMessage").GetComponent<UILabel>();
        challengeMessage.color = SystemUIColorManager.BROWN;
        challengeImg = transform.Find("ChallengeGridImage").GetComponent<UISprite>();
        m_goChallengeGridActiveFX = transform.Find("ChallengeGridActiveFX").gameObject;
        m_goFlashSurface = transform.Find("ChallengeGridFlashSurface").gameObject;
    }

    #region 事件
     
    bool m_isDragging = false;

    void OnDrag(Vector2 v)
    {
        m_isDragging = true;
    }

    void OnPress(bool isPressed)
    {
        if (!m_isDragging)
        {
            // OnClick
            if (!isPressed)
                FakeIt();
        }
        else
        {
            if (!isPressed)            
                m_isDragging = false;
        }
    }

    public void FakeIt()
    {
        EventDispatcher.TriggerEvent(Events.ChallengeUIEvent.Enter, id);
    }

    #endregion

    #region 倒计时重载

    private void ReleaseCountDown()
    {
        if (countDown != null)
        {
            countDown.Release();
        }
    }

    public void BeginCountDown(string countingStr, string endStr, int theHour, int times, Action action = null)
    {
        ReleaseCountDown();

        if (theHour != 0)
            challengeMessage.color = SystemUIColorManager.RED;

        countDown = new MogoCountDown(challengeMessage, theHour, 0, 0, countingStr, "", endStr, MogoCountDown.TimeStringType.UpToHour, () =>
            {
                challengeMessage.color = SystemUIColorManager.BROWN;

                if (action != null)
                    action();
            });
    }

    public void BeginCountDown(string countingStr, string endStr, int theHour, int theMinutes, int timesLeft, Action action = null)
    {
        ReleaseCountDown();

        if (theHour != 0 || theMinutes != 0)
            challengeMessage.color = SystemUIColorManager.RED;

        countDown = new MogoCountDown(challengeMessage, theHour, theMinutes, 0, countingStr, "", endStr, MogoCountDown.TimeStringType.UpToHour, () =>
            {
                challengeMessage.color = SystemUIColorManager.BROWN;

                if (action != null)
                    action();
            });
    }

    public void BeginCountDown(string countingStr, string endStr, int theHour, int theMinutes, int theSecond, int timesLeft, Action action = null)
    {
        ReleaseCountDown();

        if (theHour != 0 || theMinutes != 0 || theSecond != 0)
            challengeMessage.color = SystemUIColorManager.RED;

        countDown = new MogoCountDown(challengeMessage, theHour, theMinutes, theSecond, countingStr, "", endStr, MogoCountDown.TimeStringType.UpToHour, () =>
            {
                challengeMessage.color = SystemUIColorManager.BROWN;

                if (action != null)
                    action();
            });
    }

    #endregion

    public void SetCountdownBeginningString(string theCountdownBeginningString)
    {
        if (countDown != null)
            countDown.UpdateCountingText(theCountdownBeginningString);
    }

    private void SetTimesLeftText()
    {
        SetTimesLeftText(LanguageData.GetContent(47120) + timesLeft);
    }

    public void SetTimesLeft(int theTimes)
    {
        timesLeft = theTimes;
        SetTimesLeftText();
    }

    public void SetChallengeText(string text)
    {
        challengeMessage.text = text;
    }

    /// <summary>
    /// 设置挑战Message颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetChallengeMessageColor(Color32 color)
    {
        if (challengeMessage != null)
            challengeMessage.color = color;
    }

    public void SetTimesLeftDecrease()
    {
        if (timesLeft > 0)
            timesLeft--;
        SetTimesLeftText();
    }

    public void SetTimesLeftText(string str)
    {
        if (countDown != null)
            countDown.UpdateEndText(str, true);
    }

    public void SetSplitSign(string hourMinuteStr, string mituteSecondStr)
    {
        if (countDown != null)
            countDown.SetSplitSign(hourMinuteStr, mituteSecondStr);
    }

    public void SetName(string name)
    {
        challengeName.text = name;
    }

    public void SetImg(string path)
    {
        challengeImg.spriteName = path;
        Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" + path);
    }

    public void ShowFlashSurface(bool isShow = true)
    {
        m_goFlashSurface.SetActive(isShow);
    }

    public void SetGray(bool isGray = true)
    {
        LoggerHelper.Debug("It's gray");
        challengeImg.ShowAsWhiteBlack(isGray);
    }

    /// <summary>
    /// 是否显示可进入提示动画
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowEnterTipFX(bool isShow)
    {
        m_goChallengeGridActiveFX.SetActive(isShow);
    }
}
