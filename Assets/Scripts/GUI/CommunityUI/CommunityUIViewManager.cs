using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public enum CommunityUIParent : byte
{
    MainUI,
    NormalMainUI,
}

public class CommunityLabelInfo
{
    public float height;
    public GameObject go;
}

public class CommunityUIViewManager : MogoUIBehaviour
{
    private static CommunityUIViewManager m_instance;
    public static CommunityUIViewManager Instance { get { return CommunityUIViewManager.m_instance; } }

    CommunityUIParent m_communityUIParent = CommunityUIParent.NormalMainUI;
    public CommunityUIParent CommunityUIParent
    {
        get
        {
            return m_communityUIParent;
        }
        set
        {
            m_communityUIParent = value;
        }
    }

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    //public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    Queue<CommunityLabelInfo> m_queueChatData = new Queue<CommunityLabelInfo>();

    public Action PRIVATECHANNELICONUP;
    public Action TONGCHANNELICONUP;
    public Action WORLDCHANNELICONUP;
    public Action FRIENDBTNUP;
    public Action SENDBTNUP;
    public Action PRIVATETALKBTNUP;
    public Action ADDFRIENDBTNUP;


    GameObject m_goFaceList;
    GameObject m_goFriendList;

    GameObject m_goFriendButton;
    GameObject m_goPrivateInput;
    GameObject m_goWorldInput;

    GameObject m_goAddFriendTip;
    UILabel m_lblAddFriendTipName;

    Transform m_transFaceStartPos;
    Transform m_transFaceEndPos;

    GameObject m_goFriendGridList;
    Camera m_camFriendGirdListCamera;
    Camera m_camWorldChannelCamera;
    Camera m_camTongChannelCamera;
    Camera m_camPrivateChannelCamera;
    Camera m_camInputTextCamera;
    Vector3 m_vec3FriendGridListStartPos = new Vector3(0, -0.084f, 0);

    bool m_bFaceListShown = false;
    bool m_bFriendListShown = false;

    const int FONTSIZE = 22;
    const int FONTSPACE = 10;
    const int LINEWIDTH = 570;

    const float FRIENDGRIDSPACE = 0.042f;

    List<GameObject> m_listChatUIDialogText = new List<GameObject>();
    List<GameObject> m_listFriendGird = new List<GameObject>();

    MogoInput m_inputText;
    string m_strLogicText;
    float m_fOffect;
    int m_iAddSpaceNum = 0;

    private int m_iLineNumWorld = 0;
    private int m_iLineNumTong = 0;
    private int m_iLineNumPrivate = 0;

    private int m_iSpaceNumWorld = 0;
    private int m_iSpaceNumTong = 0;
    private int m_iSpaceNumPrivate = 0;

    private UILabel m_equipDetailNeedLevel;
    private UILabel m_equipDetailGrowLevel;
    private UILabel m_equipDetailNeedJob;
    private UILabel m_equipDetailExtra;
    private UISlicedSprite m_equipDetailImageFG;
    private UILabel[] m_equipDetailDiamondHoleInfo = new UILabel[13];
    private GameObject[] m_arrNewDiamondHoleIcon = new GameObject[4];

    private GameObject m_goEquipmentDetailInfo;

    //private int m_iInputTextCount = 0;  
    string TranslateInputText(string text)
    {
        string result;

        //switch (text)
        //{
        //    case "<info=(1,1,1)>":
        //        result = "[FF00FF]超级大斧[-]";
        //        break;

        //    case "<info=(1,0,0)>":
        //        result = "[FF0000]³¬Œ¶ŽóŽž[-]";
        //        break;

        //    case "<face=(1)>":
        //        result = "{:1}";
        //        break;

        //    default:
        //        result = "";
        //        break;

        //}


        if (text.Length > 7 && text.Substring(0, 7) == "<info=(")
        {
            result = text.Substring(7);
            result = result.Substring(0, result.Length - 2);

            string tmp = "";

            int[] data = new int[5];

            for (int i = 0; i < 5; ++i)
            {
                data[i] = -1;
            }

            for (int i = 0; i < result.Length; ++i)
            {
                if (result[i] != ',')
                {
                    tmp += result[i];
                }
                else
                {
                    break;
                }
            }

            //result = ItemEquipmentData.dataMap[int.Parse(tmp)].Name;
			result = ItemEquipmentData.GetCommunityEquipNameByID(int.Parse(tmp));
            //result = "[FF00FF]" + result + "[-]";

        }
        else if (text.Length > 7 && text.Substring(0, 7) == "<face=(")
        {
            if (text.Substring(8, 1) == ")")
            {
                result = string.Concat("{:", text.Substring(7, 1), "}");
            }
            else
            {
                result = string.Concat("{:", text.Substring(7, 2),"}");
            }

            //result = "{:1}";

        }
        else
        {
            result = "";
        }

        return result;
    }

