using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;
using Mogo.GameData;

public class TimeLimitActivityGrid : MogoUIBehaviour 
{
    private UILabel m_lblCDText;
    private UISprite m_spGridFG;
    private UILabel m_lblTitleText;
    private GameObject m_goTimeLimitActivityHasReward;
    private GameObject m_goTimeLimitActivityGridFinishFX;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblCDText = FindTransform("TimeLimitActivityCDText").GetComponentsInChildren<UILabel>(true)[0];
        m_spGridFG = FindTransform("TimeLimitActivityGridFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblTitleText = FindTransform("TimeLimitActivityTitleText").GetComponentsInChildren<UILabel>(true)[0];
        m_goTimeLimitActivityHasReward = FindTransform("TimeLimitActivityHasReward").gameObject;
        m_goTimeLimitActivityGridFinishFX = FindTransform("TimeLimitActivityGridFinishFX").gameObject;
    } 

    #region 事件

    public int Index;
    public int Id = -1;
    bool m_bIsDragging = false;

    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            if (!m_bIsDragging)
            {
                EventDispatcher.TriggerEvent<int>(TimeLimitActivityUILogicManager.TimeLimitActivityUIEvent.ActivityGridUp, Id);
            }

            m_bIsDragging = false;
        }
    }

    void OnDrag(Vector2 v)
    {
        m_bIsDragging = true;
    }

    #endregion

    #region Grid设置

    /// <summary>
    /// CD文本
    /// </summary>
    private MogoCountDown countDown;
    private string m_strCDText;
    public string CDText
    {
        get { return m_strCDText; }
        set
        {
            m_strCDText = value;

            if (m_lblCDText != null)
            {
                m_lblCDText.text = m_strCDText;
                string[] temp = m_strCDText.Split(':');
                if (countDown == null)
                {
                    if (temp.Length == 4)
                    {
                        countDown = new MogoCountDown(m_lblCDText, Int32.Parse(temp[0]), Int32.Parse(temp[1]), Int32.Parse(temp[2]), Int32.Parse(temp[3]), "", "已完成", "任务超时", MogoCountDown.TimeStringType.UpToDay,
                            () =>
                            {
                                EventDispatcher.TriggerEvent(Events.OperationEvent.EventTimesUp, Id);
                            });
                        countDown.SetSplitSign(String.Empty, String.Empty, LanguageData.GetContent(7100), LanguageData.GetContent(7101), LanguageData.GetContent(7102), LanguageData.GetContent(7103));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 图标名称
    /// </summary>
    string m_strGridFGName;
    public string GridFGName
    {
        get { return m_strGridFGName; }
        set
        {
            m_strGridFGName = value;

            if (m_spGridFG != null)
            {
                m_spGridFG.spriteName = m_strGridFGName;
            }
        }
    }

    /// <summary>
    /// 标题
    /// </summary>
    string m_strTitleText;
    public string TitleText
    {
        get { return m_strTitleText; }
        set
        {
            m_strTitleText = value;

            if (m_lblTitleText != null)
            {
                m_lblTitleText.text = m_strTitleText;
            }
        }
    }

    public void SetCountDownStop()
    {
        if (countDown != null)
            countDown.StopCountDown();
    }

    /// <summary>
    /// 设置活动状态-根据不同状态设置UI
    /// </summary>
    /// <param name="status"></param>
    public void SetActivityStatus(ActivityStatus status)
    {
        switch (status)
        {
            case ActivityStatus.HasReward:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.Outline;
                    m_lblCDText.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblCDText.color = SystemUIColorManager.WHITE;

                    m_lblTitleText.effectStyle = UILabel.Effect.Outline;
                    m_lblTitleText.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblTitleText.color = SystemUIColorManager.WHITE;

                    m_goTimeLimitActivityHasReward.SetActive(true);
                    ShowActivityFinishedAnim(false);
                }
                break;
            case ActivityStatus.HasFinished:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.Outline;
                    m_lblCDText.effectColor = SystemUIColorManager.YELLOW_OUTLINE;
                    m_lblCDText.color = SystemUIColorManager.YELLOW;

                    m_lblTitleText.effectStyle = UILabel.Effect.Outline;
                    m_lblTitleText.effectColor = SystemUIColorManager.YELLOW_OUTLINE;
                    m_lblTitleText.color = SystemUIColorManager.YELLOW;

                    m_goTimeLimitActivityHasReward.SetActive(false);
                    ShowActivityFinishedAnim(true);
                }
                break;
            case ActivityStatus.TimeUseUp:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.Outline;
                    m_lblCDText.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblCDText.color = SystemUIColorManager.WHITE;

                    m_lblTitleText.effectStyle = UILabel.Effect.Outline;
                    m_lblTitleText.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblTitleText.color = SystemUIColorManager.WHITE;

                    m_goTimeLimitActivityHasReward.SetActive(false);
                    ShowActivityFinishedAnim(false);
                }
                break;
            case ActivityStatus.OtherStatus:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.None;
                    m_lblCDText.color = SystemUIColorManager.BROWN;

                    m_lblTitleText.effectStyle = UILabel.Effect.None;
                    m_lblTitleText.color = SystemUIColorManager.BROWN;

                    m_goTimeLimitActivityHasReward.SetActive(false);
                    ShowActivityFinishedAnim(false);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 显示或隐藏限时活动完成特效
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="id"></param>
    private void ShowActivityFinishedAnim(bool isShow)
    {
        if (isShow)
        {
            if (TimeLimitActivityUIViewManager.Instance != null)
            {
                TimeLimitActivityUIViewManager.Instance.AttachFXToTimeLimitActivityUI("fx_ui_skill_yes.prefab", Id, Index);
            }            
        }
        else
        {
            if (TimeLimitActivityUIViewManager.Instance != null)
            {
                TimeLimitActivityUIViewManager.Instance.ReleaseFXFromActivityUI(Id);
            }            
        }
    }

    void OnDisable()
    {
        //ShowActivityFinishedAnim(false);
    }

    #endregion
}
