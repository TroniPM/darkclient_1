/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������MogoMsgBox
// �����ߣ�Joe Mo
// �޸����б��
// �������ڣ�2013-5-15
// ģ��������MsgBox
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;
using Mogo.GameData;

public class MogoMsgBox : MogoUIParent
{
    private static MogoMsgBox m_instance;

    public static MogoMsgBox Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = GameObject.Find("MsgBoxPanel");

                if (obj)
                {
                    m_instance = obj.transform.GetComponentsInChildren<MogoMsgBox>(true)[0];
                }
            }

            return MogoMsgBox.m_instance;

        }
    }

    Transform m_myTransform;

    OKMsgBox m_goOKCancelBox;
    FloatMsg m_floatMsg;

    GameObject m_goFloatMsgForPower;
    UILabel m_lblFloatMsgForPower;
    TweenScale m_tpFloatMsgForPower;
    TweenAlpha m_taFloatMsgForPower;
    TweenAlpha m_taFloatMsgForPowerBg;
    uint m_timerIdfloatMsgForPower = 0;

    GameObject m_msgBoxCamera;

    void Awake()
    {
        Init();
        m_myTransform = transform;

        //m_goOKCancelBox = m_myTransform.FindChild("MsgBox").GetComponentsInChildren<OKMsgBox>(true)[0];
        m_goOKCancelBox = m_myTransform.Find("MsgBox").gameObject.AddComponent<OKMsgBox>();

        //m_floatMsg = m_myTransform.FindChild("MsgBoxFloat").GetComponentsInChildren<FloatMsg>(true)[0];
        m_floatMsg = m_myTransform.Find("MsgBoxFloat").gameObject.AddComponent<FloatMsg>();

        m_waveMsg = m_myTransform.Find("MsgBoxWave").gameObject;
        m_waveMsgLbl = GetUIChild("MsgBoxWaveLbl").GetComponent<UILabel>();
        m_waveMsgTweenPostion = m_waveMsgLbl.GetComponent<TweenPosition>();
        m_waveMsgTweenPostion.enabled = false;
        m_waveMsg.SetActive(false);


        m_msgBoxCamera = GameObject.Find("MsgCameraForPower");
        //m_msgBoxCamera.SetActive(false);
        InitFloatMsgForPower();

    }

    private void InitFloatMsgForPower()
    {
        m_goFloatMsgForPower = GetUIChild("floatMsgForPower").gameObject;
        m_lblFloatMsgForPower = GetUIChild("floatMsgForPowerLbl").GetComponent<UILabel>();
        m_tpFloatMsgForPower = m_goFloatMsgForPower.GetComponent<TweenScale>();
        m_taFloatMsgForPower = m_goFloatMsgForPower.GetComponent<TweenAlpha>();
        m_taFloatMsgForPowerBg = GetUIChild("floatMsgForPowerBg").GetComponent<TweenAlpha>();
        m_tpFloatMsgForPower.enabled = false;
        m_taFloatMsgForPower.enabled = false;
        m_taFloatMsgForPowerBg.enabled = false;

        m_goFloatMsgForPower.SetActive(false);
    }

    public void ShowMsgBox(string text, string okText = "OK")
    {
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
        //m_goOKCancelBox.SetBoxText(text);
        //m_goOKCancelBox.SetOKBtnText(okText);

        //m_goOKCancelBox.gameObject.SetActive(true);
        //m_goOKCancelBox.SetCallback(Hide);

        if (okText.Equals("OK"))
            okText = LanguageData.GetContent(25561);

        MogoGlobleUIManager.Instance.Info(text, okText);
    }

    float beginTime = 0;
    uint length = 1000;
    Queue textQueue = new Queue();
    public void ShowFloatingTextQueue(string text)
    {
        if (!MogoWorld.showFloatText)
        {
            return;
        }
        if (transform == null) return;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
        if (textQueue == null) return;
        textQueue.Enqueue(text);


        ////����һ���ı���������Ʈ
        //AssetCacheMgr.GetUIInstance("MsgBoxFloat.prefab", (str, id, obj) =>
        //{
        //    GameObject go = obj as GameObject;
        //    Mogo.Util.Utils.MountToSomeObjWithoutPosChange(go.transform, transform);
        //    FloatMsg floatMsg = go.AddComponent<FloatMsg>();
        //    floatMsg.text = text;
        //    TimerHeap.AddTimer(length+500, 0,
        //        () =>
        //        {
        //            AssetCacheMgr.ReleaseInstance(go);
        //        });
        //});
    }
    public void ShowFloatingText(string text)
    {
        if (!MogoWorld.showFloatText)
        {
            return;
        }
        m_floatMsg.Show(text);
    }

    public void ShowFloatTxtForPower(string txt)
    {
        //m_msgBoxCamera.SetActive(true);
        TimerHeap.DelTimer(m_timerIdfloatMsgForPower);
        m_tpFloatMsgForPower.onFinished = (t) =>
        {
            m_timerIdfloatMsgForPower = TimerHeap.AddTimer(2000, 0, () => 
            {
                m_taFloatMsgForPowerBg.enabled = true;
                m_taFloatMsgForPower.enabled = true;
            });
        };
        m_taFloatMsgForPower.onFinished = (t) =>
        {
            m_goFloatMsgForPower.SetActive(false);
            //m_msgBoxCamera.SetActive(false);
        };

        m_taFloatMsgForPowerBg.Reset();
        m_tpFloatMsgForPower.Reset();
        m_taFloatMsgForPower.Reset();

        m_tpFloatMsgForPower.enabled = true;
        m_taFloatMsgForPower.enabled = false;
        m_taFloatMsgForPowerBg.enabled = false;
        
        m_lblFloatMsgForPower.text = txt;

        m_goFloatMsgForPower.SetActive(true);
    }

    void Update()
    {
        if ((uint)((Time.time - beginTime) * 1000) > length && textQueue.Count > 0)
        {
            beginTime = Time.time;
            int count = 0;
            while (textQueue.Count > 0)
            {
                string text = textQueue.Dequeue() as string;
                //����һ���ı���������Ʈ
                var index = count;
                AssetCacheMgr.GetUIInstance("MsgBoxFloat.prefab", (str, id, obj) =>
                {
                    GameObject go = obj as GameObject;
                    Mogo.Util.Utils.MountToSomeObjWithoutPosChange(go.transform, transform);
                    FloatMsg floatMsg = go.AddComponent<FloatMsg>();
                    floatMsg.m_tweenPosition.from = new Vector3(0, 0 - index * 40, 0);
                    floatMsg.m_tweenPosition.to = new Vector3(0, 180 - index * 40, 0);
                    floatMsg.Show(text);
                    TimerHeap.AddTimer(length + 500, 0,
                        () =>
                        {
                            AssetCacheMgr.ReleaseInstance(go);
                        });
                });
                count++;
            }

        }
    }

    public void Hide()
    {
        m_goOKCancelBox.gameObject.SetActive(false);
    }

    class WaveMsg
    {
        public string m_msg;
        public float m_time;
    }

    private Queue m_waveTextQueue = new Queue();
    private bool m_isShowingWaveText = false;
    private GameObject m_waveMsg;
    private TweenPosition m_waveMsgTweenPostion;
    //private Transform m_waveMsgBeginPosition;
    private UILabel m_waveMsgLbl;
    /// <summary>
    /// ������Ʈ���ı�
    /// </summary>
    /// <param name="msg"></param>
    public void ShowWaveText(string msg, float time = 10f)
    {
        WaveMsg waveMsg = new WaveMsg() { m_msg = msg, m_time = time };
        m_waveTextQueue.Enqueue(waveMsg);

        DoShowWaveText();
        ////����һ���ı�������Ʈ
        //AssetCacheMgr.GetUIInstance("MsgBoxWave.prefab", (str, id, obj) =>
        //{
        //    GameObject go = obj as GameObject;
        //    Mogo.Util.Utils.MountToSomeObjWithoutPosChange(go.transform, transform);
        //    //WaveMsg waveMsg = go.GetComponent<WaveMsg>();
        //    WaveMsg waveMsg = go.AddComponent<WaveMsg>();
        //    waveMsg.Show(msg, time, transform.FindChild("MsgBoxWavePosEnd").position,
        //        transform.FindChild("MsgBoxWavePosFrom").position, () => { AssetCacheMgr.ReleaseInstance(go); });

        //});
    }

    private void DoShowWaveText()
    {
        if (m_isShowingWaveText || m_waveTextQueue.Count <= 0) return;
        m_isShowingWaveText = true;
        if (!m_waveMsg.gameObject.activeSelf) m_waveMsg.SetActive(true);
        WaveMsg waveMsg = m_waveTextQueue.Dequeue() as WaveMsg;
        string msg = waveMsg.m_msg;
        float t = waveMsg.m_time;
        m_waveMsgLbl.text = msg;
        m_waveMsgTweenPostion.enabled = true;
        m_waveMsgTweenPostion.Reset();

        m_waveMsgTweenPostion.from = new Vector3(Mogo.Game.UIConfig.WIDTH / 2, 0, 0);
        m_waveMsgTweenPostion.to = new Vector3(-Mogo.Game.UIConfig.WIDTH / 2 - m_waveMsgLbl.relativeSize.x * m_waveMsgLbl.transform.localScale.x, 0, 0);
        m_waveMsgTweenPostion.duration = t;

        m_waveMsgTweenPostion.onFinished = (uiTween) =>
        {
            m_isShowingWaveText = false;
            if (m_waveTextQueue.Count <= 0)
                m_waveMsg.SetActive(false);
            else
                DoShowWaveText();
        };


    }
}
