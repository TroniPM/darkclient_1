using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;

public class MogoBloodAnim : MonoBehaviour
{
    public GameObject BloodAnim;
    public GameObject BloodFG;

    TweenScale m_ts;
    UISprite m_spBloodAnim;
    UISprite m_spBloodFG;

    Vector3 m_vec3FGFullSize;
    Vector3 m_vec3FGPos;

    Action m_cb;
    
    bool m_bIsLastTween = false;

    float m_fLeftScaleX;

    int m_iLastLevel = 4;

    float m_fLastFill = 1f;

    void Awake()
    {
        BloodAnim.SetActive(false);
        m_ts = BloodAnim.GetComponentsInChildren<TweenScale>(true)[0];
        m_ts.enabled = false;

        m_spBloodFG = BloodFG.GetComponentsInChildren<UISprite>(true)[0];
        m_spBloodAnim = BloodAnim.GetComponentsInChildren<UISprite>(true)[0];

        m_vec3FGFullSize = BloodFG.transform.localScale;
        m_vec3FGPos = BloodFG.transform.localPosition;
    }

    void OnBloodAnimDone()
    {
        m_fLeftScaleX = 0;
        //m_fLastFill = 1f;
        BloodAnim.SetActive(false);

        m_spBloodAnim.transform.localScale = new Vector3(0.01f, m_spBloodAnim.transform.localScale.y, m_spBloodAnim.transform.localScale.z);

        //Debug.LogError(m_spBloodAnim.name + " " + m_spBloodAnim.transform.localScale);

        if (m_bIsLastTween == true && m_cb != null)
        {
            m_bIsLastTween = false;
           // m_cb(); //Maifeo
        }

    }

    /// <summary>
    /// 计算动画位置
    /// </summary>
    /// <param name="currentFill"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    float CalculateAnimPosX(float currentFill, int level, bool bRightToLeft = true)
    {
        float posX = 0;

        if (level != m_iLastLevel)
        {
            if (bRightToLeft)
            {
                // 从右到左
                posX = m_vec3FGPos.x + (m_vec3FGFullSize.x * currentFill);
            }
            else
            {
                // 从左到右
                posX = m_vec3FGPos.x - (m_vec3FGFullSize.x * currentFill);
            }
        }
        else
        {
            if (bRightToLeft)
            {
                // 从右到左
                posX = m_vec3FGPos.x + (m_vec3FGFullSize.x * currentFill);
            }
            else
            {
                // 从左到右
                posX = m_vec3FGPos.x - (m_vec3FGFullSize.x * currentFill);
            }
        }

        if (Mathf.Abs(currentFill) < 0.001f)
        {
            m_bIsLastTween = true;
        }

        return posX;
    }

    /// <summary>
    /// 计算动画长度
    /// </summary>
    /// <param name="currentFill"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    float CalculateAnimLength(float currentFill,int level)
    {
        float length = 0;

        LoggerHelper.Debug(level + " @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@2 " + m_iLastLevel);

        if (level != m_iLastLevel)
        {
            length = m_vec3FGFullSize.x * (1 - currentFill);
            m_fLeftScaleX = 0;
            m_fLastFill = 1f;

            length = m_vec3FGFullSize.x * (m_spBloodFG.fillAmount - currentFill) + m_fLeftScaleX;
        }
        else
        {
            length = m_vec3FGFullSize.x * (m_fLastFill - currentFill) + m_fLeftScaleX;
        }

        LoggerHelper.Debug(length + " @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ " + m_fLeftScaleX);
        return length;
    }

    public void PlayBloodAnim(float currentFill, int level = 4, bool bRightToLeft = true, Action cb = null)
    {
        LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@2PlayBloodAnim " + currentFill + " " + m_fLastFill);
        if (currentFill > 1)
        {
            return;
        }

        if (m_spBloodAnim != null)
        {
            BloodAnim.SetActive(true);

            if (m_spBloodAnim.transform.localScale.x > 0.1f)
            {
                m_fLeftScaleX = m_spBloodAnim.transform.localScale.x;
            }

            m_ts.Reset();
            m_ts.enabled = true;

            m_ts.callWhenFinished = "OnBloodAnim0Done";
            m_ts.eventReceiver = gameObject;

            m_cb = cb;
            Vector3 pos = BloodAnim.transform.localPosition;

            BloodAnim.transform.localPosition = new Vector3(
                CalculateAnimPosX(currentFill, level, bRightToLeft), pos.y, pos.z);

            BloodAnim.transform.localScale = new Vector3(
                CalculateAnimLength(currentFill, level), BloodAnim.transform.localScale.y, BloodAnim.transform.localScale.z);          

            m_ts.from = BloodAnim.transform.localScale;
            m_ts.to = new Vector3(0.1f, BloodAnim.transform.localScale.y, BloodAnim.transform.localScale.z);

            m_ts.Play(true);

            m_fLastFill = currentFill;
            m_iLastLevel = level;
        }
        LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@2PlayBloodAnim " + currentFill + " " + m_fLastFill);
    }

    void OnEnable()
    {
        OnBloodAnimDone();
    }
}
