/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������MogoNotice
// �����ߣ�Joe Mo
// �޸����б��
// �������ڣ�2013-7-29
// ģ��������MogoNotice
//----------------------------------------------------------------*/


using UnityEngine;
using System.Collections.Generic;
using System;
using Mogo.Util;
using System.Collections;

public class MogoNotice : MonoBehaviour
{
    public static MogoNotice Instance;
    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public class Notice
    {
        public string title { get; set; }
        public string text { get; set; }
        public bool isNew { get; set; }
        public string date { get; set; }
    }

    //public static MogoNotice Instance
    //{
    //    get
    //    {
    //        if (m_instance == null)
    //        {
    //            GameObject obj = GameObject.Find("MogoNotice");

    //            if (obj)
    //            {
    //                m_instance = obj.transform.GetComponentsInChildren<MogoNotice>(true)[0];
    //            }
    //        }

    //        return MogoNotice.m_instance;

    //    }
    //}


    GameObject m_mogoNoticeTitles;
    GameObject m_mogoNoticeContents;

    SimpleDragCamera m_contentsDragCamera;
    SimpleDragCamera m_titleDragCamera;

    UILabel m_contentLabel;
    List<GameObject> m_objList = new List<GameObject>();
    List<int> m_contentPositionList = new List<int>();
    const string TITLE_PREFAB = "MogoNoticeTitle.prefab";
    const string CONTENT_PREFAB = "MogoNoticeContent.prefab";
    //MogoNoticeTitle
    OKMsgBox m_goOKCancelBox;
    FloatMsg m_floatMsg;
    int contentLineWidth;
    int contentFontSize;
    public bool isShow = false;

    int m_contentHeight;
    int m_titleHeight;

    Transform GetTransformByName(string str)
    {
        return transform.Find(m_widgetToFullName[str]);
    }

    void Awake()
    {
        FillFullNameData(transform);

        m_mogoNoticeTitles = transform.Find(m_widgetToFullName["MogoNoticeTitles"]).gameObject;
        m_mogoNoticeContents = transform.Find(m_widgetToFullName["MogoNoticeContents"]).gameObject;

        m_contentsDragCamera = transform.Find(m_widgetToFullName["MogoNoticeContentsBG"]).gameObject.AddComponent<SimpleDragCamera>();
        m_titleDragCamera = transform.Find(m_widgetToFullName["MogoNoticeTitlesBG"]).gameObject.AddComponent<SimpleDragCamera>();

        m_contentsDragCamera.RelatedCamera = GetTransformByName("MogoNoticeContentsCamera").GetComponent<Camera>();
        m_titleDragCamera.RelatedCamera = GetTransformByName("MogoNoticeTitlesCamera").GetComponent<Camera>();

        m_contentsDragCamera.beginTransform = GetTransformByName("MogoNoticeContentsBegin");
        m_titleDragCamera.beginTransform = GetTransformByName("MogoNoticeTitlesBegin");

        m_contentHeight = (int)GetTransformByName("MogoNoticeContentsBG").localScale.y;
        m_titleHeight = (int)GetTransformByName("MogoNoticeTitlesBG").localScale.y;
        GetTransformByName("MogoNoticeMaskBG").gameObject.AddComponent<MogoBgClose>();

        Camera camera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        GetComponent<UIStretch>().uiCamera = camera;
        GameObject.Find("MogoNoticeContentsCamera").GetComponent<UIViewport>().sourceCamera = camera;
        GameObject.Find("MogoNoticeTitlesCamera").GetComponent<UIViewport>().sourceCamera = camera;

        GameObject.Find("MogoNoticeTitlesCamera").GetComponent<UIViewport>().sourceCamera = camera;


        Instance = GetComponent<MogoNotice>();

        AssetCacheMgr.GetUIResource(CONTENT_PREFAB, (obj) =>
        {
            GameObject go = obj as GameObject;
            UILabel lable = go.transform.Find("MogoNoticeContentText").GetComponent<UILabel>();
            m_contentLabel = lable;
            contentLineWidth = lable.lineWidth;
            contentFontSize = lable.font.size;
        }
        );
        Reset();
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
        m_titleDragCamera.Reset();
        m_contentsDragCamera.Reset();
        m_contentPositionList.Clear();
    }

    public IEnumerator ShowNotice(List<Notice> list)
    {
        while (isShow || m_contentLabel == null)
        {
            Mogo.Util.LoggerHelper.Debug("m_contentLabel is Loading!");
            yield return null;
        }
        isShow = true;
        Reset();

        int titleGap = 30 + 10;//����߶�+gap

        int count1 = 0;
        int count2 = 0;
        int gap = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var index = i;

            //����titleList,�ѹҵ�MogoNoticeTitles
            AssetCacheMgr.GetUIInstance(TITLE_PREFAB, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                go.AddComponent<NoticeTitle>().index = count1;
                UILabel label = go.transform.Find("MogoNoticeTitleText").GetComponent<UILabel>();
                label.text = list[count1].title;
                //Mogo.Util.LoggerHelper.Debug("list[index].title:" + list[count1].title);
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_mogoNoticeTitles.transform);

                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - count1 * titleGap, go.transform.localPosition.z);

                m_objList.Add(go);
                count1++;
            });

            var tempGap = gap;
            m_contentPositionList.Add(tempGap);
            //����contentList,�ҵ�MogoNoticeContents
            AssetCacheMgr.GetUIInstance(CONTENT_PREFAB, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                UILabel labelText = go.transform.Find("MogoNoticeContentText").GetComponent<UILabel>();
                UILabel labelTitle = go.transform.Find("MogoNoticeContentTitle").GetComponent<UILabel>();
                labelTitle.text = list[count2].title;
                labelText.text = list[count2].text;

                count2++;
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_mogoNoticeContents.transform);

                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y + tempGap, go.transform.localPosition.z);

                m_objList.Add(go);
            });


            //����߶�
            if (m_contentLabel == null) LoggerHelper.Error("m_contentLabel == null");
            if (list.Count <= index) LoggerHelper.Error("list[index] == null");
            Vector2 temp = m_contentLabel.font.CalculatePrintedSize(list[index].text, true, UIFont.SymbolStyle.None);
            float length = temp.x * contentFontSize;
            float contentGap = (length / contentLineWidth) * temp.y * contentFontSize;
            gap -= ((int)contentGap) + 25 + titleGap;

        }

        m_titleDragCamera.height = list.Count * titleGap - m_titleHeight;
        m_contentsDragCamera.height = -gap - m_contentHeight;
        gameObject.SetActive(true);
    }

    public void CloseNotice()
    {
        Reset();
        gameObject.SetActive(false);
        isShow = false;
    }

    public void OnSelectTitle(int index)
    {
        Mogo.Util.LoggerHelper.Debug("OnSelectTitle:" + index);
        m_contentsDragCamera.MoveFromBeginByHeight(m_contentPositionList[index]);
        //throw new NotImplementedException();
    }
}
