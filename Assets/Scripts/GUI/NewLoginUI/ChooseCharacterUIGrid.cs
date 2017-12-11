using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ChooseCharacterUIGrid : MonoBehaviour 
{
    UILabel m_lblName;
    UILabel m_lblDefaultText;
    UILabel m_lblLevel;

    UISprite m_spDefaultImg;
    UISprite m_spHeadImg;

    GameObject m_goEnablePart;
    GameObject m_goDisablePart;

    Transform m_myTransform;

    public int Id = -1;

    void Awake()
    {
        m_myTransform = transform;

        m_lblName = m_myTransform.Find("ChooseCharcterUIGridEnable/ChooseCharcterUIGridName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDefaultText = m_myTransform.Find("ChooseCharcterUIGridDisable/ChooseCharcterUIGridDefaultText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLevel = m_myTransform.Find("ChooseCharcterUIGridEnable/ChooseCharcterUIGridLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_spDefaultImg = m_myTransform.Find("ChooseCharcterUIGridDisable/ChooseCharcterUIGridDefaultImg").GetComponentsInChildren<UISprite>(true)[0];
        m_spHeadImg = m_myTransform.Find("ChooseCharcterUIGridEnable/ChooseCharcterUIGriHeadImg").GetComponentsInChildren<UISprite>(true)[0];

        m_goEnablePart = m_myTransform.Find("ChooseCharcterUIGridEnable").gameObject;
        m_goDisablePart = m_myTransform.Find("ChooseCharcterUIGridDisable").gameObject;
    }

    void Start()
    {
        if (m_strDefaultText != null)
        {
            m_lblDefaultText.text = m_strDefaultText;
        }

        m_lblLevel.text = m_strLevel;
        m_lblName.text = m_strName;

        m_spHeadImg.spriteName = m_strHeadImgName;
    }

    void OnClick()
    {
        EventDispatcher.TriggerEvent<int>(NewLoginUILogicManager.NewLoginUIEvent.CHOOSECHARACTERGRIDUP, Id);
    }

    public void ShowAsEnable(bool isEnable)
    {
        if (!m_goDisablePart || !m_goEnablePart)
            return;
        if (isEnable)
        {
            m_goDisablePart.SetActive(false);
            m_goEnablePart.SetActive(true);
        }
        else
        {
            m_goEnablePart.SetActive(false);
            m_goDisablePart.SetActive(true);
        }
    }

    string m_strName;

    public string Name
    {
        get { return m_strName; }
        set 
        {
            m_strName = value;

            if (m_lblName != null)
            {
                m_lblName.text = m_strName;
            }
        }
    }

    string m_strLevel;

    public string Level
    {
        get { return m_strLevel; }
        set
        {
            m_strLevel = value;

            if (m_lblLevel != null)
            {
                m_lblLevel.text = m_strLevel;
            }
        }
    }

    string m_strDefaultText;

    public string DefaultText
    {
        get { return m_strDefaultText; }
        set
        {
            m_strDefaultText = value;

            if (m_lblDefaultText != null)
            {
                m_lblDefaultText.text = m_strDefaultText;
            }
        }
    }

    string m_strHeadImgName;

    public string HeadImgName
    {
        get { return m_strHeadImgName; }
        set
        {
            m_strHeadImgName = value;

            if (m_spHeadImg != null)
            {
                m_spHeadImg.spriteName = m_strHeadImgName;
            }
        }
    }
 
}
