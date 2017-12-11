using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class LoginRewardGrid : MonoBehaviour
{
    private Transform m_myTransform;

    private UILabel m_lblNewServerRightText;
    private UILabel m_lblOldServerRightText;
    private UISprite m_spLeftGetSign;
    private UISprite m_spRightGetSign;
    private UILabel m_lblOldServerItemName;
    private UILabel m_lblOldServerItemNum;
    private UILabel m_lblOldServerItemCD;
    private UILabel m_lblOldServerCostText;
    private UISprite m_spOldServerCostSign;
    private UISprite m_spOldServerItemFG;
    private UISprite m_spOldServerItemBG;
    private TweenScale m_tsOldServerBuySign;

    private TweenScale m_tsLeftGetSign;
    private TweenScale m_tsRightGetSign;

    private GameObject m_goLoginRewardGridLeftBG;
    private GameObject[] m_arrLeftItem = new GameObject[6];
    private GameObject[] m_arrRightItem = new GameObject[2];
    private UILabel[] m_arrrLeftItemNum = new UILabel[6];
    private GameObject[] m_arrLeftItemGetSigin = new GameObject[6];
    private UISprite[] m_arrLeftItemImg = new UISprite[6];

    private GameObject m_goOldServerPart;
    private GameObject m_goNewServerPart;

    private MogoButton m_mbBuy;
    private MogoButton m_mbGet;

    private Transform m_trans1Pos;
    private Transform m_trans2Pos;
    private Transform m_trans3Pos;
    private Transform m_trans4Pos;

    private Vector3 m_vec1Pos = new Vector3(0, -170, 0);
    private Vector3 m_vec2Pos = new Vector3(-107, -170, 0);

    private GameObject m_goLeftItemList;
    private GameObject m_goRightItemList;

    public int Id = -1;

    private string m_strNewServerRightText;

    private List<InventoryGrid> m_goLeftItemGridList = new List<InventoryGrid>();

    public string NewServerRightText
    {
        get { return m_strNewServerRightText; }
        set
        {
            m_strNewServerRightText = value;

            if (m_lblNewServerRightText != null)
            {
                m_lblNewServerRightText.text = m_strNewServerRightText;
            }
        }
    }

    string m_strOldServerRightText;

    public string OldServerRightText
    {
        get { return m_strOldServerRightText; }
        set
        {
            m_strOldServerRightText = value;

            if (m_lblOldServerRightText != null)
            {
                m_lblOldServerRightText.text = m_strOldServerRightText;
            }
        }
    }

    public void SetOldServerRightTextColor(Color32 color)
    {
        m_lblOldServerRightText.color = color;
    }

    string m_strLeftGetSign;

    public string LeftGetSign
    {
        get { return m_strLeftGetSign; }
        set
        {
            m_strLeftGetSign = value;

            if (m_spLeftGetSign != null)
            {
                m_spLeftGetSign.spriteName = m_strLeftGetSign;
            }
        }
    }

    string m_strRightGetSign;

    public string RightGetSign
    {
        get { return m_strRightGetSign; }
        set
        {
            m_strRightGetSign = value;


            if (m_spRightGetSign != null)
            {
                m_spRightGetSign.spriteName = m_strRightGetSign;
            }
        }
    }

    string m_strOldServerItemName;

    public string OldServerItemName
    {
        get { return m_strOldServerItemName; }
        set
        {
            m_strOldServerItemName = value;

            if (m_lblOldServerItemName != null)
            {
                m_lblOldServerItemName.text = m_strOldServerItemName;
            }
        }
    }

    string m_strOldServerItemNum;

    public string OldServerItemNum
    {
        get { return m_strOldServerItemNum; }
        set
        {
            m_strOldServerItemNum = value;

            if (m_lblOldServerItemNum != null)
            {
                m_lblOldServerItemNum.text = m_strOldServerItemNum;
            }
        }
    }

    string m_strOldServerItemCD;

    public string OldServerItemCD
    {
        get { return m_strOldServerItemCD; }
        set
        {
            m_strOldServerItemCD = value;

            if (m_lblOldServerItemCD != null)
            {
                m_lblOldServerItemCD.text = m_strOldServerItemCD;
            }
        }
    }

    string m_strOldServerCostText;

    public string OldServerCostText
    {
        get { return m_strOldServerCostText; }
        set
        {
            m_strOldServerCostText = value;

            if (m_lblOldServerCostText != null)
            {
                m_lblOldServerCostText.text = "x" + m_strOldServerCostText;
            }
        }
    }

    string m_strOldServerCostSign;

    public string OldServerCostSign
    {
        get { return m_strOldServerCostSign; }
        set
        {
            m_strOldServerCostSign = value;

            if (m_spOldServerCostSign != null)
            {
                m_spOldServerCostSign.spriteName = m_strOldServerCostSign;
            }
        }
    }

    int m_strOldServerItemFG;

    public int OldServerItemFG
    {
        get { return m_strOldServerItemFG; }
        set
        {
            m_strOldServerItemFG = value;

            if (m_spOldServerItemFG != null)
            {
                // InventoryManager.SetIcon(m_strOldServerItemFG, m_spOldServerItemFG);
                InventoryManager.SetIcon(m_strOldServerItemFG, m_spOldServerItemFG, 1, null, m_spOldServerItemBG);
            }
        }
    }

    bool m_bIsOldServer;

    public bool IsOldServer
    {
        get { return m_bIsOldServer; }
        set
        {
            m_bIsOldServer = value;

            if (m_goNewServerPart != null)
            {
                if (m_bIsOldServer)
                {
                    m_goNewServerPart.SetActive(!m_bIsOldServer);
                    m_goOldServerPart.SetActive(m_bIsOldServer);
                }
            }
        }
    }

    List<KeyValuePair<int, int>> m_listLeftItem = new List<KeyValuePair<int,int>>();

    public List<KeyValuePair<int, int>> ListLeftItem
    {
        get { return m_listLeftItem; }
        set
        {
            m_listLeftItem = value;


            for (int i = 0; i < 6; ++i)
            {
                if (i < m_listLeftItem.Count)
                {
                    m_arrLeftItem[i].SetActive(true);

                    // m_arrLeftItem[i].transform.FindChild("LoginRewardGridLeftItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0].spriteName = m_listLeftItem[i].Key;

                    InventoryManager.SetIcon(m_listLeftItem[i].Key, m_arrLeftItem[i].transform.Find("LoginRewardGridLeftItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0], m_listLeftItem[i].Value, m_arrLeftItem[i].transform.Find("LoginRewardGridLeftItem" + i + "Num").GetComponentsInChildren<UILabel>(true)[0], m_arrLeftItem[i].transform.Find("LoginRewardGridLeftItem" + i + "BG").GetComponentsInChildren<UISprite>(true)[0]);

                    if (m_listLeftItemColor != null && m_listLeftItemColor.Count > 0)
                    {
                        int color = m_listLeftItemColor[i];

                        if (color == -1) return;

                        switch (color)
                        {
                            case 0: m_arrLeftItemImg[i].ShowAsWhiteBlack(false); break;
                            case 1: m_arrLeftItemImg[i].ShowAsLakeBlue(); break;
                            case 2: m_arrLeftItemImg[i].ShowAsGreen(); break;
                            case 3: m_arrLeftItemImg[i].ShowAsDeepBlue(); break;
                            case 4: m_arrLeftItemImg[i].ShowAsPurpose(); break;
                            case 5: m_arrLeftItemImg[i].ShowAsOrange(); break;
                            case 6: m_arrLeftItemImg[i].ShowAsRed(); break;
                            case 7: m_arrLeftItemImg[i].ShowAsYellow(); break;
                            case 8: m_arrLeftItemImg[i].ShowAsRoseRed(); break;
                            case 9: m_arrLeftItemImg[i].ShowAsGrassGreen(); break;
                            default: break;
                        }
                    }
                }
                else
                {
                    m_arrLeftItem[i].SetActive(false);
                }
            }

            switch (m_listLeftItem.Count)
            {
                case 1:
                    {
                        m_goLeftItemList.transform.localPosition = m_trans1Pos.localPosition;
                        m_goLoginRewardGridLeftBG.transform.localScale = new Vector3(1, 0.8f, 1);
                        m_goLoginRewardGridLeftBG.transform.localPosition = new Vector3(m_goLoginRewardGridLeftBG.transform.localPosition.x, -20, m_goLoginRewardGridLeftBG.transform.localPosition.z);
                    }
                    break;

                case 2:
                    {
                        m_goLeftItemList.transform.localPosition = m_trans2Pos.localPosition;
                        m_goLoginRewardGridLeftBG.transform.localScale = new Vector3(1, 0.8f, 1);
                        m_goLoginRewardGridLeftBG.transform.localPosition = new Vector3(m_goLoginRewardGridLeftBG.transform.localPosition.x, -20, m_goLoginRewardGridLeftBG.transform.localPosition.z);
                    }
                    break;

                case 3:
                    {
                        m_goLeftItemList.transform.localPosition = m_trans3Pos.localPosition;
                        m_goLoginRewardGridLeftBG.transform.localScale = new Vector3(1, 0.8f, 1);
                        m_goLoginRewardGridLeftBG.transform.localPosition = new Vector3(m_goLoginRewardGridLeftBG.transform.localPosition.x, -20, m_goLoginRewardGridLeftBG.transform.localPosition.z);
                    }
                    break;

                default:
                    {
                        m_goLeftItemList.transform.localPosition = m_trans4Pos.localPosition;
                        m_goLoginRewardGridLeftBG.transform.localScale = new Vector3(1, 1, 1);
                        m_goLoginRewardGridLeftBG.transform.localPosition = new Vector3(m_goLoginRewardGridLeftBG.transform.localPosition.x, 0, m_goLoginRewardGridLeftBG.transform.localPosition.z);
                    }
                    break;
            }
        }
    }

    List<string> m_listRightItem = new List<string>();

    public List<string> ListRightItem
    {
        get { return m_listRightItem; }
        set
        {
            m_listRightItem = value;

            for (int i = 0; i < 2; ++i)
            {
                if (i < m_listRightItem.Count)
                {
                    m_arrRightItem[i].SetActive(true);
                    m_arrRightItem[i].transform.Find("LoginRewardGridLeftItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0].spriteName = m_listRightItem[i];
                }
                else
                {
                    m_arrRightItem[i].SetActive(false);
                }
            }

            switch (m_listRightItem.Count)
            {
                case 1:
                    m_goRightItemList.transform.localPosition = m_vec1Pos;
                    break;

                case 2:
                    m_goRightItemList.transform.localPosition = m_vec2Pos;
                    break;
            }
        }
    }

    public List<int> ListRightItemID
    {
        get
        {
            List<int> temp = new List<int>();
            foreach (var grid in m_goLeftItemGridList)
            {
                temp.Add(grid.iconID);
            }
            return temp;
        }

        set
        {
            int count = m_goLeftItemGridList.Count > value.Count ? value.Count : m_goLeftItemGridList.Count;
            for (int i = 0; i < count; i++)
            {
                m_goLeftItemGridList[i].iconID = value[i];
            }
        }
    }

    protected InventoryGrid m_oldServerItemFGImgGrid;
    public int OldServerItemFGImgID
    {
        get
        {
            return m_oldServerItemFGImgGrid.iconID;
        }
        set
        {
            m_oldServerItemFGImgGrid.iconID = value;
        }
    }

    List<int> m_listLeftItemColor = new List<int>();

    public List<int> ListLeftItemColor
    {
        get { return m_listLeftItemColor; }
        set
        {
            m_listLeftItemColor = value;
        }
    }

    List<string> m_listLeftItemBG = new List<string>();

    public List<string> ListLeftItemBG
    {
        get { return m_listLeftItemBG; }
        set
        {
            m_listLeftItemBG = value;


            for (int i = 0; i < 6; ++i)
            {
                if (i < m_listLeftItemBG.Count)
                {
                    m_arrLeftItem[i].SetActive(true);

                    m_arrLeftItem[i].transform.Find("LoginRewardGridLeftItem" + i + "BG").GetComponentsInChildren<UISprite>(true)[0].spriteName = m_listLeftItemBG[i];
                }
                else
                {
                    m_arrLeftItem[i].SetActive(false);
                }
            }
        }
    }

    List<int> m_listLeftItemNum = new List<int>();

    public List<int> ListLeftItemNum
    {
        get { return m_listLeftItemNum; }
        set
        {
            m_listLeftItemNum = value;

            for (int i = 0; i < 6; ++i)
            {
                if (i < m_listLeftItemNum.Count)
                {
                    m_arrrLeftItemNum[i].text = m_listLeftItemNum[i].ToString();
                }
            }
        }
    }

    void Awake()
    {
        m_myTransform = transform;

        m_lblNewServerRightText = m_myTransform.Find(
            "LoginRewardGridRight/LoginRewardGridRightNewServer/LoginRewardGridRightNewServerTitle").GetComponentsInChildren<UILabel>(true)[0];

        m_lblOldServerRightText = m_myTransform.Find(
            "LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerTitle").GetComponentsInChildren<UILabel>(true)[0];

        m_goLoginRewardGridLeftBG = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftBGGO").gameObject;
        m_spLeftGetSign = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftGetSign").GetComponentsInChildren<UISprite>(true)[0];
        m_spRightGetSign = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightNewServer/LoginRewardGridRightGetSign").GetComponentsInChildren<UISprite>(true)[0];
        m_lblOldServerItemName = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerItemName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOldServerItemNum = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerItemNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOldServerItemCD = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerItemCD").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOldServerCostText = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerItemCostNum").GetComponentsInChildren<UILabel>(true)[0];
        m_spOldServerCostSign = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerCostImg").GetComponentsInChildren<UISprite>(true)[0];
        m_spOldServerItemFG = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerItemFG").GetComponentsInChildren<UISprite>(true)[0];
        //m_spOldServerItemFG.selfRefresh = false;
        m_spOldServerItemBG = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerItemBG").GetComponentsInChildren<UISprite>(true)[0];
        m_tsOldServerBuySign = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerBuySign").GetComponentsInChildren<TweenScale>(true)[0];

        m_tsLeftGetSign = m_spLeftGetSign.GetComponentsInChildren<TweenScale>(true)[0];
        m_tsRightGetSign = m_spRightGetSign.GetComponentsInChildren<TweenScale>(true)[0];

        m_trans1Pos = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList1Pos");
        m_trans2Pos = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList2Pos");
        m_trans3Pos = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList3Pos");
        m_trans4Pos = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList4Pos");

        m_goLeftItemList = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList").gameObject;
        m_goRightItemList = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightNewServer/LoginRewardGridRightNewServerItemList").gameObject;

        m_goOldServerPart = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer").gameObject;
        m_goNewServerPart = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightNewServer").gameObject;

        m_mbBuy = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerBuyBtn").GetComponentsInChildren<MogoButton>(true)[0];
        m_mbGet = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftGetBtn").GetComponentsInChildren<MogoButton>(true)[0];

        m_mbBuy.clickHandler += OnBuyBtnUp;
        m_mbGet.clickHandler += OnGetBtnUp;

        for (int i = 0; i < 6; ++i)
        {
            m_arrLeftItem[i] = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList/LoginRewardGridLeftItem" + i).gameObject;
            m_arrrLeftItemNum[i] = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList/LoginRewardGridLeftItem" + i
                + "/LoginRewardGridLeftItem" + i + "Num").GetComponentsInChildren<UILabel>(true)[0];
            m_arrLeftItemGetSigin[i] = m_myTransform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList/LoginRewardGridLeftItem" + i
                + "/LoginRewardGridLeftItem" + i + "GetSign").gameObject;
            m_arrLeftItemImg[i] = m_arrLeftItem[i].transform.Find("LoginRewardGridLeftItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0];
            //m_arrLeftItemImg[i].selfRefresh = false;
            m_goLeftItemGridList.Add(m_arrLeftItem[i].AddComponent<InventoryGrid>());
        }

        for (int i = 0; i < 2; ++i)
        {
            m_arrRightItem[i] = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightNewServer/LoginRewardGridRightNewServerItemList/LoginRewardGridLeftItem" + i).gameObject;
        }

        m_oldServerItemFGImgGrid = m_myTransform.Find("LoginRewardGridRight/LoginRewardGridRightOldServer/LoginRewardGridRightOldServerItemBG").gameObject.AddComponent<InventoryGrid>(); ;
    }

    public void Release()
    {
        m_mbBuy.clickHandler -= OnBuyBtnUp;
        m_mbGet.clickHandler -= OnGetBtnUp;
    }

    /// <summary>
    /// �������
    /// </summary>
    void OnBuyBtnUp()
    {
        EventDispatcher.TriggerEvent<int>("LoginRewardUIBuyBtnUp", Id);
    }

    /// <summary>
    /// �����ȡ
    /// </summary>
    void OnGetBtnUp()
    {
        EventDispatcher.TriggerEvent<int>("LoginRewardUIGetBtnUp", Id);
    }

    #region "����ȡ" "�ѹ���"

    /// <summary>
    /// �Ƿ���ʾ"����ȡ"���
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowLeftGetSign(bool isShow)
    {
        for (int i = 0; i < m_listLeftItem.Count; i++)
        {
            m_arrLeftItemGetSigin[i].SetActive(isShow);
        }
    }

    /// <summary>
    /// �������"����ȡ"����
    /// </summary>
    public void PlayLeftGetSignAnim()
    {
        for (int i = 0; i < m_listLeftItem.Count; i++)
        {
            TweenScale ts = m_arrLeftItemGetSigin[i].GetComponentsInChildren<TweenScale>(true)[0];
            ts.gameObject.SetActive(true);
            ts.Reset();
            ts.enabled = true;
            ts.Play(true);
        }
    }

    /// <summary>
    /// Old Code
    /// </summary>
    public void PlayRightGetSignAnim()
    {
        m_tsRightGetSign.gameObject.SetActive(true);
        m_tsRightGetSign.Reset();
        m_tsRightGetSign.enabled = true;
        m_tsRightGetSign.Play(true);
    }

    /// <summary>
    /// �Ƿ���ʾ"�ѹ���"���
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowRightBuySign(bool isShow)
    {
        m_tsOldServerBuySign.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// �����Ҳ�"�ѹ���"����
    /// </summary>
    public void PlayRightBuyAnim()
    {
        m_tsOldServerBuySign.gameObject.SetActive(true);
        m_tsOldServerBuySign.Reset();
        m_tsOldServerBuySign.enabled = true;
        m_tsOldServerBuySign.Play(true);
    }

    /// <summary>
    /// �Ƿ���ʾ��ȡ��ť
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowGetBtn(bool isShow)
    {
        m_mbGet.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// �Ƿ���ʾ����ť
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBuyBtn(bool isShow)
    {
        m_mbBuy.gameObject.SetActive(isShow);
    }

    #endregion

    void OnDrag(Vector2 vec)
    {
        LoginRewardUIViewManager.Instance.ShowLoginRewardGrid();
    }
}
