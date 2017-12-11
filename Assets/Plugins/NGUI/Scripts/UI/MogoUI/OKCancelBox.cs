using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class OKCancelBox : MonoBehaviour
{

    UILabel m_lblBoxText;
    UILabel m_lblOKBtnText;
    UILabel m_lblCancelBtnText;

    public UISprite m_spOKBgUp;
    public UISprite m_spOKBgDown;
    public UISprite m_spCancelBgUp;
    public UISprite m_spCancelBgDown;
    public bool m_isInForwardLoading = false; 

    Transform m_myTransform;

    Action<bool> m_actCallback;

    float m_fCountDown = 0;
    float m_fCountCurrent = 0;
    bool m_bStartCount = false;

    GameObject m_goMessageBoxCamera;

    public void ShowAsOK()
    {
        //m_goMessageBoxCamera.SetActive(true);
        m_lblCancelBtnText.transform.parent.gameObject.SetActive(false);
        m_lblOKBtnText.transform.parent.localPosition = new Vector3(0, -100.0f, 0);
    }

    public void ShowAsOKCancel()
    {
        //m_goMessageBoxCamera.SetActive(true);
        m_lblCancelBtnText.transform.parent.gameObject.SetActive(true);
        m_lblOKBtnText.transform.parent.localPosition = new Vector3(150.0f, -100.0f, 0);
    }

    void Awake()
    {

        m_myTransform = transform;

        Initialize();

        m_lblBoxText = m_myTransform.Find("OKCancelText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOKBtnText = m_myTransform.Find("OKButton/OKButtonText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblCancelBtnText = m_myTransform.Find("CancelButton/CancelButtonText").GetComponentsInChildren<UILabel>(true)[0];

        m_spOKBgUp = m_myTransform.Find("OKButton/OKButtonBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spOKBgDown = m_myTransform.Find("OKButton/OKButtonBGDown").GetComponentsInChildren<UISprite>(true)[0];
        m_spCancelBgUp = m_myTransform.Find("CancelButton/CancelButtonBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spCancelBgDown = m_myTransform.Find("CancelButton/CancelButtonBGDown").GetComponentsInChildren<UISprite>(true)[0];

        //Debug.LogError("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ awake");
        //if (!m_isInForwardLoading)
        //{
        //    m_goMessageBoxCamera = m_myTransform.parent.parent.parent.parent.FindChild("MessageBoxCamera").gameObject;
        //    m_goMessageBoxCamera.SetActive(false);
        //}


        if (!m_isInForwardLoading)
        {

            gameObject.SetActive(false);
        }
     
    }

    void OnDestroy()
    {
        m_lblBoxText = null;
        m_lblOKBtnText = null;
        m_lblCancelBtnText = null;
        m_spOKBgUp = null;
        m_spOKBgDown = null;
        m_spCancelBgUp = null;
        m_spCancelBgDown = null;
        m_myTransform = null;
        m_goMessageBoxCamera = null;
    }

    void OnOKButtonUp()
    {
        if (m_actCallback != null)
        {
            m_actCallback(true);
        }
    }

    void OnCancelButtonUp()
    {
        if (m_actCallback != null)
        {
            m_actCallback(false);
        }
    }

    void Initialize()
    {
        EventDispatcher.AddEventListener("OKButtonUp", OnOKButtonUp);
        EventDispatcher.AddEventListener("CancelButtonUp", OnCancelButtonUp);
    }

    void Release()
    {
        EventDispatcher.RemoveEventListener("OKButtonUp", OnOKButtonUp);
        EventDispatcher.RemoveEventListener("CancelButtonUp", OnCancelButtonUp);
    }

    public void SetBoxText(string text)
    {
        m_lblBoxText.text = text;
    }

    public void SetOKBtnText(string text)
    {
        m_lblOKBtnText.text = text;
    }

    public void SetCancelBtnText(string text)
    {
        m_lblCancelBtnText.text = text;
    }

    public void SetCallback(Action<bool> callback)
    {
        m_actCallback = callback;
    }

    public void SetCountDown(float fTime)
    {
        if (fTime >= 0)
        {
            m_fCountDown = fTime;
            m_bStartCount = true;
        }
    }

    void Update()
    {
        if (m_bStartCount)
        {
            m_fCountCurrent += Time.deltaTime;

            if (m_fCountCurrent >= m_fCountDown)
            {
                OnOKButtonUp();
                m_bStartCount = false;
                m_fCountCurrent = 0;
                m_fCountDown = 0;
            }
        }
    }

    void OnDisable()
    {
        if (!m_isInForwardLoading)
        {

            EventDispatcher.TriggerEvent("OKCancelBoxClose");
            //m_goMessageBoxCamera.SetActive(false);
        }
    }
}
