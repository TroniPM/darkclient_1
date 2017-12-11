using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public enum AloneBattleBillboardType
{
    Gold = 0,
    Exp = 1
}

public class GetGold : AloneBattleBillboard
{
    public GetGold(int gold)
    {
        //AddBillboardText("[F8B62D]" + "Gold + " + gold.ToString() + "[-]");
        AddBillboardText("[FFD200]" + "Gold + " + gold.ToString() + "[-]");
        SetBillboardLabel(new Color32(68, 41, 0, 255));
        BillboardViewManager.Instance.AloneBattleBillboardBuffer.Add(this);
    }
}

public class GetExp : AloneBattleBillboard
{
    public GetExp(int exp)
    {
        //AddBillboardText("[67E4F4]" + "Exp + " + exp.ToString() + "[-]");
        AddBillboardText("[60FE00]" + "Exp + " + exp.ToString() + "[-]");
        SetBillboardLabel(new Color32(13, 52 , 0, 255));
        BillboardViewManager.Instance.AloneBattleBillboardBuffer.Add(this);
    }
}


public class GetItem : AloneBattleBillboard
{
    public GetItem(string str)
    {
        AddBillboardText(str);


        BillboardViewManager.Instance.AloneBattleBillboardBuffer.Add(this);

    }
}

public class GetAloneBillboard : AloneBattleBillboard
{
    public GetAloneBillboard(string text)
    {
        AddBillboardText(text);

        BillboardViewManager.Instance.AloneBattleBillboardBuffer.Add(this);
    }
}

public class AloneBattleBillboard 
{
    protected GameObject m_goBillboard;
    protected UIFont m_font;
    string m_text;
    bool m_bIsLoaded = false;
    Vector3 m_vec3Pos = new Vector3(10000, 10000, 10000);

    TweenPosition m_tp;

    public AloneBattleBillboard()
    {

        m_font = BillboardViewManager.Instance.GetBattleBillboardFont();  

        //AssetCacheMgr.GetUIInstance("AloneBattleBillboard.prefab", (prefab, guid, gameObject) =>
        //{
        //    m_goBillboard = (GameObject)gameObject;
        //    m_goBillboard.SetActive(false);
        //    m_goBillboard.transform.parent = GameObject.Find("MogoMainUIPanel").transform.FindChild("BillboardList").transform;
        //    m_goBillboard.AddComponent<BattleBillboardAnim>().IsPoolObject = false;

        //    m_goBillboard.transform.localPosition = Vector3.zero;
        //    m_goBillboard.transform.localScale = new Vector3(1, 1, 1);
        //    m_goBillboard.transform.localEulerAngles = Vector3.zero;

        //    TruelyAddBillbarodText();
        //    TruelySetBillboardPos();
        //    m_bIsLoaded = true;
        //});


        int id = BillboardViewManager.Instance.DictPoolTypeToID[MaiFeoMemoryPoolType.PoolType_AloneBattleBillobard];

        m_goBillboard = MaiFeoMemoryPoolManager.GetSingleton().GetPoolByID(id).GetOne();

        //m_goBillboardList = m_goBillboard.transform.FindChild("BillboardList").gameObject;
        m_goBillboard.transform.parent = GameObject.Find("MogoMainUIPanel").transform.Find("BattleBillboardList").transform;
        m_goBillboard.transform.localPosition = new Vector3(0, 0, 0);
        m_goBillboard.transform.localScale = new Vector3(1, 1, 1);
        m_goBillboard.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_bIsLoaded = true;

        m_tp = m_goBillboard.GetComponentsInChildren<TweenPosition>(true)[0];
    }

    public void EnableBillboard()
    {
        if (m_tp.enabled == false)
        {
            m_tp.Reset();
            m_tp.Play(true);
        }
        m_goBillboard.SetActive(true);
    }

    void TruelyAddBillbarodText()
    {
        UILabel lbl = NGUITools.AddWidget<UILabel>(m_goBillboard);
        lbl.name = "Text";
        lbl.font = m_font;
        lbl.text = m_text;
        lbl.transform.localPosition = Vector3.zero;
        lbl.transform.localEulerAngles = Vector3.zero;
        lbl.transform.localScale = new Vector3(m_font.size, m_font.size, 1);

        TweenAlpha ta = lbl.gameObject.AddComponent<TweenAlpha>();
        ta.from = 1;
        ta.to = 0;
        ta.callWhenFinished = "FadeFinished";
        ta.eventReceiver = m_goBillboard;
        ta.delay = 0.1f;
    }

    public void AddBillboardText(string text)
    {
        if (m_bIsLoaded)
        {
            if (m_goBillboard.GetComponentsInChildren<UILabel>(true).Length > 0)
            {
                UILabel lbl = m_goBillboard.GetComponentsInChildren<UILabel>(true)[0];

                lbl.name = "Text";
                lbl.font = m_font;
                lbl.text = text;
                lbl.transform.localPosition = Vector3.zero;
                lbl.transform.localEulerAngles = Vector3.zero;
                lbl.transform.localScale = new Vector3(m_font.size*1.2f, m_font.size*1.2f, 1);

                TweenAlpha ta = m_goBillboard.GetComponentsInChildren<TweenAlpha>(true)[0];
                TweenPosition tp = m_goBillboard.GetComponentsInChildren<TweenPosition>(true)[0];

                ta.Reset();
                tp.Reset();

                ta.from = 1;
                ta.to = 0;
                ta.callWhenFinished = "FadeFinished";
                ta.eventReceiver = m_goBillboard;
                ta.delay = 0.1f;

                
                ta.Play(true);
                tp.Play(true);
            }
            else
            {
                UILabel lbl = NGUITools.AddWidget<UILabel>(m_goBillboard);
                lbl.name = "Text";
                lbl.font = m_font;
                lbl.text = text;
                lbl.transform.localPosition = Vector3.zero;
                lbl.transform.localEulerAngles = Vector3.zero;
                lbl.transform.localScale = new Vector3(m_font.size*1.2f, m_font.size*1.2f, 1);

                TweenAlpha ta = lbl.gameObject.AddComponent<TweenAlpha>();
                ta.from = 1;
                ta.to = 0;
                ta.callWhenFinished = "FadeFinished";
                ta.eventReceiver = m_goBillboard;
                ta.delay = 0.1f;
            }
        }
        else
        {
            m_text = text;
        }
    }

    public void SetBillboardLabel(Color32 effectColor, UILabel.Effect effectStyle = UILabel.Effect.Outline)
    {
        if (m_goBillboard.GetComponentsInChildren<UILabel>(true).Length > 0)
        {
            UILabel lbl = m_goBillboard.GetComponentsInChildren<UILabel>(true)[0];
            lbl.effectStyle = effectStyle;
            lbl.effectColor = effectColor;
        }
    }

    void TruelySetBillboardPos()
    {
        if(m_vec3Pos != new Vector3(10000,10000,10000) )
        {
            m_goBillboard.transform.position = m_vec3Pos;
        }
    }

    public void SetBillboardPos(Vector3 pos)
    {
        if (m_goBillboard != null)
        {
            m_goBillboard.transform.position = pos;
        }
        else
        {
            m_vec3Pos = pos;
        }
    }   
}

