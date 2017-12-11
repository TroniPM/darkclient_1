/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������MogoNotice2
// �����ߣ�Ī׿��
// �޸����б��
// �������ڣ�2013-8-20
// ģ��������MogoNotice2
//----------------------------------------------------------------*/


using UnityEngine;
using System.Collections.Generic;
using System;
using Mogo.Util;
using System.Collections;



public class MogoNotice2 : MonoBehaviour
{
    //class WebViewCallbackTest : Kogarasi.WebView.IWebViewCallback
    //{
    //    public void onLoadStart(string url)
    //    {
    //        Debug.Log("call onLoadStart : " + url);
    //    }
    //    public void onLoadFinish(string url)
    //    {
    //        Debug.Log("call onLoadFinish : " + url);
    //    }
    //    public void onLoadFail(string url)
    //    {
    //        Debug.Log("call onLoadFail : " + url);
    //    }
    //}

    public static MogoNotice2 Instance;
    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();
    bool m_isShow = false;

    public class Notice
    {
        public string title { get; set; }
        public string text { get; set; }
        public bool isNew { get; set; }
        public string date { get; set; }
    }

    UILabel m_titleLbl;
    UILabel m_dateLbl;
    UILabel m_contentLbl;
    Transform m_listRoot;
    MogoSingleButtonList m_buttonList;
    Transform m_listBegin;
    Vector3 m_listBeginPos;
    Camera m_listCamera;
    Camera m_contentCamera;
    MyDragableCamera m_listDragableCamera;
    Transform m_contentCameraBegin;
    Transform m_listCameraBegin;
    Transform m_contentBR;
    Transform m_contentTL;
    float m_contentAreaHeight = 0;
    Camera m_sourceCamera;
    List<GameObject> m_objList = new List<GameObject>();
    List<Notice> m_currentNoticeList = new List<Notice>();

    int m_currentIndex = 0;
    //WebViewBehavior m_webview;
    Transform GetTransformByName(string str)
    {
        return transform.Find(m_widgetToFullName[str]);
    }