    public void SetTextCameraPosX(float posX)
    {
        Vector3 pos = m_camInputTextCamera.transform.localPosition;
        m_camInputTextCamera.transform.localPosition = new Vector3(posX, pos.y, pos.z);
    }

    public int GetInputLabelContentLength()
    {
        return m_inputText.GetInputLabelContentCount();
    }

    public void AddChatUIInputText(string text)
    {
        m_inputText.text += text;
        m_inputText.text += TranslateInputText(text);
        //   m_strLogicText = m_inputText.text;

        //   m_inputText.text = SplitViewText(m_inputText.text);
        m_inputText.label.supportEncoding = true;


        //AssetCacheMgr.GetUIInstance("ChatUIEquipmentLabel.prefab", (prefab, id, go) =>
        //       {
        //           GameObject obj = (GameObject)go;
        //           obj.SetActive(true);
        //           obj.transform.parent = m_myTransform.FindChild(m_widgetToFullName["ChatUIInputSLList"]);
        //           obj.transform.localPosition = new Vector3(0, 0, 0);
        //           obj.transform.localScale = new Vector3(1, 1, 1);


        //           obj.transform.GetChild(0).GetComponentsInChildren<UILabel>(true)[0].text = text;
        //           obj.transform.GetChild(0).GetComponentsInChildren<UILabel>(true)[0].supportEncoding = true;

        //           if (text.Length > 7 && (text[0] == '[' && text[7] == ']'))
        //           {
        //                obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
        //               new Vector3((text.Length - 8 - 2) * 22 + 14, 22, 0);

        //                obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center =
        //                  new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.x * 0.5f,0f,0f);

        //                m_lblInputText.text += text.Substring(8);


        //           }
        //           else
        //           {
        //                obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
        //              new Vector3((text.Length - 2) * 22 + 14, 22, 0);

        //                m_lblInputText.text += text;
        //           }


        //       });
    }

    public void EmptyChatUIInput()
    {
        m_inputText.EmptyInput();
        m_fOffect = 0;
        m_iAddSpaceNum = 0;
    }

    public void AddChatUIEquipmentLabel(string text, Transform parent, Action<GameObject> action)
    {
        AssetCacheMgr.GetUIInstance("ChatUIEquipmentLabel.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.SetActive(true);
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);

            UILabel lbl = obj.transform.GetChild(0).GetComponentsInChildren<UILabel>(true)[0];

            lbl.text = text;
            lbl.supportEncoding = true;

            obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
                new Vector3(lbl.font.size * lbl.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x,
                    lbl.font.size * lbl.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).y, 1);

            obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center =
                new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.x * 0.5f, 0f, 0f);

            //if (text.Length > 7 && (text[0] == '[' && text[7] == ']'))
            //{
            //    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
            //   new Vector3((text.Length - 8 - 2) * 22 + 14, 22, 0);

            //    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center =
            //      new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.x * 0.5f, 0f, 0f);

            //    // m_lblInputText.text += text.Substring(8);


            //}
            //else
            //{
            //    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
            //  new Vector3((text.Length - 2) * 22 + 14, 22, 0);

            //    //m_lblInputText.text += text;
            //}
            if (action != null)
                action(obj);
        });
    }

    public void AddChatUIFaceLabel(string text, Transform parent, Action<GameObject> action)
    {
        AssetCacheMgr.GetUIInstance("FaceIconDialog.prefab", (prefab, id, go) =>
        {
            int faceId = 1;

            if (text.Length == 4)
            {
                faceId = int.Parse(text.Substring(2,1))+1;

                //MogoGlobleUIManager.Instance.Info(text.Substring(2,1));
            }
            else if(text.Length == 5)
            {
                faceId = int.Parse(text.Substring(2, 2))+1;
                //MogoGlobleUIManager.Instance.Info(text.Substring(2, 2));
            }

            GameObject obj = (GameObject)go;
            obj.SetActive(true);
            obj.transform.parent = parent;

            UISprite sp = obj.transform.GetComponentsInChildren<UISprite>(true)[0];
            sp.pivot = UIWidget.Pivot.Left;

            sp.transform.localPosition = new Vector3(0, 0, -1);
            //obj.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);
            obj.transform.localScale = new Vector3(0.7f,0.7f, 1);

            sp.spriteName = UIFaceIconData.dataMap[faceId].facefirst;
            obj.GetComponentsInChildren<UISpriteAnimation>(true)[0].namePrefix = UIFaceIconData.dataMap[faceId].facehead;
            //  obj.transform.localScale = new Vector3(0.25f, 0.25f, 0.22f);

            //obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
            //    new Vector3(lbl.font.size * lbl.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x,
            //        lbl.font.size * lbl.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).y, 1);

            //obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center =
            //    new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.x * 0.5f, 0f, 0f);


            if (action != null)
                action(obj);
        });
    }

    void PreLoadResource()
    {
        //AssetCacheMgr.GetUIInstance("ChatUIDialogLabel.prefab", (prefab, id, go) => { Destroy(go); });
        //AssetCacheMgr.GetUIInstance("FaceIcon.prefab", (prefab, id, go) => { Destroy(go); });
        //AssetCacheMgr.GetUIInstance("FaceIconDialog.prefab", (prefab, id, go) => { Destroy(go); });

        AssetCacheMgr.GetUIResource("ChatUIDialogLabel.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("FaceIcon.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("FaceIconDialog.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("ChatUIEquipmentLabel.prefab", (obj) => { });
    }

    /// <summary>
    /// 添加一条聊天信息
    /// </summary>
    /// <param name="text">聊天信息显示的内容</param>
    /// <param name="channel">聊天信息放至的频道</param>
    /// <param name="userId">发送玩家的ID</param>
    /// <param name="channelIcon">聊天信息发送的频道,用于显示频道图标</param>
    /// <param name="LvAndNameString">聊天信息的人名,用于计算下划线的长度</param>
    /// <param name="underLineColor">人名下划线颜色</param>
    /// <param name="lineOffsetX">人名下划线偏移,自己向别的玩家的私聊信息需处理</param>    
    public void AddChatUIDialogText(string text, ChannelId channel, ulong userId,
        ChannelId channelIcon, string LvAndNameString = "", string underLineColor = "13FFD5", int lineOffsetX = 60)
    {
        string name = "WorldChannelDialogLabelList";

        int m_iLineNum = 0;
        int m_iSpaceNum = 0;
        Camera relatedCam = m_camWorldChannelCamera;


        AssetCacheMgr.GetUIInstance("ChatUIDialogLabel.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            
            UILabel lblChatUIDialogLabelText = obj.transform.Find("ChatUIDialogLabelText").GetComponentsInChildren<UILabel>(true)[0];
            lblChatUIDialogLabelText.effectStyle = UILabel.Effect.Outline;

            UISprite spChatUIDialogLabelChannel = obj.transform.Find("ChatUIDialogLabelChannel").GetComponentsInChildren<UISprite>(true)[0];
            SetChatUIDialogLabelChannel(channelIcon, spChatUIDialogLabelChannel);

            // 设置下划线
            UILabel lblChatUIDialogLabelUnderLine = obj.transform.Find("ChatUIDialogLabelUnderLine").GetComponentsInChildren<UILabel>(true)[0];
            float totalUnderline = m_inputText.label.font.CalculatePrintedSize(LvAndNameString, true, UIFont.SymbolStyle.None).x;
            float oneUnderLine = m_inputText.label.font.CalculatePrintedSize("_", true, UIFont.SymbolStyle.None).x;
            int underLineCount = (int)(totalUnderline / oneUnderLine);

            lblChatUIDialogLabelUnderLine.text = "";
            for (int i = 0; i < underLineCount; i++)
            {
                lblChatUIDialogLabelUnderLine.text += "_";
            }

            // 设置下划线颜色
            lblChatUIDialogLabelUnderLine.text = string.Concat("[", underLineColor, "]", lblChatUIDialogLabelUnderLine.text, "[-]");

            switch (channel)
            {
                case ChannelId.WORLD:
                    name = "WorldChannelDialogLabelList";
                    m_iLineNum = m_iLineNumWorld;
                    m_iSpaceNum = m_iSpaceNumWorld;
                    relatedCam = m_camWorldChannelCamera;
                    lblChatUIDialogLabelText.effectColor = new Color32(32, 19, 14, 255);
                    break;

                case ChannelId.UNION:
                    name = "TongChannelDialogLabelList";
                    m_iLineNum = m_iLineNumTong;
                    m_iSpaceNum = m_iSpaceNumTong;
                    relatedCam = m_camTongChannelCamera;
                    lblChatUIDialogLabelText.effectColor = new Color32(0, 0, 0, 255);
                    break;

                case ChannelId.PERSONAL:
                    name = "PrivateChannelDialogLabelList";
                    m_iLineNum = m_iLineNumPrivate;
                    m_iSpaceNum = m_iSpaceNumPrivate;
                    relatedCam = m_camPrivateChannelCamera;
                    lblChatUIDialogLabelText.effectColor = new Color32(13, 0, 16, 255);
                    lblChatUIDialogLabelUnderLine.transform.localPosition = new Vector3(lineOffsetX,
                        lblChatUIDialogLabelUnderLine.transform.localPosition.y,
                        lblChatUIDialogLabelUnderLine.transform.localPosition.z);
                    break;

                case ChannelId.SYSTEM:
                    name = "WorldChannelDialogLabelList";
                    m_iLineNum = m_iLineNumWorld;
                    m_iSpaceNum = m_iSpaceNumWorld;
                    relatedCam = m_camWorldChannelCamera;
                    lblChatUIDialogLabelText.effectColor = new Color32(0, 0, 0, 255);
                    break;
            }
            
            obj.SetActive(true);
            obj.transform.parent = m_myTransform.Find(m_widgetToFullName[name]);
            obj.transform.parent.localScale = new Vector3(0.0008f, 0.0008f, 1);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = relatedCam;

            string strTmp = text;

            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] == '<' && text.Substring(i).Length >= 6 && text.Substring(i, 6) == "<info=")
                {
                    int tmp = i + 7;

                    while (tmp < text.Length)
                    {
                        if (text[tmp] == '>')
                        {
                            float length = m_inputText.label.font.CalculatePrintedSize(text.Substring(0, i), true, UIFont.SymbolStyle.None).x;

                            string originString = text.Substring(i, tmp - i + 1);
                            text = text.ReplaceFirst(originString, TranslateInputText(originString));
                            AddChatUIEquipmentLabel(TranslateInputText(originString),
                                obj.transform.Find("ChatUIDialogLabelSLList"), (g) =>
                                {
                                    float width = m_inputText.label.font.size * length;
                                    int row = (int)((width + m_fOffect) / LINEWIDTH);

                                    float labelWidth = m_inputText.label.font.CalculatePrintedSize(TranslateInputText(originString),
                                        true, UIFont.SymbolStyle.None).x * m_inputText.label.font.size;

                                    if ((width + m_fOffect) + labelWidth > LINEWIDTH * (row + 1))
                                    {
                                        m_iAddSpaceNum = (int)((LINEWIDTH * (row + 1) - (width + m_fOffect)) /
                                        (m_inputText.label.font.CalculatePrintedSize(" ",
                                        true, UIFont.SymbolStyle.None).x * m_inputText.label.font.size));


                                        //m_fOffect += m_inputText.label.font.CalculatePrintedSize("   ", true, UIFont.SymbolStyle.None).x * 
                                        //    m_inputText.label.font.size;

                                        for (int j = 0; j < m_iAddSpaceNum; ++j)
                                        {
                                            text = text.Insert(i + j, " ");
                                        }

                                        width += (m_iAddSpaceNum * m_inputText.label.font.CalculatePrintedSize(" ",
                                            true, UIFont.SymbolStyle.None).x * m_inputText.label.font.size);

                                        m_fOffect += (LINEWIDTH * (row + 1) - (width + m_fOffect));

                                        width += m_fOffect;
                                    }
                                    else
                                    {
                                        //width = width + m_fOffect - m_iAddSpaceNum * (m_inputText.label.font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).x *
                                        //  m_inputText.label.font.size);
                                        width += m_fOffect;
                                    }
                                    //else
                                    //{
                                    //   width += m_fOffect;
                                    //}

                                    //g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                    // width % 750,
                                    //     obj.transform.localPosition.y - (int)(width / 750) *
                                    // 35, obj.transform.localPosition.z);

                                    //g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                    //m_inputText.label.font.size * length % 750,
                                    //    obj.transform.localPosition.y - (int)(m_inputText.label.font.size * length / 750) *
                                    //m_inputText.label.font.size, obj.transform.localPosition.z);

                                    //g.transform.GetComponentsInChildren<ChatUIEquipmentGrid>(true)[0].LogicText = originString;


                                    //g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                    //     width % LINEWIDTH,
                                    //    obj.transform.localPosition.y - (int)(width / LINEWIDTH) * m_inputText.label.font.size, obj.transform.localPosition.z);//这里改成0 - ...

                                    g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                        width % LINEWIDTH,
                                       0 - (int)(width / LINEWIDTH) * m_inputText.label.font.size, obj.transform.localPosition.z);

                                    g.transform.GetComponentsInChildren<ChatUIEquipmentGrid>(true)[0].LogicText = originString;

                                });
                            break;
                        }

                        ++tmp;
                    }
                }
                else if (text[i] == '<' && text.Substring(i).Length >= 6 && text.Substring(i, 6) == "<face=")
                {
                    int tmp = i + 7;

                    while (tmp < text.Length)
                    {
                        if (text[tmp] == '>')
                        {
                            float length = m_inputText.label.font.CalculatePrintedSize(text.Substring(0, i), true, UIFont.SymbolStyle.None).x;

                            string originString = text.Substring(i, tmp - i + 1);
                            text = text.ReplaceFirst(originString, /*TranslateInputText(originString)*/"　");
                            AddChatUIFaceLabel(TranslateInputText(originString),
                                obj.transform.Find("ChatUIDialogLabelSLList"), (g) =>
                                {
                                    float width = m_inputText.label.font.size * length;
                                    int row = (int)((width + m_fOffect) / LINEWIDTH);

                                    if ((width + m_fOffect) + FONTSIZE > LINEWIDTH * (row + 1))
                                    {
                                        m_fOffect += (LINEWIDTH * (row + 1) - (width + m_fOffect));
                                        width = width + m_fOffect;
                                    }
                                    else
                                    {
                                        width += m_fOffect;
                                    }

                                    g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                     width % LINEWIDTH,
                                         obj.transform.localPosition.y - (int)(width / LINEWIDTH) *
                                     FONTSIZE, obj.transform.localPosition.z);

                                });
                            break;
                        }

                        ++tmp;
                    }
                }
                else
                {
                    float length = m_inputText.label.font.CalculatePrintedSize(text.Substring(0, i), true, UIFont.SymbolStyle.None).x;

                    float width = m_inputText.label.font.size * length;
                    int row = (int)((width + m_fOffect) / LINEWIDTH);
                    float labelWidth = m_inputText.label.font.CalculatePrintedSize(text[i].ToString(),
                           true, UIFont.SymbolStyle.None).x * m_inputText.label.font.size;

                    if ((width + m_fOffect) + labelWidth > LINEWIDTH * (row + 1))
                    {
                        // m_iAddSpaceNum = (int)((LINEWIDTH * (row + 1) - (width + m_fOffect)) /
                        //(m_inputText.label.font.CalculatePrintedSize(" ",
                        //         true, UIFont.SymbolStyle.None).x * m_inputText.label.font.size));
                        m_fOffect += (LINEWIDTH * (row + 1) - (width + m_fOffect));
                        //width += m_fOffect;
                    }
                    //else
                    //{
                    //    width += m_fOffect;
                    //}*/
                }
            }

            lblChatUIDialogLabelText.text = text;
            lblChatUIDialogLabelText.supportEncoding = true;

            obj.transform.localPosition = new Vector3(0, 0 - FONTSIZE * m_iLineNum - FONTSPACE * m_iSpaceNum, 0);


            obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
                new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.x,
                    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.y *
                    ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * FONTSIZE / LINEWIDTH) + 1),
                    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.z);

            obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center =
                new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center.x,
                    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center.y -
                    ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * FONTSIZE / LINEWIDTH)) * 0.5f * FONTSIZE,
                    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center.z);

            obj.GetComponentsInChildren<CommunityUIDialogLabel>(true)[0].ID = userId;

            m_iLineNum += ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * FONTSIZE / LINEWIDTH) + 1);


            relatedCam.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -192 - (FONTSIZE * m_iLineNum + m_iSpaceNum * FONTSPACE) + 406;

            CommunityLabelInfo info = new CommunityLabelInfo();
            info.height = FONTSIZE * ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * FONTSIZE / LINEWIDTH) + 1);
            info.go = obj;

            m_queueChatData.Enqueue(info);

            if (m_queueChatData.Count > 50)
            {
                CommunityLabelInfo infoTmp = m_queueChatData.Dequeue();
                AssetCacheMgr.ReleaseInstance(infoTmp.go);
                //Debug.LogError(infoTmp.height);
                relatedCam.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXY -= (infoTmp.height + FONTSPACE);
            }

            m_iSpaceNum++;

            //if (text.Length > 7 && (text[0] == '[' && text[7] == ']'))
            //{
            //    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
            //   new Vector3((text.Length - 8 - 2) * 22 + 14, 22, 0);

            //    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center =
            //      new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.x * 0.5f, 0f, 0f);

            //    m_lblInputText.text += text.Substring(8);


            //}
            //else
            //{
            //    obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
            //  new Vector3((text.Length - 2) * 22 + 14, 22, 0);

            //    m_lblInputText.text += text;
            //}

            switch (channel)
            {
                case ChannelId.WORLD:
                case ChannelId.SYSTEM:
                    m_iLineNumWorld = m_iLineNum;
                    m_iSpaceNumWorld = m_iSpaceNum;
                    break;

                case ChannelId.UNION:
                    m_iLineNumTong = m_iLineNum;
                    m_iSpaceNumTong = m_iSpaceNum;
                    break;

                case ChannelId.PERSONAL:
                    m_iLineNumPrivate = m_iLineNum;
                    m_iSpaceNumPrivate = m_iSpaceNum;
                    break;
            }

            if ((m_iLineNum * FONTSIZE + m_iSpaceNum * FONTSPACE) > 428)
            {
                float offect = ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * FONTSIZE / LINEWIDTH) + 1) * FONTSIZE + FONTSPACE;
                relatedCam.transform.localPosition = new Vector3(relatedCam.transform.localPosition.x,
                    relatedCam.transform.localPosition.y - offect, relatedCam.transform.localPosition.z);
            }

            m_listChatUIDialogText.Add(obj);
        });


        m_fOffect = 0;
        m_iAddSpaceNum = 0;
    }

    /// <summary>
    /// 设置频道图标
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="spChannel"></param>
    void SetChatUIDialogLabelChannel(ChannelId channel, UISprite spChannel)
    {
        if (spChannel == null)
            return;

        switch (channel)
        {
            case ChannelId.WORLD:
                spChannel.spriteName = "lt_sj";
                break;

            case ChannelId.UNION:
                spChannel.spriteName = "lt_gh";
                break;

            case ChannelId.PERSONAL:
                spChannel.spriteName = "lt_sl";
                break;

            case ChannelId.SYSTEM:
                spChannel.spriteName = "lt_xt";
                break;
        }
    }

    /// <summary>
    /// 添加聊天表情
    /// </summary>
    public void AddFaceIconToList()
    {
        float fFaceSpaceWidth = (m_transFaceEndPos.localPosition.x - m_transFaceStartPos.localPosition.x) / 5f;
        float fFaceSpaceHeight = (m_transFaceStartPos.localPosition.y - m_transFaceEndPos.localPosition.y) / 2f;

        string spName;

        for (int i = 0; i < 3; ++i)
        {
            int indexI = i;
            for (int j = 0; j < 6; ++j)
            {
                int indexJ = j;

                AssetCacheMgr.GetUIInstance("FaceIcon.prefab", (prefab, guid, go) =>
                {
                    GameObject obj = (GameObject)go;
                    obj.transform.parent = m_goFaceList.transform;
                    obj.transform.localScale = new Vector3(1, 1, 1);
                    obj.transform.localPosition = m_transFaceStartPos.localPosition + new Vector3(indexJ * fFaceSpaceWidth, indexI * (-fFaceSpaceHeight), 0);
                    obj.name = "FaceIcon";
                    obj.transform.GetComponentsInChildren<CommunityUIButton>(true)[0].ID = indexI * 6 + indexJ;

                    spName = UIFaceIconData.dataMap[indexI * 6 + indexJ + 1].facefirst;
                    obj.GetComponentsInChildren<UISprite>(true)[0].spriteName = spName;
                });
            }
        }
    }

    public void ShowFriendButton(bool isShow)
    {
        m_goFriendButton.SetActive(isShow);
    }

    public void ShowPrivateInput(bool isShow)
    {
        m_goPrivateInput.SetActive(isShow);
    }

    public void ShowWorldInput(bool isShow)
    {
        m_goWorldInput.SetActive(isShow);
    }

    public string GetChatUILogicText()
    {

        // return SplitLogicText(m_strLogicText);
        return m_inputText.LogicText;

    }

    public void AddFriendListGrid(string name, ulong id)
    {

        AssetCacheMgr.GetUIInstance("CommunityUIFriendListGrid.prefab", (prefab, guid, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.transform.parent = m_goFriendGridList.transform;
            obj.transform.localPosition = new Vector3(0, m_vec3FriendGridListStartPos.y - 0.042f * m_listFriendGird.Count, 0);
            obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camFriendGirdListCamera;
            obj.name = id.ToString();
            obj.GetComponentsInChildren<CommunityUIFriendGird>(true)[0].ID = id;
            obj.GetComponentsInChildren<UILabel>(true)[0].text = name;
            var s = m_goFriendGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0] as MogoSingleButtonList;
            s.SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);

            m_listFriendGird.Add(obj);

            if (m_listFriendGird.Count >= 4)
            {
                m_camFriendGirdListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -0.148f - 0.042f * (m_listFriendGird.Count - 4);
            }
            else
            {
                m_camFriendGirdListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -0.148f;
            }
        });

       
    }

    public void ClearFriendList()
    {
        for (int i = 0; i < m_listFriendGird.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listFriendGird[i]);
        }

        m_camFriendGirdListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transform.localPosition = new Vector3(0, -0.148f, 0);
        m_listFriendGird.Clear();
        m_goFriendGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Clear();
    }

    /// <summary>
    /// 显示添加好友和私聊小界面
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowAddFriendTip(bool isShow)
    {        
        m_goAddFriendTip.SetActive(isShow);
    }

    /// <summary>
    /// 设置点击的玩家昵称
    /// </summary>
    /// <param name="name"></param>
    public void SetAddFriendTipName(string name)
    {
        if (!string.IsNullOrEmpty(name) && name.Equals(MogoWorld.thePlayer.name))
        {
            m_lblAddFriendTipName.effectStyle = UILabel.Effect.None;
            m_lblAddFriendTipName.color = new Color32(51, 190, 255, 255);
        }
        else
        {
            m_lblAddFriendTipName.effectStyle = UILabel.Effect.Outline;
            m_lblAddFriendTipName.effectColor = new Color32(0, 39, 12, 255);
            m_lblAddFriendTipName.color = new Color32(19, 255, 213, 255);
        }

        m_lblAddFriendTipName.text = name;
    }

    /// <summary>
    /// 显示或隐藏好友列表
    /// </summary>
    public void ShowFriendList()
    {
        m_goFriendList.SetActive(!m_bFriendListShown);
        m_bFriendListShown = !m_bFriendListShown;
    }

    /// <summary>
    /// 隐藏好友列表
    /// </summary>
    void HideFriendList()
    {
        if (m_bFriendListShown)
            ShowFriendList();
    }

    public void SetEquipDetailInfoNeedLevel(int level)
    {
        m_equipDetailNeedLevel.text = level.ToString();
    }

    public void SetEquipDetailInfoGrowLevel(string level)
    {
        m_equipDetailGrowLevel.text = level;
    }

    public void SetEquipDetailInfoNeedJob(string job)
    {
        m_equipDetailNeedJob.text = job;
    }

    public void SetEquipDetailInfoExtra(string text)
    {
        m_equipDetailExtra.text = text;
    }

    public void SetEquipDetailInfoImage(string imgName)
    {
        m_equipDetailImageFG.spriteName = imgName;
    }

    public void SetDiamondHoleInfo(string text, int holeIndex)
    {
        m_equipDetailDiamondHoleInfo[holeIndex].text = text;
    }

    public void ShowNewDiamondHoleIcon(int id, bool isShow)
    {
        m_arrNewDiamondHoleIcon[id - 9].SetActive(isShow);
    }

    public void ShowEquipmentInfo(bool isShow)
    {
        //m_goEquipmentDetailInfo.SetActive(isShow);
        //InventoryManager.Instance.ShowEquipTip(,null);
    }

    void OnPrivateChannelIconUp(int i)
    {
        if (PRIVATECHANNELICONUP != null)
            PRIVATECHANNELICONUP();
    }

    void OnTongChannelIconUp(int i)
    {
        if (TONGCHANNELICONUP != null)
            TONGCHANNELICONUP();
    }

    void OnWorldChannelIconUp(int i)
    {
        if (WORLDCHANNELICONUP != null)
            WORLDCHANNELICONUP();
    }

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);        

        Initialize();
        PreLoadResource();

        m_myTransform.Find("CommunityUIBottom").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_goFaceList = m_myTransform.Find(m_widgetToFullName["CommunityUIFaceList"]).gameObject;
        m_goFriendList = m_myTransform.Find(m_widgetToFullName["CommunityUIFriendList"]).gameObject;

        //m_inputText = m_myTransform.FindChild(m_widgetToFullName["CommunityUIPrivateInput"]).GetComponentsInChildren<MogoInput>(true)[0];
        m_inputText = m_myTransform.Find(m_widgetToFullName["CommunityUIPrivateInput"]).gameObject.AddComponent<MogoInput>();
        //m_inputText.text = "";

        m_inputText.label = m_myTransform.Find(m_widgetToFullName["CommunityUIPrivateInputText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_transFaceStartPos = m_myTransform.Find(m_widgetToFullName["CommunityUIFaceTopLeft"]);
        m_transFaceEndPos = m_myTransform.Find(m_widgetToFullName["CommunityUIFaceBottomRight"]);

        m_goFriendButton = m_myTransform.Find(m_widgetToFullName["CommunityUIFriendButton"]).gameObject;
        m_goPrivateInput = m_myTransform.Find(m_widgetToFullName["CommunityUIPrivateInput"]).gameObject;

        m_goAddFriendTip = m_myTransform.Find(m_widgetToFullName["CommunityUIAddFriendTip"]).gameObject;
        m_lblAddFriendTipName = m_myTransform.Find(m_widgetToFullName["CommunityUIAddFriendTipName"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goFriendGridList = m_myTransform.Find(m_widgetToFullName["CommunityUIFriendGridList"]).gameObject;
        m_camFriendGirdListCamera = m_myTransform.Find(m_widgetToFullName["CommunityUIFriendGridCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camWorldChannelCamera = m_myTransform.Find(m_widgetToFullName["WorldCannelDialogCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camTongChannelCamera = m_myTransform.Find(m_widgetToFullName["TongCannelDialogCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camPrivateChannelCamera = m_myTransform.Find(m_widgetToFullName["PrivateCannelDialogCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camInputTextCamera = m_myTransform.Find(m_widgetToFullName["CommunityUIPrivateInputListCamera"]).GetComponentsInChildren<Camera>(true)[0];

        m_camFriendGirdListCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_camWorldChannelCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_camTongChannelCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_camPrivateChannelCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_camInputTextCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_equipDetailNeedLevel = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailNeedLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailGrowLevel = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailGrowLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedJob = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailNeedJobType"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailExtra = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailExtraText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailImageFG = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];

        m_goEquipmentDetailInfo = m_myTransform.Find(m_widgetToFullName["PacakgeEquipNewInfo"]).gameObject;

        for (int i = 0; i < 4; ++i)
        {
            m_arrNewDiamondHoleIcon[i] = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDiamonHole" + (i + 9) + "FG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].gameObject;
        }

        AddFaceIconToList();
    }

    /// <summary>
    /// 隐藏聊天表情面板
    /// </summary>
    void HideFaceList()
    {
        if (m_bFaceListShown)
        {
            OnFaceButtonUp(0);
        }
    }

    void OnFaceButtonUp(int i)
    {
        m_goFaceList.SetActive(!m_bFaceListShown);
        m_bFaceListShown = !m_bFaceListShown;
    }

    void OnFriendButtonUp(int i)
    {
        if (FRIENDBTNUP != null)
            FRIENDBTNUP();

        ShowFriendList();
    }

    void OnCommunityUICloseButtonUp(int i)
    {
        if (CommunityUIParent == CommunityUIParent.MainUI)
        {
            NormalMainUIViewManager.Instance.SetCommunityIconBG(false);
            gameObject.SetActive(false);
        }
        else
        {
            NormalMainUIViewManager.Instance.SetCommunityIconBG(false);
            NormalMainUIViewManager.Instance.ShowCommunityButton(true);
            gameObject.SetActive(false);
        }
    }

    void OnCommunityUISendButtonUp(int i)
    {
        if (SENDBTNUP != null)
            SENDBTNUP();
    }

    void OnCommunityUIFaceIconUp(int id)
    {
        string text = string.Concat("<face=(", id, ")>");

        AddChatUIInputText(text);
    }

    void OnCommunityUIAddFriendBtnUp(int i)
    {
        if (ADDFRIENDBTNUP != null)
        {
            ADDFRIENDBTNUP();
        }
    }

    void OnCommunityUIPrivateTalkBtnUp(int i)
    {
        if (PRIVATETALKBTNUP != null)
            PRIVATETALKBTNUP();
    }

    void OnEquipmentInfoCloseUp(int i)
    {
        ShowEquipmentInfo(false);
    }

    /// <summary>
    /// 频道背景(确定投影范围)点击
    /// </summary>
    /// <param name="i"></param>
    void OnCommunityUIChannelBGUp(int i)
    {
        OnCommunityUIMaskBGUp(i);
    }

    /// <summary>
    /// 聊天界面Mask点击
    /// </summary>
    /// <param name="i"></param>
    void OnCommunityUIMaskBGUp(int i)
    {
        m_goAddFriendTip.SetActive(false);
        HideFaceList();
        HideFriendList();
    }

    void Initialize()
    {
        CommunityUILogicManager.Instance.Initialize();

        CommunityUIDict.ButtonTypeToEventUp.Add("PrivateChannelIcon", OnPrivateChannelIconUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("TongChannelIcon", OnTongChannelIconUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("WorldChannelIcon", OnWorldChannelIconUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("CommunityUIFaceButton", OnFaceButtonUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("CommunityUIFriendButton", OnFriendButtonUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("CommunityUICloseButton", OnCommunityUICloseButtonUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("CommunityUISendButton", OnCommunityUISendButtonUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("CommunityUIAddFriendBtn", OnCommunityUIAddFriendBtnUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("CommunityUIPrivateTalkBtn", OnCommunityUIPrivateTalkBtnUp);

        CommunityUIDict.ButtonTypeToEventUp.Add("CommunityUIMaskBG", OnCommunityUIMaskBGUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("WorldChannelDialogBG", OnCommunityUIChannelBGUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("TongChannelDialogBG", OnCommunityUIChannelBGUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("PrivateChannelDialogBG", OnCommunityUIChannelBGUp);

        CommunityUIDict.ButtonTypeToEventUp.Add("FaceIcon", OnCommunityUIFaceIconUp);
        CommunityUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoClose", OnEquipmentInfoCloseUp);
    }

    public void Release()
    {
        CommunityUILogicManager.Instance.Release();

        CommunityUIDict.ButtonTypeToEventUp.Clear();
    }
}
