using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class ChatUIViewManager : MonoBehaviour
{
    private static ChatUIViewManager m_instance;

    public static ChatUIViewManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = GameObject.Find("MogoMainUIPanel");

                if (obj)
                {
                    m_instance = obj.transform.Find("ChatUI").GetComponentsInChildren<ChatUIViewManager>(true)[0];
                }
            }

            return ChatUIViewManager.m_instance;
        }
    }

    public Action CHATUISENDUP;
    public Action CHATUISHOWUP;
    public Action CHATUICLOSEUP;
    public Action CHATUISHOW2UP;
    public Action CHATUISHOWFACEUP;

    private Transform m_myTransform;

    private int m_iOffsetNum = 0;
    private int m_iLineNum = 0;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    public static Dictionary<string, Action> ButtonTypeToEventUp = new Dictionary<string, Action>();

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    List<GameObject> m_listChatUIDialogText = new List<GameObject>();

    MogoInput m_inputText;
    string m_strLogicText;

    float m_fOffect;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    //string SplitViewText(string text)
    //{
    //    string tmp = text;

    //    int length = text.Length;

    //    for (int i = 0; i < length; ++i)
    //    {
    //        if (text[i] == '<' && i + 5 < length && text.Substring(i,6) == "<info=")
    //        {
    //            int index = i + 6;

    //            while (index < length)
    //            {
    //                if (text[index] == '>')// && index + 2 < length && text.Substring(index, 3) == "[-]")
    //                {
    //                    tmp = text.Replace(text.Substring(i, index - i + 1), "");
    //                    break;
    //                }

    //                ++index;
    //            }
    //        }
    //    }

    //    return tmp;
    //}

    //string SplitLogicText(string text)
    //{
    //    string tmp = text;

    //    int length = text.Length;

    //    for (int i = 0; i < length; ++i)
    //    {
    //        if (text[i] == '[' && i + 7 < length && text[i + 7] == ']')
    //        {
    //            int index = i + 8;

    //            while (index < length)
    //            {
    //                if (text[index] == '[' && index + 2 < length && text.Substring(index, 3) == "[-]")
    //                {
    //                    tmp = text.Replace(text.Substring(i, index + 3 - i), "");
    //                    break;
    //                }

    //                ++index;
    //            }
    //        }
    //    }

    //    return tmp;
    //}

    string TranslateInputText(string text)
    {
        string result;

        switch (text)
        {
            case "<info=(1,1,1)>":
                result = "[FF00FF][����ս��][-]";
                break;

            case "<info=(1,0,0)>":
                result = "[FF0000][������][-]";
                break;

            case "<face=(1)>":
                result = "{:1}";
                break;

            default:
                result = "";
                break;

        }

        return result;
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
        m_iOffsetNum = 0;
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
        AssetCacheMgr.GetUIInstance("ChatUIFace.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.SetActive(true);
            obj.transform.parent = parent;
            obj.transform.GetComponentsInChildren<UISprite>(true)[0].pivot = UIWidget.Pivot.Left;

            obj.transform.GetComponentsInChildren<UISprite>(true)[0].transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);

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


    public void AddChatUIDialogText(string text)
    {
        AssetCacheMgr.GetUIInstance("ChatUIDialogLabel.prefab", (prefab, id, go) =>
               {
                   GameObject obj = (GameObject)go;
                   obj.SetActive(true);
                   obj.transform.parent = m_myTransform.Find(m_widgetToFullName["ChatUIDialogLabelList"]);
                   obj.transform.localPosition = Vector3.zero;
                   obj.transform.localScale = new Vector3(1, 1, 1);

                   string strTmp = text;

                   for (int i = 0; i < text.Length; ++i)
                   {
                       if (text[i] == '<' && text.Substring(i, 6) == "<info=")
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
                                       int row = (int)((width + m_fOffect) / 750);

                                       float labelWidth = m_inputText.label.font.CalculatePrintedSize(TranslateInputText(originString),
                                           true, UIFont.SymbolStyle.None).x * m_inputText.label.font.size;

                                       if ((width + m_fOffect) + labelWidth > 750 * (row + 1))
                                       {
                                           m_fOffect += (750 * (row + 1) - (width + m_fOffect));
                                           //m_fOffect += m_inputText.label.font.CalculatePrintedSize("   ", true, UIFont.SymbolStyle.None).x * 
                                           //    m_inputText.label.font.size;
                                           text = text.Insert(i, "   ");
                                           m_iOffsetNum++;
                                           width += m_fOffect;
                                       }
                                       else if (width > 750 && m_iOffsetNum > 0)
                                       {
                                           width = width + m_fOffect - (m_inputText.label.font.CalculatePrintedSize("   ", true, UIFont.SymbolStyle.None).x *
                                               m_inputText.label.font.size);

                                       }
                                       else
                                       {
                                           width += m_fOffect;
                                       }

                                       //g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                       // width % 750,
                                       //     obj.transform.localPosition.y - (int)(width / 750) *
                                       // 35, obj.transform.localPosition.z);

                                       //g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                       //m_inputText.label.font.size * length % 750,
                                       //    obj.transform.localPosition.y - (int)(m_inputText.label.font.size * length / 750) *
                                       //m_inputText.label.font.size, obj.transform.localPosition.z);

                                       //g.transform.GetComponentsInChildren<ChatUIEquipmentGrid>(true)[0].LogicText = originString;

                                       g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                            width % 750,
                                           obj.transform.localPosition.y - (int)(width / 750) * m_inputText.label.font.size, obj.transform.localPosition.z);

                                       g.transform.GetComponentsInChildren<ChatUIEquipmentGrid>(true)[0].LogicText = originString;
                                   });
                                   break;
                               }

                               ++tmp;
                           }
                       }
                       else if (text[i] == '<' && text.Substring(i, 6) == "<face=")
                       {
                           int tmp = i + 7;

                           while (tmp < text.Length)
                           {
                               if (text[tmp] == '>')
                               {
                                   float length = m_inputText.label.font.CalculatePrintedSize(text.Substring(0, i), true, UIFont.SymbolStyle.None).x;

                                   string originString = text.Substring(i, tmp - i + 1);
                                   text = text.ReplaceFirst(originString, /*TranslateInputText(originString)*/"��");
                                   AddChatUIFaceLabel(TranslateInputText(originString),
                                       obj.transform.Find("ChatUIDialogLabelSLList"), (g) =>
                                   {
                                       float width = m_inputText.label.font.size * length;
                                       int row = (int)((width + m_fOffect) / 750);

                                       if ((width + m_fOffect) + 22 > 750 * (row + 1))
                                       {
                                           m_fOffect += (750 * (row + 1) - (width + m_fOffect));
                                       }


                                       if (m_iOffsetNum > 0)
                                       {
                                           //width = width + m_fOffect - (m_inputText.label.font.CalculatePrintedSize("��", true, UIFont.SymbolStyle.None).x *
                                           //    m_inputText.label.font.size);
                                           width = width + m_fOffect - 22;
                                       }
                                       else
                                       {
                                           width += m_fOffect;
                                       }

                                       g.transform.localPosition = new Vector3(obj.transform.localPosition.x +
                                        width % 750,
                                            obj.transform.localPosition.y - (int)(width / 750) *
                                        22, obj.transform.localPosition.z);

                                   });
                                   break;
                               }

                               ++tmp;
                           }
                       }
                   }

                   obj.transform.Find("ChatUIDialogLabelText").GetComponentsInChildren<UILabel>(true)[0].text = text;
                   obj.transform.Find("ChatUIDialogLabelText").GetComponentsInChildren<UILabel>(true)[0].supportEncoding = true;

                   obj.transform.localPosition = new Vector3(obj.transform.parent.Find("ChatUIDialogBeginPos").localPosition.x,
                       obj.transform.parent.Find("ChatUIDialogBeginPos").localPosition.y - 22 * m_iLineNum,
                       obj.transform.parent.Find("ChatUIDialogBeginPos").localPosition.z);

                   obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size =
                       new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.x,
                           obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.y *
                           ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * 22 / 750) + 1),
                           obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].size.z);

                   obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center =
                       new Vector3(obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center.x,
                           obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center.y -
                           ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * 22 / 750)) * 0.5f * 22,
                           obj.transform.GetComponentsInChildren<BoxCollider>(true)[0].center.z);


                   obj.GetComponentsInChildren<MyDragPanel>(true)[0].m_myDPanel = obj.transform.parent.GetComponentsInChildren<MyDragablePanel>(true)[0];
                   m_iLineNum += ((int)(m_inputText.label.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None).x * 22 / 750) + 1);

                   obj.transform.parent.GetComponentsInChildren<MyDragablePanel>(true)[0].PanelHeight = m_iLineNum * 22;


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

                   m_listChatUIDialogText.Add(obj);
               });

    }

    public string GetChatUILogicText()
    {

        // return SplitLogicText(m_strLogicText);
        return m_inputText.LogicText;

    }

    void Awake()
    {

        gameObject.SetActive(false);

        Initialize();

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_inputText = m_myTransform.Find(m_widgetToFullName["ChatUIInput"]).GetComponentsInChildren<MogoInput>(true)[0];
    }

    void OnChatUISendUp()
    {
        if (CHATUISENDUP != null)
            CHATUISENDUP();

    }

    void OnChatUIShowUp()
    {
        if (CHATUISHOWUP != null)
            CHATUISHOWUP();
    }

    void OnChatUICloseUp()
    {
        if (CHATUICLOSEUP != null)
            CHATUICLOSEUP();
    }

    void OnChatUIShow2Up()
    {
        if (CHATUISHOW2UP != null)
            CHATUISHOW2UP();
    }

    void OnChatUIShowFaceUp()
    {
        if (CHATUISHOWFACEUP != null)
            CHATUISHOWFACEUP();
    }

    void OnChatUIEquipmentGridUp(string logicText)
    {
    }

    void Initialize()
    {
        ChatUILogicManager.Instance.Initialize();

        ButtonTypeToEventUp.Add("ChatUISend", OnChatUISendUp);
        ButtonTypeToEventUp.Add("ChatUIShow", OnChatUIShowUp);
        ButtonTypeToEventUp.Add("ChatUIShow2",OnChatUIShow2Up);
        ButtonTypeToEventUp.Add("ChatUIClose",OnChatUICloseUp);
        ButtonTypeToEventUp.Add("ChatUIShowFace",OnChatUIShowFaceUp);

        EventDispatcher.AddEventListener<string>("ChatUIEquipmentGridUp", OnChatUIEquipmentGridUp);
    }

    public void Release()
    {
        ChatUILogicManager.Instance.Release();

        ButtonTypeToEventUp.Clear();
        EventDispatcher.RemoveEventListener<string>("ChatUIEquipmentGridUp", OnChatUIEquipmentGridUp);

    }
}
