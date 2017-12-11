using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeCamera : MonoBehaviour 
{
    Transform playerTrans;
    Transform myTrans;
    Ray m_ray;
    List<GameObject> m_listLastHit = new List<GameObject>();

    void Awake()
    {
        playerTrans = MogoWorld.thePlayer.Transform;
        myTrans = transform;
    }

    void Update()
    {
        RaycastHit[] hit = Physics.RaycastAll(new Ray(myTrans.position, playerTrans.position - myTrans.position),
             Vector3.Distance(myTrans.position, playerTrans.position - myTrans.position), 1 << 9);

        if (hit.Length > 0)
        {
            for (int j = 0; j < m_listLastHit.Count; ++j)
            {
                bool IsAlready = false;

                for (int i = 0; i < hit.Length; ++i)
                {
                    if (m_listLastHit[j].GetComponent<Collider>()==hit[i].collider)
                    {
                        IsAlready = true;
                        break;
                    }
                }

                if (!IsAlready)
                {

                    if (m_listLastHit[j].GetComponent<FadeOccluer>() != null)
                    {
                        m_listLastHit[j].GetComponent<FadeOccluer>().StartFadeIn();

                        m_listLastHit.Remove(m_listLastHit[j]);
                    }
                }
            }

            for (int i = 0; i < hit.Length; ++i)
            {
                bool IsAlready = false;

                for (int j = 0; j < m_listLastHit.Count; ++j)
                {
                    if (hit[i].collider.gameObject == m_listLastHit[j])
                    {
                        IsAlready = true;
                        break;
                    }
                }

                if (!IsAlready)
                {
                    FadeOccluer fo;
                    if (hit[i].collider.gameObject.GetComponent<FadeOccluer>() == null)
                    {
                        fo = hit[i].collider.gameObject.AddComponent<FadeOccluer>();
                    }
                    else
                    {
                        fo = hit[i].collider.gameObject.GetComponent<FadeOccluer>();
                    }
                    if (fo != null)
                    {
                        fo.StartFadeOut();
                    }

                    m_listLastHit.Add(hit[i].collider.gameObject);
                }
            }
        }
        else
        {
            if (m_listLastHit.Count > 0)
            {
                for (int i = 0; i < m_listLastHit.Count; ++i)
                {
                    if (m_listLastHit[i].GetComponent<FadeOccluer>() != null)
                    {
                        m_listLastHit[i].GetComponent<FadeOccluer>().StartFadeIn();
                    }
                }

                m_listLastHit.Clear();
            }
        }
      
    }
}
