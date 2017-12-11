using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class CriticalMonster : SuperBattleBillboard
{
    public CriticalMonster(int blood)
    {
        AddBattleBillboadSign("critical_guaiwu",1.2f);
        AddBattleBillboardNum(blood.ToString(), "blue_",1.2f);
        PlayAllAnim();
    }
}

public class CriticalPlayer : SuperBattleBillboard
{
    public CriticalPlayer(int blood)
    {
        AddBattleBillboadSign("critical_juese",1.2f);
        AddBattleBillboardNum(blood.ToString(), "Crimson_", 1.2f);
        PlayAllAnim();
    }
}

public class BrokenAttack : SuperBattleBillboard
{
    public BrokenAttack(int blood)
    {
        AddBattleBillboadSign(null);
        AddBattleBillboardNum(blood.ToString(), "zi_", 1.8f);
        PlayAllAnim();
    }
}

public class SuperBattleBillboard
{
    protected GameObject m_goBillboard;
    protected GameObject m_goBillboardList;
    protected UIAtlas m_atlas;
    protected List<UISprite> m_listSprite;
    protected float m_fBillboardLength;

    string m_signImg;
    // Ϊȥ��������ʱ�������´���
    //float m_signScale = 1;
    string m_num;
    string m_numHeadImg;
    float m_numScale = 1;
    bool m_bIsLoaded = false;
    Vector3 m_vec3Pos = new Vector3(10000, 10000, 10000);
    int m_iNumCount = 0;
    bool m_bIsNeedSign = false;

    void TruelyAddBattleBillboardSign()
    {
        //if (m_signImg != null)
        //{
        //    UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
        //    sp.name = "BattleBillboardSign";
        //    sp.atlas = m_atlas;
        //    sp.spriteName = m_signImg;
        //    sp.transform.parent = m_goBillboardList.transform;
        //    sp.MakePixelPerfect();
        //    sp.pivot = UIWidget.Pivot.Left;
        //    sp.transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
        //    sp.transform.localEulerAngles = new Vector3(0, 0, 0);
        //    sp.transform.localScale = new Vector3(sp.transform.localScale.x * m_signScale, sp.transform.localScale.y * m_signScale, 1);
        //    m_listSprite.Add(sp);
        //    m_fBillboardLength += sp.transform.localScale.x;

        //    TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
        //    ta.from = 1;
        //    ta.to = 1;
        //    ta.callWhenFinished = "FadeFinished";
        //    ta.eventReceiver = m_goBillboard;
        //    // ta.delay = 0.2f;
        //    ta.duration = 0.5f;

        //    TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
        //    ts.from = new Vector3(sp.transform.localScale.x * 1.5f, sp.transform.localScale.y * 1.5f, 1);
        //    ts.to = new Vector3(sp.transform.localScale.x, sp.transform.localScale.y, 1);
        //    ts.duration = 0.2f;
        //}
    }

    protected void AddBattleBillboadSign(string imgName, float scale = 1)
    {
        m_iNumCount = GetBillboardSpriteList();
        if (imgName != null)
        {
            m_fBillboardLength = 0;
            m_bIsNeedSign = true;
            

            if (m_iSignID == -1)
            {
                UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
                sp.name = "BattleBillboardSign";
                sp.atlas = m_atlas;
                sp.spriteName = imgName;
                sp.transform.parent = m_goBillboardList.transform;
                //sp.MakePixelPerfect();
                sp.transform.localScale = new Vector3(236, 56, 1);
                sp.pivot = UIWidget.Pivot.Left;
                sp.transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
                sp.transform.localEulerAngles = Vector3.zero;
                sp.transform.localScale = new Vector3(sp.transform.localScale.x * scale, sp.transform.localScale.y * scale, 1);
                m_listSprite.Add(sp);
                m_fBillboardLength += sp.transform.localScale.x;

                TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
                ta.from = 1;
                ta.to = 1;
                ta.callWhenFinished = "FadeFinished";
                ta.eventReceiver = m_goBillboard;
                // ta.delay = 0.2f;
                ta.duration = 0.5f;
                ta.enabled = false;

                TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
                ts.from = new Vector3(sp.transform.localScale.x * 1.5f, sp.transform.localScale.y * 1.5f, 1);
                ts.to = new Vector3(sp.transform.localScale.x, sp.transform.localScale.y, 1);
                ts.duration = 0.2f;
                ts.enabled = false;

                ReCalculateBillboardCenter();

                m_iSignID = 0;

                m_listSprite[m_iSignID].gameObject.SetActive(true);

            }
            else
            {
                UISprite sp = m_goBillboard.GetComponentsInChildren<UISprite>(true)[m_iSignID];
                sp.spriteName = imgName;
                //sp.MakePixelPerfect();
                sp.transform.localScale = new Vector3(236, 56, 1);
                sp.transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
                sp.transform.localEulerAngles = Vector3.zero;
                sp.transform.localScale = new Vector3(sp.transform.localScale.x * scale, sp.transform.localScale.y * scale, 1);
                m_fBillboardLength += sp.transform.localScale.x;

                TweenAlpha ta = sp.gameObject.GetComponent<TweenAlpha>();
                ta.from = 1;
                ta.to = 1;
                ta.callWhenFinished = "FadeFinished";
                ta.eventReceiver = m_goBillboard;
                // ta.delay = 0.2f;
                ta.duration = 0.5f;
                ta.enabled = false;

                TweenScale ts = sp.gameObject.GetComponent<TweenScale>();
                ts.from = new Vector3(sp.transform.localScale.x * 1.5f, sp.transform.localScale.y * 1.5f, 1);
                ts.to = new Vector3(sp.transform.localScale.x, sp.transform.localScale.y, 1);
                ts.duration = 0.2f;
                ts.enabled = false;

                ReCalculateBillboardCenter();

                sp.gameObject.SetActive(true);

            }
        }
        else
        {
            //m_signImg = imgName;
            //m_signScale = scale;

            m_bIsNeedSign = false;
            if (m_iSignID == -1)
            {
                m_fBillboardLength = 0;
            }
            else
            {
                m_goBillboard.GetComponentsInChildren<UISprite>(true)[m_iSignID].gameObject.SetActive(false);
                m_fBillboardLength = 0;
            }
        }

    }

