using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;
using TDBID = System.UInt64;
public class FriendQuestAddGrid : MonoBehaviour 
{

    UISprite m_spFriendHead;
    UILabel m_lblFriendName;
    UILabel m_lblFriendLevel;
    UILabel m_lblText;

    Transform m_myTransform;

    GameObject m_goRegectButton;
    GameObject m_goAcceptButton;

    public TDBID Id;

    string m_strFriendHeadImg;

    public string FriendHeadImg
    {
        get { return m_strFriendHeadImg; }
        set 
        { 
            m_strFriendHeadImg = value;

            if (m_spFriendHead != null)
            {
                m_spFriendHead.spriteName = m_strFriendHeadImg;
            }
        }
    }

    string m_strFriendName;

    public string FriendName
    {
        get { return m_strFriendName; }
        set
        {
            m_strFriendName = value;

            if (m_lblFriendName != null)
            {
                m_lblFriendName.text = m_strFriendName;
            }
        }
    }

    string m_strFriendLevel;

    public string FriendLevel
    {
        get { return m_strFriendLevel; }
        set 
        {
            m_strFriendLevel = value;

            if (m_lblFriendLevel != null)
            {
                m_lblFriendLevel.text = m_strFriendLevel;
            }
        }
    }

    void OnRegectButtonUp()
    {
        EventDispatcher.TriggerEvent<TDBID>(SocietyUILogicManager.SocietyUIEvent.REGECTADDFRIENDUP, Id);
    }

    void OnAcceptButtonUp()
    {
        EventDispatcher.TriggerEvent<TDBID>(SocietyUILogicManager.SocietyUIEvent.ACCEPTADDFRIENDUP, Id);
    }

    void Awake()
    {
        m_myTransform = transform;

        m_spFriendHead = m_myTransform.Find("FriendHeadImg").GetComponentsInChildren<UISprite>(true)[0];
        m_lblFriendLevel = m_myTransform.Find("FriendLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblFriendName = m_myTransform.Find("FriendName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblText = m_myTransform.Find("AddFriendQuestText").GetComponentsInChildren<UILabel>(true)[0];
        m_goRegectButton = m_myTransform.Find("RegectButton").gameObject;
        m_goAcceptButton = m_myTransform.Find("AcceptButton").gameObject;

        m_goRegectButton.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRegectButtonUp;
        m_goAcceptButton.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnAcceptButtonUp;

        I18n();
    }

    void Start()
    {
        m_spFriendHead.spriteName = m_strFriendHeadImg;
        m_lblFriendLevel.text = m_strFriendLevel;
        m_lblFriendName.text = m_strFriendName;
    }

    public void Release()
    {
        m_goRegectButton.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnRegectButtonUp;
        m_goAcceptButton.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnAcceptButtonUp;
    }

    private void I18n()
    {
        m_lblText.text = LanguageData.GetContent(ContentDefine.Friend.REQ_U_AS_FRIEND);//dataMap[ContentDefine.Friend.REQ_U_AS_FRIEND].Format();
    }
}
