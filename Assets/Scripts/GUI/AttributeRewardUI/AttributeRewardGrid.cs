using UnityEngine;
using System.Collections;
using Mogo.Util;

public class AttributeRewardGrid : MonoBehaviour 
{
    Transform m_myTransform;

    UISprite m_spSignFG;
    UILabel m_lblSignText;
    UILabel m_lblTitle;
    UILabel m_lblQuest;
    UISprite m_spProcessFG;
    UISprite m_spFinishedSign;
    GameObject m_goShareBtn;
    UILabel m_lblProcessText;

    GameObject m_goRunningSign;

    public int Id = -1;
    Vector3 defaultScale;

    void Awake()
    {
        m_myTransform = transform;

        m_spSignFG = m_myTransform.Find("AttributeRewardGridSignFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblSignText = m_myTransform.Find("AttributeRewardGridSignText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblTitle = m_myTransform.Find("AttributeRewardGridTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblQuest = m_myTransform.Find("AttributeRewardGridQuest").GetComponentsInChildren<UILabel>(true)[0];
        m_spProcessFG = m_myTransform.Find("AttributeRewardGridProcessFG").GetComponentsInChildren<UISprite>(true)[0];
        m_spFinishedSign = m_myTransform.Find("AttributeRewardFinishedSign").GetComponentsInChildren<UISprite>(true)[0];
        m_lblProcessText = m_myTransform.Find("AttributeRewardGridProcessText").GetComponentsInChildren<UILabel>(true)[0];
        m_goShareBtn = m_myTransform.Find("AttributeRewardShareBtn").gameObject;
        m_goRunningSign = m_myTransform.Find("AttributeRewardGridRunningText").gameObject;

        m_myTransform.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnShareBtnUp;

        defaultScale = m_spProcessFG.transform.localScale;
    }

    void Start()
    {
        if (m_lblQuest.text == null)
        {
            m_spSignFG.spriteName = m_strSignFGImg;
            m_lblSignText.text = m_strSignText;
            m_lblTitle.text = m_strTitle;
            m_lblQuest.text = m_strQuest;
            m_spFinishedSign.gameObject.SetActive(m_bIsFinished);
            m_goShareBtn.SetActive(m_bIsShare);
            //m_spProcessFG.transform.localScale = new Vector3(
            //            m_spProcessFG.transform.localScale.x * m_iProcessStatus * 0.01f,
            //            m_spProcessFG.transform.localScale.y,
            //            m_spProcessFG.transform.localScale.z); ;
            //m_spProcessFG.fillAmount = m_iProcessStatus * 0.01f;
            m_lblProcessText.text = m_strProcessText;
            m_goRunningSign.SetActive(m_bIsRunning);


            float currentScale = m_iProcessStatus * 0.01f * 558f;

            if (currentScale == 0)
            {
                currentScale = 0.01f;
            }

            m_spProcessFG.transform.localScale = new Vector3(currentScale, m_spProcessFG.transform.localScale.y, m_spProcessFG.transform.localScale.z);
        }
    }

    public void Release()
    {
        m_myTransform.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnShareBtnUp;
    }

    void OnShareBtnUp()
    {
        EventDispatcher.TriggerEvent(AttributeRewardLogicManager.AttributeRewardUIEvent.AttributeGridShareBtnUp, Id);
    }

    string m_strSignFGImg;

    public string SignFGImg
    {
        get { return m_strSignFGImg; }
        set 
        {
            m_strSignFGImg = value;

            if (m_spSignFG != null)
            {
                m_spSignFG.spriteName = m_strSignFGImg;
            }
        }
    }

    string m_strSignText;

    public string SignText
    {
        get { return m_strSignText; }
        set
        {
            m_strSignText = value;

            if (m_lblSignText != null)
            {
                m_lblSignText.text = m_strSignText;
            }
        }
    }

    string m_strTitle;

    public string Title
    {
        get { return m_strTitle; }
        set 
        { 
            m_strTitle = value;

            if (m_lblTitle != null)
            {
                m_lblTitle.text = m_strTitle;
            }
        }
    }

    string m_strQuest;

    public string Quest
    {
        get { return m_strQuest; }
        set 
        { 
            m_strQuest = value;

            if (m_lblQuest != null)
            {
                m_lblQuest.text = m_strQuest;
            }
        }
    }



    int m_iProcessStatus;

    public int ProcessStatus
    {
        get { return m_iProcessStatus; }
        set
        {
            //m_iProcessStatus = value <= 0 ? 1 : value;

            if (m_spProcessFG != null)
            {
                //m_spProcessFG.transform.localScale = new Vector3(
                //    defaultScale.x * m_iProcessStatus * 0.01f,
                //    defaultScale.y, defaultScale.z);

                //Mogo.Util.LoggerHelper.Debug("m_spProcessFG.transform.localScale: " + m_spProcessFG.transform.localScale);

                m_iProcessStatus = value;

                //m_spProcessFG.fillAmount = m_iProcessStatus * 0.01f;

                float currentScale = m_iProcessStatus * 0.01f * 553;

                if(currentScale ==0)
                {
                    currentScale = 0.01f ;
                }

                m_spProcessFG.transform.localScale = new Vector3(currentScale, m_spProcessFG.transform.localScale.y, m_spProcessFG.transform.localScale.z);

            }
        }
    }

    bool m_bIsFinished;

    public bool IsFinished
    {
        get { return m_bIsFinished; }
        set
        {
            m_bIsFinished = value;

            if (m_spFinishedSign != null)
            {
                m_spFinishedSign.gameObject.SetActive(m_bIsFinished);
            }
        }
    }

    bool m_bIsShare;

    public bool IsShare
    {
        get { return m_bIsShare; }
        set 
        {
            m_bIsShare = value;

            if (m_goShareBtn != null)
            {
                m_goShareBtn.SetActive(m_bIsShare);
            }
        }
    }

    bool m_bIsRunning;

    public bool IsRunning
    {
        get { return m_bIsRunning; }
        set
        {
            m_bIsRunning = value;

            if (m_goRunningSign != null)
            {
                m_goRunningSign.SetActive(m_bIsRunning);
            }
        }
    }

    string m_strProcessText;

    public string ProcessText
    {
        get { return m_strProcessText; }
        set
        {
            m_strProcessText = value;

            if (m_lblProcessText != null)
            {
                m_lblProcessText.text = m_strProcessText;
            }
        }
    }




}