    int m_iSignID = -1;

    public int GetBillboardSpriteList()
    {
        if (m_listSprite.Count == 0)
        {
            UISprite[] spList = m_goBillboard.GetComponentsInChildren<UISprite>(true);

            for (int i = 0; i < spList.Length; ++i)
            {
                if (spList[i].name != "BattleBillboardSign")
                {
                    m_listSprite.Add(spList[i]);
                }
                else
                {
                    m_iSignID = i;
                }
            }
        }

        return m_listSprite.Count;
    }

    void TruelyAddBattleBillboardNum()
    {
        //if (m_signImg != null)
        //{
        //    for (int i = 0; i < m_num.Length; ++i)
        //    {
        //        UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
        //        sp.name = m_num[i].ToString();
        //        sp.atlas = m_atlas;
        //        sp.spriteName = m_numHeadImg + m_num[i];
        //        sp.transform.parent = m_goBillboardList.transform;
        //        sp.MakePixelPerfect();
        //        sp.pivot = UIWidget.Pivot.Left;
        //        sp.transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
        //        sp.transform.localEulerAngles = new Vector3(0, 0, 0);
        //        sp.transform.localScale = new Vector3(sp.transform.localScale.x * m_numScale, sp.transform.localScale.y * m_numScale, 1);
        //        m_listSprite.Add(sp);
        //        m_fBillboardLength += sp.transform.localScale.x;

        //        TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
        //        ta.from = 1;
        //        ta.to = 1;
        //        ta.callWhenFinished = "FadeFinished";
        //        ta.eventReceiver = m_goBillboard;
        //        //   ta.delay = 0.2f;          
        //        ta.duration = 0.5f;

        //        TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
        //        ts.from = new Vector3(sp.transform.localScale.x * 1.5f, sp.transform.localScale.y * 1.5f, 1);
        //        ts.to = new Vector3(sp.transform.localScale.x, sp.transform.localScale.y, 1);
        //        ts.duration = 0.2f;
        //    }
        //}
    }

