using UnityEngine;
using System.Collections;
using Mogo.Util;

public class BattleBillboardAnim : MonoBehaviour
{
    TweenTransform[] m_tt = new TweenTransform[2];
    TweenScale m_ts;
    public bool IsPoolObject = true;

    Transform from;
    Transform to;

    void Awake()
    {
        from = transform.Find("BillboardList/From");
        to = transform.Find("BillboardList/To");
        //Transform from1 = transform.FindChild("BillboardList/From1");
        //Transform to1 = transform.FindChild("BillboardList/To1");

        if (from == null)
        {
            return;
        }


        //if (Random.Range(0, 2) == 0)
        //{
            //from.localPosition = new Vector3(-from.localPosition.x, from.localPosition.y, from.localPosition.z);
            //to.localPosition = new Vector3(-to.localPosition.x, to.localPosition.y, to.localPosition.z);
            //from1.localPosition = new Vector3(-from1.localPosition.x, from1.localPosition.y, from1.localPosition.z);
            //to1.localPosition = new Vector3(-to1.localPosition.x, to1.localPosition.y, to1.localPosition.z);
        //}

        if (transform.GetComponentsInChildren<TweenTransform>(true).Length > 0)
        {
            m_tt[0] = transform.GetComponentsInChildren<TweenTransform>(true)[0];
            //m_tt[1] = transform.GetComponentsInChildren<TweenTransform>(true)[1];
        }

        m_ts = transform.GetComponentsInChildren<TweenScale>(true)[0];
    }
    void FadeFinished()
    {
        //Destroy(gameObject);
        if (IsPoolObject)
        {
            MaiFeoMemoryPoolManager.GetSingleton().GetPoolByID(BillboardViewManager.Instance.DictPoolTypeToID[MaiFeoMemoryPoolType.PoolType_SplitBattleBillboard]).ReturnOne(gameObject);

            //Mogo.Util.LoggerHelper.Debug("FadeFinished!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetAnim()
    {

        if (m_ts != null)
        {
            m_ts.Reset();
            m_ts.enabled = false;
        }
    }

    public void PlayAnim()
    {

        if (m_tt[0] != null)
        {
            m_tt[0].Reset();
            //m_tt[1].Reset();

            m_tt[0].enabled = true;
            //m_tt[1].enabled = true;

            m_tt[0].Play(true);
            //m_tt[1].Play(true);
        } 
        if (m_ts != null)
        {
            m_ts.Reset();
            m_ts.enabled = true;
            m_ts.Play(true);
        }

    }
}