    void Awake()
    {
        Instance = GetComponent<MogoNotice2>();
        FillFullNameData(transform);

        m_sourceCamera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        gameObject.GetComponent<UIStretch>().uiCamera = m_sourceCamera;
        m_listCamera = GetTransformByName("NoticeListCamera").gameObject.GetComponent<Camera>();
        m_listDragableCamera = m_listCamera.GetComponent<MyDragableCamera>();
        GetTransformByName("NoticeListCamera").GetComponent<UIViewport>().sourceCamera = m_sourceCamera;

        m_titleLbl = GetTransformByName("BannerTitle").GetComponent<UILabel>();
        m_dateLbl = GetTransformByName("BannerDate").GetComponent<UILabel>();
        m_contentLbl = GetTransformByName("ContentLbl").GetComponent<UILabel>();
        m_listRoot = GetTransformByName("NoticeList");
        m_buttonList = m_listRoot.GetComponent<MogoSingleButtonList>();
        m_listBegin = GetTransformByName("NoticeListPosBegin");
        m_listBeginPos = m_listBegin.localPosition;

        m_listCameraBegin = GetTransformByName("NoticeListCameraPosBegin");
        GetTransformByName("NoticeBoxClose").gameObject.AddComponent<MogoBgClose>();


        m_contentCamera = GetTransformByName("ContentCamera").GetComponent<Camera>();
        m_contentCamera.gameObject.GetComponent<UIViewport>().sourceCamera = m_sourceCamera;

        Transform contentArea = GetTransformByName("ContentArea");
        m_contentBR = GetTransformByName("ContentAreaBR");
        m_contentTL = GetTransformByName("ContentAreaTL");
        contentArea.GetComponent<MyDragCamera>().RelatedCamera = m_contentCamera;
        m_contentAreaHeight = contentArea.localScale.y;

        m_contentCameraBegin = GetTransformByName("ContentCameraPosBegin");

        Reset();


        //m_callback = new WebViewCallbackTest();

        //m_webview = GetComponent<WebViewBehavior>();

        //if (m_webview != null)
        //{
        //    m_webview.setCallback(m_callback);
        //}

        gameObject.SetActive(false);

    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            try
            {
                m_widgetToFullName.Add(rootTransform.GetChild(i).name, Utils.GetFullName(transform, rootTransform.GetChild(i)));
            }
            catch
            {
                Mogo.Util.LoggerHelper.Debug("rootTransform.GetChild(i):" + rootTransform.GetChild(i).name);
            }

            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    public void Reset()
    {
        foreach (GameObject go in m_objList)
        {
            AssetCacheMgr.ReleaseInstance(go);
        }
        m_objList.Clear();

        m_buttonList.SingleButtonList = new List<MogoSingleButton>();
        m_currentNoticeList = new List<Notice>();

        m_listCamera.transform.localPosition = m_listCameraBegin.localPosition;
        m_contentCamera.transform.localPosition = m_contentCameraBegin.localPosition;
    }

    const int LIST_GAP = 100;
    const string LIST_GRID_PREFAB = "NoticeGrid.prefab";
    const int PAGE_NUM = 5;
    private bool m_loadingDone = false;

    public void PreloadResource()
    {
        AssetCacheMgr.GetUIResource(LIST_GRID_PREFAB, null);
    }

    //private WebViewCallbackTest m_callback;

    //���⴦���������ʱֻ����һ�Σ��´δ�listҲû����
    public void ShowNotice(List<Notice> list)
    {
        if (list == null || list.Count <= 0 || m_isShow)
        {
            return;
        }
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        if (!m_loadingDone)
        {
            //Debug.LogError("!m_loadingDone");
            m_isShow = true;
            //����֮ǰ������
            Reset();

            m_currentNoticeList = list;
            //������Դ���Ű�
            LoadNotice();


            //if (m_webview != null)
            //{
            //    m_webview.LoadURL("http://www.baidu.com/");
            //    m_webview.SetVisibility(true);
            //    Vector3 tl = m_sourceCamera.WorldToScreenPoint(m_contentTL.position);
            //    Vector3 br = m_sourceCamera.WorldToScreenPoint(m_contentBR.position);
            //    m_webview.SetMargins((int)tl.x, (int)(m_sourceCamera.GetScreenHeight() - tl.y), (int)(m_sourceCamera.GetScreenWidth() - br.x), (int)br.y);

            //}
        }
        else
        {
            //Debug.LogError("m_loadingDone");
            RefreshRightUI(m_currentIndex);
            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        }
        //��ʾgameObject
        gameObject.SetActive(true);
    }

    private void LoadNotice()
    {
        int gap = 0;

        SetupListDragCamera();

        for (int i = 0; i < m_currentNoticeList.Count; i++)
        {
            int index = i;
            int tempGap = gap;
            //������Դ
            AssetCacheMgr.GetUIInstance(LIST_GRID_PREFAB, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                go.AddComponent<MogoNoticeListButton>().id = index;
                go.GetComponent<MyDragCamera>().RelatedCamera = m_listCamera;
                UILabel label = go.transform.Find("NoticeGridText").GetComponent<UILabel>();
                GameObject goIsNew = go.transform.Find("NoticeGridIsNew").gameObject;

                //�ҵ�����ȥ
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_listRoot);

                //����ֵ
                if (m_currentNoticeList.Count > index)
                {
                    label.text = m_currentNoticeList.Get(index).date;
                    goIsNew.SetActive(m_currentNoticeList.Get(index).isNew);
                }

                //�Ű�
                go.transform.localPosition = new Vector3(m_listBeginPos.x, m_listBeginPos.y + tempGap, m_listBeginPos.z);

                //�����б�
                m_objList.Add(go);
                m_buttonList.SingleButtonList.Add(go.GetComponent<MogoSingleButton>());

                if (index == m_currentNoticeList.Count - 1)
                {
                    //Ĭ��ѡ�е�һ��
                    m_currentIndex = 0;

                    //ˢ�½���
                    RefreshRightUI(m_currentIndex);

                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    m_loadingDone = true;
                }
            });
            gap -= LIST_GAP;
        }


    }

    private void SetupListDragCamera()
    {
        //��transform����
        int tSize = m_currentNoticeList.Count / PAGE_NUM + 1;
        if (m_currentNoticeList.Count % PAGE_NUM == 0) tSize--;

        //�������
        if (m_listDragableCamera.transformList != null) m_listDragableCamera.transformList.Clear();
        else m_listDragableCamera.transformList = new List<Transform>();

        Vector3 position = m_listCameraBegin.localPosition;
        for (int i = 0; i < tSize; i++)
        {
            GameObject go = new GameObject();
            //�ҵ�����ȥ
            Utils.MountToSomeObjWithoutPosChange(go.transform, m_listRoot);
            go.transform.localPosition = new Vector3(position.x, position.y - i * LIST_GAP * PAGE_NUM, position.z);
            m_listDragableCamera.transformList.Add(go.transform);
        }
        if (tSize == 1) m_listDragableCamera.transformList.Add(m_listCameraBegin);
    }

    public void RefreshRightUI(int index)
    {
        m_currentIndex = index;
        m_contentCamera.transform.localPosition = m_contentCameraBegin.localPosition;
        m_buttonList.SetCurrentDownButton(index);
        m_titleLbl.text = m_currentNoticeList[index].title;
        m_dateLbl.text = m_currentNoticeList[index].date;
        m_contentLbl.text = m_currentNoticeList[index].text;
        SetupContentDragCamera();
    }

    private void SetupContentDragCamera()
    {
        string text = m_currentNoticeList[m_currentIndex].text;
        //int contentLineWidth = m_contentLbl.lineWidth;
        //int contentFontSize = m_contentLbl.font.size;//

        //����߶�
        //Vector2 temp = m_contentLbl.font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None);
        //int length = (int)(temp.x * contentFontSize);
        //int line = ((length / contentLineWidth) + 1);
        //Mogo.Util.LoggerHelper.Debug("line:" + line);
        //float contentGap = line * temp.y * contentFontSize;
        //Mogo.Util.LoggerHelper.Debug("temp.y:" + temp.y);

        float contentGap = m_contentLbl.relativeSize.y * 30;
        Mogo.Util.LoggerHelper.Debug("contentGap:" + contentGap);
        contentGap = contentGap - m_contentAreaHeight;
        contentGap = contentGap < 0 ? 0 : contentGap;
        m_contentCamera.GetComponent<MyDragableCamera>().MINY = m_contentCameraBegin.transform.localPosition.y - contentGap;


    }

    public void CloseNotice()
    {
        //Reset();

        gameObject.SetActive(false);
        m_isShow = false;
        //if (m_webview != null)
        //{
        //    m_webview.SetVisibility(false);
        //}
        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
    }

}
