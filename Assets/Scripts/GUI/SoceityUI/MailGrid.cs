using UnityEngine;
using System.Collections;
using Mogo.Util;
using TDBID = System.UInt64;

public class MailGrid : MonoBehaviour 
{
    UILabel m_lblMailGridName;
    UILabel m_lblMailGridTopic;
    UILabel m_lblMailGridTime;
    UISprite m_spMailGridSignAttachNoRead;
    UISprite m_spMailGridSignAttachRead;
    UISprite m_spMailGridSignNoRead;
    UISprite m_spMailGridSignRead;

    string m_strMailGridName;

    Transform m_myTransform;

    public int Id = -1;

    private bool m_bIsDragging = false;

    public string MailGridName
    {
        get { return m_strMailGridName; }
        set 
        { 
            m_strMailGridName = value;

            if (m_lblMailGridName != null)
            {
                m_lblMailGridName.text = m_strMailGridName;
            }
        }
    }

    string m_strMailGridTopic;

    public string MailGridTopic
    {
        get { return m_strMailGridTopic; }
        set
        { 
            m_strMailGridTopic = value;

            if (m_lblMailGridTopic != null)
            {
                m_lblMailGridTopic.text = m_strMailGridTopic;
            }
        }
    }

    string m_strMailGridTime;
    public string MailGridTime
    {
        get { return m_strMailGridTime; }
        set
        {
            m_strMailGridTime = value;
            if (m_lblMailGridTime != null)
            {
                m_lblMailGridTime.text = m_strMailGridTime;
            }
        }
    }

    bool m_bIsAttachNoRead;

    public bool AttachNoRead
    {
        get { return m_bIsAttachNoRead; }
        set
        {
            m_bIsAttachNoRead = value;

            if (m_spMailGridSignAttachNoRead != null)
            {
                m_spMailGridSignAttachNoRead.gameObject.SetActive(m_bIsAttachNoRead);
            }
        }
    }

    bool m_bIsAttachRead;

    public bool AttachRead
    {
        get { return m_bIsAttachRead; }
        set
        {
            m_bIsAttachRead = value;

            if (m_spMailGridSignAttachRead != null)
            {
                m_spMailGridSignAttachRead.gameObject.SetActive(m_bIsAttachRead);
            }
        }
    }

    bool m_bIsNoRead;

    public bool NoRead
    {
        get { return m_bIsNoRead; }
        set
        {
            m_bIsNoRead = value;

            if (m_spMailGridSignNoRead != null)
            {
                m_spMailGridSignNoRead.gameObject.SetActive(m_bIsNoRead);
            }
        }

    }


    bool m_bIsRead;

    public bool Read
    {
        get { return m_bIsRead; }
        set 
        {
            m_bIsRead = value;

            if (m_spMailGridSignRead != null)
            {
                m_spMailGridSignRead.gameObject.SetActive(m_bIsRead);
            }
        }
    }

    void Awake()
    {
        m_myTransform = transform;

        m_lblMailGridName = m_myTransform.Find("MailGridName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailGridTopic = m_myTransform.Find("MailGridTopic").GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailGridTime = m_myTransform.Find("MailGridTime").GetComponentsInChildren<UILabel>(true)[0];
        m_spMailGridSignAttachNoRead = m_myTransform.Find("MailGridSign/MailGridSignAttachNoRead").GetComponentsInChildren<UISprite>(true)[0];
        m_spMailGridSignAttachRead = m_myTransform.Find("MailGridSign/MailGridSignAttachRead").GetComponentsInChildren<UISprite>(true)[0];
        m_spMailGridSignNoRead = m_myTransform.Find("MailGridSign/MailGridSignNoRead").GetComponentsInChildren<UISprite>(true)[0];
        m_spMailGridSignRead = m_myTransform.Find("MailGridSign/MailGridSignRead").GetComponentsInChildren<UISprite>(true)[0];
    }

    void Start()
    {
        m_lblMailGridName.text = m_strMailGridName;
        m_lblMailGridTopic.text = m_strMailGridTopic;
        m_lblMailGridTime.text = m_strMailGridTime;

        m_spMailGridSignAttachNoRead.gameObject.SetActive(m_bIsAttachNoRead);
        m_spMailGridSignAttachRead.gameObject.SetActive(m_bIsAttachRead);
        m_spMailGridSignNoRead.gameObject.SetActive(m_bIsNoRead);
        m_spMailGridSignRead.gameObject.SetActive(m_bIsRead);
    }


    void OnDrag(Vector2 v)
    {
        m_bIsDragging = true;
    }

    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {

            if (!m_bIsDragging)
            {
                EventDispatcher.TriggerEvent<TDBID>(SocietyUILogicManager.SocietyUIEvent.MAILGRIDUP, (TDBID)Id);
            }


            m_bIsDragging = false;
        }
    }
}
