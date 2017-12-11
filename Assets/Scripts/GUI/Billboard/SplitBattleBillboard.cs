using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public enum SplitBattleBillboardType
{
    CriticalMonster = 0,
    CriticalPlayer = 1,
    BrokenAttack = 2,
    NormalPlayer = 3,
    NormalMonster = 4,
    Miss = 5
}

//public class CriticalMonster : SplitBattleBillboard
//{
//    public CriticalMonster(int blood)
//    {
//        AddBattleBillboadSign("critical_guaiwu");
//        AddBattleBillboardNum(blood.ToString(), "yellow_");
//        ReCalculateBillboardCenter();
//    }
//}

//public class CriticalPlayer : SplitBattleBillboard
//{
//    public CriticalPlayer(int blood)
//    {
//        AddBattleBillboadSign("critical_juese");
//        AddBattleBillboardNum(blood.ToString(), "Crimson_");
//        ReCalculateBillboardCenter();
//    }
//}

//public class BrokenAttack : SplitBattleBillboard
//{
//    public BrokenAttack(int blood)
//    {
//        AddBattleBillboardNum(blood.ToString(), "blue_",2);
//        ReCalculateBillboardCenter();
//    }
//}

public class NormalPlayer : SplitBattleBillboard
{
    public NormalPlayer(int blood)
    {
        AddBattleBillboadSign(null);
        AddBattleBillboardNum(blood.ToString(), "red_", 1.3f);
        PlayAllAnim();
    }
}

public class NormalMonster : SplitBattleBillboard
{
    public NormalMonster(int blood)
    {
        AddBattleBillboadSign(null);
        AddBattleBillboardNum(blood.ToString(), "yellow_", 1.3f);
        PlayAllAnim();
    }
}

public class Miss : SplitBattleBillboard
{
    public Miss(int blood)
    {
        AddBattleBillboadSign("miss");
        AddBattleBillboardNum(null, null);
        PlayAllAnim();
    }
}

public class SplitBattleBillboard
{
    protected GameObject m_goBillboard;
    protected GameObject m_goBillboardList;
    protected UIAtlas m_atlas;
    protected List<UISprite> m_listSprite;
    protected float m_fBillboardLength;

    string m_signImg;
    string m_num;
    float m_scale = 1;
    float m_signScale = 1;
    string m_numHeadImg;
    bool m_bIsLoaded = false;
    Vector3 m_vec3Pos = new Vector3(10000, 10000, 10000);
    int m_iNumCount = 0;

    BattleBillboardAnim m_billboardAnim;

    //void TruelyAddBattleBillboardSing()
    //{
    //    if (m_signImg != null)
    //    {
    //        UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
    //        sp.name = "BattleBillboardSign";
    //        sp.atlas = m_atlas;
    //        sp.spriteName = m_signImg;
    //        sp.transform.parent = m_goBillboardList.transform;
    //        sp.MakePixelPerfect();
    //        sp.pivot = UIWidget.Pivot.Left;
    //        sp.transform.localPosition = new Vector3(m_fBillboardLength, 0, 0);
    //        sp.transform.localEulerAngles = new Vector3(0, 0, 0);
    //        sp.transform.localScale = new Vector3(sp.transform.localScale.x * m_signScale, sp.transform.localScale.y * m_signScale, 1);
    //        m_listSprite.Add(sp);
    //        m_fBillboardLength += sp.cachedTransform.localScale.x;

    //        TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
    //        ta.from = 1;
    //        ta.to = 0;
    //        ta.callWhenFinished = "FadeFinished";
    //        ta.eventReceiver = m_goBillboard;
    //        ta.delay = 0.2f;

    //        TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
    //        ts.from = new Vector3(sp.transform.localScale.x * 0.7f, sp.transform.localScale.y * 0.7f, 1);
    //        ts.to = new Vector3(sp.transform.localScale.x, sp.transform.localScale.y, 1);
    //        ts.duration = 0.5f;

    //        ta.enabled = false;
    //        ts.enabled = false;
    //    }
    //}

