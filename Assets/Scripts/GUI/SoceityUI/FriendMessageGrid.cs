using UnityEngine;
using System.Collections;
using Mogo.Util;

using TDBID = System.UInt64;
using Mogo.GameData;

public class FriendMessageGrid : MonoBehaviour
{
    private Transform m_myTransform;

    private UISprite m_spFriendHead;
    private UILabel m_lblFriendLevel;
    private UILabel m_lblFightingPower;
    private UILabel m_lblFriendName;
    private UISprite[] m_ArrFriendDegree = new UISprite[6];

    private UISprite m_spLeaveMessageSign;
    private UISprite m_spWishStrenthSign;
    private UILabel m_lblFriendHasWishTip;    

    private bool m_bIsDragging = false;
    private bool m_bAllFriendDegreeLoaded = false;

    public TDBID Id;

    void Awake()
    {
        m_myTransform = transform;

        m_spFriendHead = m_myTransform.Find("FriendHeadImage").GetComponentsInChildren<UISprite>(true)[0];
        m_lblFriendLevel = m_myTransform.Find("FriendLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblFightingPower = m_myTransform.Find("FightingPower").GetComponentsInChildren<UILabel>(true)[0];
        m_lblFriendName = m_myTransform.Find("FriendName").GetComponentsInChildren<UILabel>(true)[0];
        m_spLeaveMessageSign = m_myTransform.Find("LeaveMessageSign").GetComponentsInChildren<UISprite>(true)[0];
        m_spWishStrenthSign = m_myTransform.Find("WishStrenthSign").GetComponentsInChildren<UISprite>(true)[0];
        m_lblFriendHasWishTip = m_myTransform.Find("FriendHasWishTip").GetComponentsInChildren<UILabel>(true)[0];

        for (int i = 0; i < 6; ++i)
        {
            m_ArrFriendDegree[i] = m_myTransform.Find("FriendDegreeList/Degree" + i).GetComponentsInChildren<UISprite>(true)[0];
            if (i == 6)
            {
                m_bAllFriendDegreeLoaded = true;
            }
        }

        m_spLeaveMessageSign.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnLeaveMessageSignUp;
        m_spWishStrenthSign.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnWishStrenthSignUp;

        // ChineseData
        m_lblFriendHasWishTip.text = LanguageData.GetContent(ContentDefine.Friend.WISH_BUTTON_WISHED);
    }

    void Start()
    {
        m_spFriendHead.spriteName = m_strFriendHeadImg;
        m_lblFriendLevel.text = m_strFriendLevel;
        m_lblFightingPower.text = m_strFightingPower;
        m_lblFriendName.text = m_strFriendName;

        for (int i = 0; i < m_ArrFriendDegree.Length; ++i)
        {
            if (i < m_iDegreeNum)
            {
                m_ArrFriendDegree[i].gameObject.SetActive(true);
            }
            else
            {
                m_ArrFriendDegree[i].gameObject.SetActive(false);
            }
        }

        LevelMessage = LevelMessage;
        WishStrenth = WishStrenth;
    }

    #region �¼�

    public void Release()
    {
        m_spLeaveMessageSign.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnLeaveMessageSignUp;
        m_spWishStrenthSign.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnWishStrenthSignUp;
    }

    void OnLeaveMessageSignUp()
    {
        EventDispatcher.TriggerEvent<TDBID>(SocietyUILogicManager.SocietyUIEvent.LEAVEMESSAGESIGNUP, Id);
        LevelMessage = false;
    }

    void OnWishStrenthSignUp()
    {
        EventDispatcher.TriggerEvent<TDBID>(SocietyUILogicManager.SocietyUIEvent.WISHSTRENTHSIGNUP, Id);
        WishStrenth = false;
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
                EventDispatcher.TriggerEvent<TDBID>(SocietyUILogicManager.SocietyUIEvent.FRIENDMESSAGEGRIDUP, Id);

            m_bIsDragging = false;
        }
    }

    #endregion

    #region ������Ϣ

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

    string m_strFightingPower;
    public string FightingPower
    {
        get { return m_strFightingPower; }
        set
        {
            m_strFightingPower = value;

            if (m_lblFightingPower != null)
            {
                m_lblFightingPower.text = m_strFightingPower;
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

    int m_iDegreeNum;
    public int DegreeNum
    {
        get { return m_iDegreeNum; }
        set
        {
            m_iDegreeNum = value;

            if (m_ArrFriendDegree.Length > 0 && m_bAllFriendDegreeLoaded)
            {
                for (int i = 0; i < m_ArrFriendDegree.Length; ++i)
                {
                    if (i < m_iDegreeNum)
                    {
                        m_ArrFriendDegree[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        m_ArrFriendDegree[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    bool m_bLevelMessage;
    public bool LevelMessage
    {
        get { return m_bLevelMessage; }
        set
        {
            m_bLevelMessage = value;
            ShowFriendHasWishTip();

            if (m_spLeaveMessageSign != null)
            {
                m_spLeaveMessageSign.gameObject.SetActive(m_bLevelMessage);
            }
        }
    }

    bool m_bWishStrenth;
    public bool WishStrenth
    {
        get { return m_bWishStrenth; }
        set 
        {
            m_bWishStrenth = value;
            ShowFriendHasWishTip();

            if (m_spWishStrenthSign != null)
            {
                m_spWishStrenthSign.gameObject.SetActive(m_bWishStrenth);
            }
        }
    }

    bool m_bHasWish;
    public bool HasWish
    {
        get { return m_bHasWish; }
        set
        {
            m_bHasWish = value;
            ShowFriendHasWishTip();
        }
    }

    /// <summary>
    /// �Ƿ���ʾ"������ף��"Tip
    /// </summary>
    /// <returns></returns>
    private void ShowFriendHasWishTip()
    {
        if(m_lblFriendHasWishTip != null)
        {
            if (LevelMessage || WishStrenth)
                m_lblFriendHasWishTip.gameObject.SetActive(false);
            else if(HasWish)
                m_lblFriendHasWishTip.gameObject.SetActive(true);
            else
                m_lblFriendHasWishTip.gameObject.SetActive(false);
        }           
    }

    #endregion
}