    protected void AddBattleBillboardNum(string num, string imgName, float scale = 1)
    {
        if (m_bIsLoaded)
        {
            //m_iNumCount = GetBillboardSpriteList();
            //LoggerHelper.Debug(m_iNumCount + " !!!!!!!!!!!!!!!!!!!!!!!!");

            if (num.Length < m_iNumCount)
            {

                for (int i = 0; i < m_iNumCount; ++i)
                {
                    if (i < num.Length)
                    {
                        m_listSprite[i].spriteName = imgName + num[i];
                        m_listSprite[i].gameObject.SetActive(true);
                        //m_listSprite[i].MakePixelPerfect();
                        m_listSprite[i].transform.localScale = new Vector3(32, 40, 1);
                        m_listSprite[i].transform.localScale *= scale;
                        m_listSprite[i].transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
                        m_fBillboardLength += m_listSprite[i].transform.localScale.x;

                        TweenAlpha ta = m_listSprite[i].gameObject.GetComponent<TweenAlpha>();
                        ta.from = 1;
                        ta.to = 1;
                        ta.callWhenFinished = "FadeFinished";
                        ta.eventReceiver = m_goBillboard;
                        //   ta.delay = 0.2f;          
                        ta.duration = 0.5f;
                        ta.enabled = false;

                        TweenScale ts = m_listSprite[i].gameObject.GetComponent<TweenScale>();
                        ts.from = new Vector3(m_listSprite[i].transform.localScale.x * 1.5f, m_listSprite[i].transform.localScale.y * 1.5f, 1);
                        ts.to = new Vector3(m_listSprite[i].transform.localScale.x, m_listSprite[i].transform.localScale.y, 1);
                        ts.duration = 0.2f;

                        ts.enabled = false;
                    }
                    else
                    {
                        m_listSprite[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (num.Length > m_iNumCount)
            {
                for (int i = 0; i < m_iNumCount; ++i)
                {
                    m_listSprite[i].spriteName = imgName + num[i];
                    m_listSprite[i].gameObject.SetActive(true);
                    //m_listSprite[i].MakePixelPerfect();
                    m_listSprite[i].transform.localScale = new Vector3(32, 40, 1);
                    m_listSprite[i].transform.localScale *= scale;
                    m_listSprite[i].transform.localPosition = new Vector3(m_fBillboardLength,100, 0);
                    m_fBillboardLength += m_listSprite[i].transform.localScale.x;

                    TweenAlpha ta = m_listSprite[i].gameObject.GetComponent<TweenAlpha>();
                    ta.from = 1;
                    ta.to = 1;
                    ta.callWhenFinished = "FadeFinished";
                    ta.eventReceiver = m_goBillboard;
                    //   ta.delay = 0.2f;          
                    ta.duration = 0.5f;
                    ta.enabled = false;

                    TweenScale ts = m_listSprite[i].gameObject.GetComponent<TweenScale>();
                    ts.from = new Vector3(m_listSprite[i].transform.localScale.x * 1.5f, m_listSprite[i].transform.localScale.y * 1.5f, 1);
                    ts.to = new Vector3(m_listSprite[i].transform.localScale.x, m_listSprite[i].transform.localScale.y, 1);
                    ts.duration = 0.2f;

                    ts.enabled = false;
                }

                for (int i = m_iNumCount; i < num.Length; ++i)
                {
                    UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
                    sp.name = num[i].ToString();
                    sp.atlas = m_atlas;
                    sp.spriteName = imgName + num[i];
                    sp.transform.parent = m_goBillboardList.transform;
                    //sp.MakePixelPerfect();
                    m_listSprite[i].transform.localScale = new Vector3(32, 40, 1);
                    sp.pivot = UIWidget.Pivot.Left;
                    sp.transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
                    sp.transform.localEulerAngles = Vector3.zero;
                    sp.transform.localScale = new Vector3(sp.transform.localScale.x * scale, sp.transform.localScale.y * scale, 1);
                    m_listSprite.Add(sp);
                    m_fBillboardLength += sp.transform.localScale.x;

                    TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
                    ta.from = 1;
                    ta.to = 1;
                    ta.callWhenFinished = "FadeFinished";
                    ta.eventReceiver = m_goBillboard;
                    //   ta.delay = 0.2f;          
                    ta.duration = 0.5f;
                    ta.enabled = false;

                    TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
                    ts.from = new Vector3(sp.transform.localScale.x * 1.5f, sp.transform.localScale.y * 1.5f, 1);
                    ts.to = new Vector3(sp.transform.localScale.x, sp.transform.localScale.y, 1);
                    ts.duration = 0.2f;

                    ts.enabled = false;
                }
            }
            else
            {
                for (int i = 0; i < m_iNumCount; ++i)
                {
                    m_listSprite[i].spriteName = imgName + num[i];
                    m_listSprite[i].gameObject.SetActive(true);

                    //m_listSprite[i].MakePixelPerfect();
                    m_listSprite[i].transform.localScale = new Vector3(32, 40, 1);
                    
                    m_listSprite[i].transform.localScale *= scale;
                    m_listSprite[i].transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
                    m_fBillboardLength += m_listSprite[i].transform.localScale.x;

                    TweenAlpha ta = m_listSprite[i].gameObject.GetComponent<TweenAlpha>();
                    ta.from = 1;
                    ta.to = 1;
                    ta.callWhenFinished = "FadeFinished";
                    ta.eventReceiver = m_goBillboard;
                    //   ta.delay = 0.2f;          
                    ta.duration = 0.5f;
                    ta.enabled = false;

                    TweenScale ts = m_listSprite[i].gameObject.GetComponent<TweenScale>();
                    ts.from = new Vector3(m_listSprite[i].transform.localScale.x * 1.5f, m_listSprite[i].transform.localScale.y * 1.5f, 1);
                    ts.to = new Vector3(m_listSprite[i].transform.localScale.x, m_listSprite[i].transform.localScale.y, 1);
                    ts.duration = 0.2f;

                    ts.enabled = false;
                }
            }

            m_iNumCount = num.Length;
            ReCalculateBillboardCenter();

            //for (int i = 0; i < num.Length; ++i)
            //{
            //    UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
            //    sp.name = num[i].ToString();
            //    sp.atlas = m_atlas;
            //    sp.spriteName = imgName + num[i];
            //    sp.transform.parent = m_goBillboardList.transform;
            //    sp.MakePixelPerfect();
            //    sp.pivot = UIWidget.Pivot.Left;
            //    sp.transform.localPosition = new Vector3(m_fBillboardLength, 100, 0);
            //    sp.transform.localEulerAngles = new Vector3(0, 0, 0);
            //    sp.transform.localScale = new Vector3(sp.transform.localScale.x * scale, sp.transform.localScale.y * scale, 1);
            //    m_listSprite.Add(sp);
            //    m_fBillboardLength += sp.transform.localScale.x;

            //    TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
            //    ta.from = 1;
            //    ta.to = 1;
            //    ta.callWhenFinished = "FadeFinished";
            //    ta.eventReceiver = m_goBillboard;
            //    //   ta.delay = 0.2f;          
            //    ta.duration = 0.5f;

            //    TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
            //    ts.from = new Vector3(sp.transform.localScale.x * 1.5f, sp.transform.localScale.y * 1.5f, 1);
            //    ts.to = new Vector3(sp.transform.localScale.x, sp.transform.localScale.y, 1);
            //    ts.duration = 0.2f;

            //    ReCalculateBillboardCenter();
            //}
        }
        else
        {
            m_num = num;
            m_numHeadImg = imgName;
            m_numScale = scale;
        }
    }

    public void PlayAnim()
    {
        for (int i = 0; i < m_listSprite.Count; ++i)
        {
            TweenAlpha ta = m_listSprite[i].GetComponentsInChildren<TweenAlpha>(true)[0];
            TweenScale ts = m_listSprite[i].GetComponentsInChildren<TweenScale>(true)[0];

            ta.Reset();
            ts.Reset();

            ta.enabled = true;
            ts.enabled = true;

            ta.Play(true);
            ts.Play(true);
        }

        if (m_bIsNeedSign )
        {
            TweenAlpha ta = m_goBillboard.GetComponentsInChildren<UISprite>(true)[m_iSignID].GetComponentsInChildren<TweenAlpha>(true)[0];
            TweenScale ts = m_goBillboard.GetComponentsInChildren<UISprite>(true)[m_iSignID].GetComponentsInChildren<TweenScale>(true)[0]; 
            ta.Reset();
            ts.Reset();

            ta.enabled = true;
            ts.enabled = true;

            ta.Play(true);
            ts.Play(true);
        }
    }

    public SuperBattleBillboard()
    {

        m_atlas = BillboardViewManager.Instance.GetBattleBillboardAtlas();
        m_listSprite = new List<UISprite>();
        //AssetCacheMgr.GetUIInstance("SuperBattleBillboard.prefab", (prefab, guid, gameObject) =>
        //{
        //    m_goBillboard = (GameObject)gameObject;
        //    m_goBillboardList = m_goBillboard.transform.FindChild("BillboardList").gameObject;
        //    m_goBillboard.transform.parent = GameObject.Find("MogoMainUIPanel").transform.FindChild("BillboardList").transform;
        //    m_goBillboard.transform.localPosition = new Vector3(0, 0, 0);
        //    m_goBillboard.transform.localScale = new Vector3(1, 1, 1);
        //    m_goBillboard.transform.localEulerAngles = new Vector3(0, 0, 0);

        //    TruelyAddBattleBillboardNum();
        //    TruelyAddBattleBillboardSign();
        //    TruelySetBillboardPos();

        //    m_bIsLoaded = true;

            
        //    ReCalculateBillboardCenter();
        //});

        int id = BillboardViewManager.Instance.DictPoolTypeToID[MaiFeoMemoryPoolType.PoolType_SuperBattleBillboard];

        m_goBillboard = MaiFeoMemoryPoolManager.GetSingleton().GetPoolByID(id).GetOne();

        m_goBillboardList = m_goBillboard.transform.Find("BillboardList").gameObject;
        m_goBillboard.transform.parent = GameObject.Find("MogoMainUIPanel").transform.Find("BattleBillboardList").transform;
        m_goBillboard.transform.localPosition = Vector3.zero;
        m_goBillboard.transform.localScale = new Vector3(1, 1, 1);
        m_goBillboard.transform.localEulerAngles = Vector3.zero;
        m_bIsLoaded = true;

        ReCalculateBillboardCenter();
    }

    public void PlayAllAnim()
    {

        PlayAnim();
    }


    public void ReCalculateBillboardCenter()
    {
        m_goBillboardList.transform.localPosition = new Vector3(-m_fBillboardLength * 0.5f, 0, 0);
    }

    public void TruelySetBillboardPos()
    {
        if(m_vec3Pos != new Vector3(10000,10000,10000))
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