    protected void AddBattleBillboadSign(string imgName, float scale = 1)
    {
        if (m_bIsLoaded)
        {
            GetBillboardSpriteList();

            if (imgName != null)
            {

                if (m_spSign == null)
                {

                    UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
                    sp.name = "BattleBillboardSign";
                    sp.atlas = m_atlas;
                    sp.spriteName = imgName;
                    sp.transform.parent = m_goBillboardList.transform;
                    //sp.MakePixelPerfect();
                    sp.transform.localScale = new Vector3(120, 44, 1);
                    sp.pivot = UIWidget.Pivot.Left;
                    sp.transform.localPosition = new Vector3(m_fBillboardLength, 0, 0);
                    sp.transform.localEulerAngles = new Vector3(0, 0, 0);
                    sp.transform.localScale = new Vector3(sp.transform.localScale.x * scale, sp.transform.localScale.y * scale, 1);
                
                    m_fBillboardLength += sp.transform.localScale.x;

                    TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
                    ta.from = 1;
                    ta.to = 0;
                    ta.callWhenFinished = "FadeFinished";
                    ta.eventReceiver = m_goBillboard;
                    ta.duration = 0.3f;

                    TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
                    ts.from = new Vector3(sp.transform.localScale.x , sp.transform.localScale.y, 1);
                    ts.to = new Vector3(sp.transform.localScale.x * 0.5f, sp.transform.localScale.y * 0.5f, 1);
                    ts.duration = 0.3f;

                    ts.enabled = false;
                    ta.enabled = false;

                    sp.gameObject.SetActive(true);
                    m_spSign = sp;

                    ReCalculateBillboardCenter();

                }
                else
                {
                    TweenAlpha ta = m_spSign.gameObject.GetComponentsInChildren<TweenAlpha>(true)[0];
                    ta.Reset();
                    ta.enabled = false;

                    TweenScale ts = m_spSign.gameObject.GetComponentsInChildren<TweenScale>(true)[0];
                    ts.from = new Vector3(m_spSign.transform.localScale.x , m_spSign.transform.localScale.y , 1);
                    ts.Reset();
                    ta.enabled = false;

                    m_spSign.gameObject.SetActive(true);

                    ReCalculateBillboardCenter();
                }
            }
            else
            {
                if (m_spSign != null)
                {
                    m_spSign.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            m_signImg = imgName;
            m_signScale = scale;
        }
    }

    UISprite m_spSign = null;

    public int GetBillboardSpriteList()
    {
        UISprite[] listSp = m_goBillboard.GetComponentsInChildren<UISprite>(true);
        m_listSprite.Clear();

        for (int i = 0; i < listSp.Length; ++i)
        {
            if (listSp[i].name != "BattleBillboardSign")
            {

                m_listSprite.Add(m_goBillboard.GetComponentsInChildren<UISprite>(true)[i]);
            }
            else
            {
                m_spSign = listSp[i];
            }
        }

        return m_listSprite.Count;
    }

    protected void AddBattleBillboardNum(string num, string imgName, float scale = 1)
    {
        if (m_bIsLoaded)
        {
            m_iNumCount = GetBillboardSpriteList();

            if (num != null)
            {
                m_fBillboardLength = 0;

                if (num.Length < m_iNumCount)
                {
                    for (int i = 0; i < m_iNumCount; ++i)
                    {
                        if (i < num.Length)
                        {
                            m_listSprite[i].spriteName = imgName + num[i];
                            m_listSprite[i].gameObject.SetActive(true);
                            //m_listSprite[i].MakePixelPerfect();
                            m_listSprite[i].transform.localScale = new Vector3(32, 40, 1) *scale;
                            m_listSprite[i].transform.localPosition = new Vector3(m_fBillboardLength, 0, 0);
                            m_fBillboardLength += m_listSprite[i].transform.localScale.x;

                            TweenAlpha ta = m_listSprite[i].gameObject.GetComponent<TweenAlpha>();
                            ta.from = 1;
                            ta.to = 0;
                            ta.callWhenFinished = "FadeFinished";
                            ta.eventReceiver = m_goBillboard;
                            ta.duration = 0.3f;
                            ta.enabled = false;

                            TweenScale ts = m_listSprite[i].gameObject.GetComponent<TweenScale>();
                            ts.from = new Vector3(m_listSprite[i].transform.localScale.x , m_listSprite[i].transform.localScale.y , 1);
                            ts.to = new Vector3(m_listSprite[i].transform.localScale.x * 0.5f, m_listSprite[i].transform.localScale.y * 0.5f, 1);
                            ts.duration = 0.3f;
                            ts.enabled = false;

                            m_listSprite[i].gameObject.SetActive(true);
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
                        m_listSprite[i].transform.localPosition = new Vector3(m_fBillboardLength, 0, 0);
                        m_fBillboardLength += m_listSprite[i].cachedTransform.localScale.x;
                    }

                    for (int i = m_iNumCount; i < num.Length; ++i)
                    {
                        UISprite sp = NGUITools.AddWidget<UISprite>(m_goBillboard);
                        sp.name = num[i].ToString();
                        sp.atlas = m_atlas;
                        sp.spriteName = imgName + num[i];
                        sp.transform.parent = m_goBillboardList.transform;
                        //sp.MakePixelPerfect();
                        sp.transform.localScale = new Vector3(32,40, 1);
                        sp.pivot = UIWidget.Pivot.Left;
                        sp.transform.localPosition = new Vector3(m_fBillboardLength, 0, 0);
                        sp.transform.localEulerAngles = new Vector3(0, 0, 0);
                        sp.transform.localScale = new Vector3(sp.transform.localScale.x * scale, sp.transform.localScale.y * scale, 1);
                        m_listSprite.Add(sp);
                        m_fBillboardLength += sp.cachedTransform.localScale.x;

                        TweenAlpha ta = sp.gameObject.AddComponent<TweenAlpha>();
                        ta.from = 1;
                        ta.to = 0;
                        ta.callWhenFinished = "FadeFinished";
                        ta.eventReceiver = m_goBillboard;
                        ta.duration = 0.3f;
                        ta.enabled = false;

                        TweenScale ts = sp.gameObject.AddComponent<TweenScale>();
                        ts.from = new Vector3(sp.transform.localScale.x , sp.transform.localScale.y , 1);
                        ts.to = new Vector3(sp.transform.localScale.x * 0.5f, sp.transform.localScale.y * 0.5f, 1);
                        ts.duration = 0.3f;
                        ts.enabled = false;

                        sp.gameObject.SetActive(true);
                    }
                }
                else
                {
                    for (int i = 0; i < m_iNumCount; ++i)
                    {
                        m_listSprite[i].spriteName = imgName + num[i];
                        m_listSprite[i].gameObject.SetActive(true);
                        m_listSprite[i].transform.localPosition = new Vector3(m_fBillboardLength, 0, 0);
                        m_fBillboardLength += m_listSprite[i].cachedTransform.localScale.x;
                    }
                }

                m_iNumCount = num.Length;
                ReCalculateBillboardCenter();
            }
            else
            {
                for (int i = 0; i < m_listSprite.Count; ++i)
                {
                    m_listSprite[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            m_num = num;
            m_numHeadImg = imgName;
            m_scale = scale;
        }
    }

    public void PlayAnim()
    {
        if (m_spSign != null)
        {
            TweenAlpha ta = m_spSign.GetComponentsInChildren<TweenAlpha>(true)[0];
            //TweenScale ts = m_spSign.GetComponentsInChildren<TweenScale>(true)[0];

            ta.Reset();
            //ts.Reset();

            ta.enabled = true;
            //ts.enabled = true;

            ta.Play(true);
            //ts.Play(true);
        }

        for (int i = 0; i < m_listSprite.Count; ++i)
        {
            TweenAlpha ta = m_listSprite[i].GetComponentsInChildren<TweenAlpha>(true)[0];
            //TweenScale ts = m_listSprite[i].GetComponentsInChildren<TweenScale>(true)[0];

            ta.Reset();
            //ts.Reset();

            ta.enabled = true;
            //ts.enabled = true;

            ta.Play(true);
            //ts.Play(true);
        }

        
    }

    public SplitBattleBillboard()
    {
        m_listSprite = new List<UISprite>();

        m_atlas = BillboardViewManager.Instance.GetBattleBillboardAtlas();

        int id = BillboardViewManager.Instance.DictPoolTypeToID[MaiFeoMemoryPoolType.PoolType_SplitBattleBillboard];

        m_goBillboard = MaiFeoMemoryPoolManager.GetSingleton().GetPoolByID(id).GetOne();

        m_goBillboardList = m_goBillboard.transform.Find("BillboardList").gameObject;
        m_goBillboard.transform.parent = GameObject.Find("MogoMainUIPanel").transform.Find("BattleBillboardList").transform;
        m_goBillboard.transform.localPosition = new Vector3(0, 0, 0);
        m_goBillboard.transform.localScale = new Vector3(1, 1, 1);
        m_goBillboard.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_bIsLoaded = true;

        ReCalculateBillboardCenter();
    }

    public void PlayAllAnim()
    {
        ResetAllAnim();

        if (m_billboardAnim == null)
        {
            m_billboardAnim = m_goBillboard.GetComponentsInChildren<BattleBillboardAnim>(true)[0];
        }

        m_billboardAnim.ResetAnim();

        TimerHeap.AddTimer(300, 0, () =>
        {
            if (m_billboardAnim == null)
            {
                m_billboardAnim = m_goBillboard.GetComponentsInChildren<BattleBillboardAnim>(true)[0];
            }

            m_billboardAnim.PlayAnim();

            PlayAnim();
        });
    }

    public void ResetAllAnim()
    {
        if (m_spSign != null)
        {
            TweenAlpha ta = m_spSign.GetComponentsInChildren<TweenAlpha>(true)[0];

            ta.Reset();
        }

        for (int i = 0; i < m_listSprite.Count; ++i)
        {
            TweenAlpha ta = m_listSprite[i].GetComponentsInChildren<TweenAlpha>(true)[0];

            ta.Reset();
        }
    }

    public void ReCalculateBillboardCenter()
    {
        m_goBillboardList.transform.localPosition = new Vector3(-m_fBillboardLength * 0.5f, 0, 0);

        switch (Random.Range(0, 5))
        {
            case 0:
                m_goBillboardList.transform.localPosition += new Vector3(30, 10, 0);
                break;

            case 1:
                m_goBillboardList.transform.localPosition += new Vector3(22, -28, 0);
                break;

            case 2:
                m_goBillboardList.transform.localPosition += new Vector3(-20, 30, 0);
                break;

            case 3:
                m_goBillboardList.transform.localPosition += new Vector3(-16, -12, 0);
                break;

            case 4:
                m_goBillboardList.transform.localPosition += new Vector3(-32, 18, 0);
                break;
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
