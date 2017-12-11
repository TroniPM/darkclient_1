using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MogoObjType
{
    NPC,
    Player,
    Dummy
}

public class MogoObjOpt : MonoBehaviour
{
    SkinnedMeshRenderer m_smr;
    MeshRenderer m_mr;
    Transform m_myTransform;
    List<Animation> m_listAnimation = new List<Animation>();
    List<Animator> m_listAnimator = new List<Animator>();

    public MogoObjType ObjType = MogoObjType.NPC;

    void Awake()
    {
        m_myTransform = transform;

        if (m_myTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length > 0)
        {
            

            m_smr = m_myTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true)[0];

            MogoObjOptWorker worker = m_smr.gameObject.AddComponent<MogoObjOptWorker>();
            worker.BecameInVisibleCB = whenObjBecameInvisible;
            worker.BecameVisibleCB = WhenObjBecameVisible;

            if (m_myTransform.GetComponentsInChildren<Animation>(true).Length > 0)
            {
                for (int i = 0; i < m_myTransform.GetComponentsInChildren<Animation>(true).Length; ++i)
                {
                    m_listAnimation.Add(m_myTransform.GetComponentsInChildren<Animation>(true)[i]);
                }
            }

            if (m_myTransform.GetComponent<Animation>() != null)
            {
                m_listAnimation.Add(m_myTransform.GetComponent<Animation>());
            }

            if (m_myTransform.GetComponentsInChildren<Animator>(true).Length > 0)
            {
                for (int i = 0; i < m_myTransform.GetComponentsInChildren<Animator>(true).Length; ++i)
                {
                    m_listAnimator.Add(m_myTransform.GetComponentsInChildren<Animator>(true)[i]);
                }
            }

            if (m_myTransform.GetComponent<Animator>() != null)
            {
                m_listAnimator.Add(m_myTransform.GetComponent<Animator>());
            }
        }

        //if(m_myTransform
    }

    void WhenObjBecameVisible()
    {

        for (int i = 0; i < m_listAnimation.Count; ++i)
        {
            m_listAnimation[i].enabled = true;
        }

        for (int i = 0; i < m_listAnimator.Count; ++i)
        {
            m_listAnimator[i].enabled = true;
        }

        switch (ObjType)
        {
            case MogoObjType.NPC:

                break;

            case MogoObjType.Player:
                BillboardViewManager.Instance.ShowBillboard(GetComponent<ActorPlayer>().theEntity.ID, true);
                GetComponent<ActorPlayer>().enabled = true;
                break;

            case MogoObjType.Dummy:
                BillboardViewManager.Instance.ShowBillboard(GetComponent<ActorDummy>().theEntity.ID, true);
                GetComponent<ActorDummy>().enabled = true;
                break;
        }

        //gameObject.SetActive(true);

    }

    void whenObjBecameInvisible()
    {
        for (int i = 0; i < m_listAnimation.Count; ++i)
        {
            m_listAnimation[i].enabled = false;
        }

        for (int i = 0; i < m_listAnimator.Count; ++i)
        {
            m_listAnimator[i].enabled = false;
        }

        switch (ObjType)
        {
            case MogoObjType.NPC:

                break;

            case MogoObjType.Player:
                BillboardViewManager.Instance.ShowBillboard(GetComponent<ActorPlayer>().theEntity.ID, false);
                GetComponent<ActorPlayer>().enabled = false;
                break;

            case MogoObjType.Dummy:
                BillboardViewManager.Instance.ShowBillboard(GetComponent<ActorDummy>().theEntity.ID, false);
                GetComponent<ActorDummy>().enabled = false;
                break;
        }
        //gameObject.SetActive(false);

    }
}
