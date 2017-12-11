using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MogoForwardLoadingUIManager : MonoBehaviour
{
    private static MogoForwardLoadingUIManager m_instance;

    public static MogoForwardLoadingUIManager Instance
    {
        get
        {
            return m_instance;
        }
    }

    Transform m_myTransform;

    GameObject m_defaultUI;
    GameObject m_goGlobleLoadingUI;
    GameObject m_goLoadingUICamera;

    MogoGlobleLoadingUI m_mgl;

    void Awake()
    {
        m_instance = transform.GetComponentsInChildren<MogoForwardLoadingUIManager>(true)[0];
        m_myTransform = transform;

        m_goGlobleLoadingUI = m_myTransform.Find("MogoGlobleLoadingUI").gameObject;

        bool isContinue = true;
        for (int i = 0; isContinue; i++)
        {
            Transform m_goGlobleLoadingUIButtonTemp = m_myTransform.Find(String.Concat("MogoGlobleLoadingUIButton", i));
            if (m_goGlobleLoadingUIButtonTemp)
            {
                m_goGlobleLoadingUIButtonTemp.gameObject.AddComponent<MogoGlobleLoadingUIButton>();
            }
            else
            {
                isContinue = false;
            }
        }

        m_defaultUI = GameObject.Find("MogoDefaultUI");
        m_goLoadingUICamera = m_defaultUI.transform.Find("DefaultUICamera").gameObject;
        m_mgl = m_goGlobleLoadingUI.AddComponent<MogoGlobleLoadingUI>();

        //Debug.LogError("MogoForwardLoadingUIManager awake!");

    }

    void OnDestroy()
    {
        m_myTransform = null;
        m_defaultUI = null;
        m_goGlobleLoadingUI = null;
        m_goLoadingUICamera = null;
        m_mgl = null;
    }

    public void ShowGlobleLoadingUI(bool isShow)
    {
        //m_goGlobleLoadingUI.SetActive(isShow);
        m_goLoadingUICamera.SetActive(isShow);
        if (!isShow)
        {
            //var meizi = GameObject.Find("MogoGlobleLoadingUIMeiziImg").GetComponent<UITexture>().mainTexture;
            GameObject.Destroy(m_defaultUI);
            //Resources.UnloadAsset(meizi);
            Resources.UnloadUnusedAssets();
        }
    }

    public void FillGlobalLoadingUIData(MogoForwardLoadingUIData gd)
    {
        m_mgl.LoadingTip = gd.tip;
        m_mgl.LoadingStatus = gd.status;
    }

    public void SetLoadingStatus(int progress)
    {
        m_mgl.LoadingStatus = progress;
    }

    public void SetLoadingStatusTip(string tip)
    {
        m_mgl.StatusTip = tip;
    }
}
