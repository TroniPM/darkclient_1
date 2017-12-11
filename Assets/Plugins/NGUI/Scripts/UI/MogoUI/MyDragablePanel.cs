using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyDragablePanel : MonoBehaviour
{

    public List<Transform> transformList;
    public List<GameObject> ListPageDown;

    public bool IsHorizontal = true;
    public bool IsMovePage = true;

    TweenPosition m_tp;
    int m_currentTransform = 0;
    Transform m_myTransform;


    float m_fViewPortWidth;
    float m_fViewPortHeight;

    float m_fPageWidth;
    float m_fPageHeight;

    Vector3 m_vec3OldCamPos;

    public float PanelHeight;

    // ≥ı ºªØ
    void Start()
    {
        m_myTransform = transform;

        m_fViewPortWidth = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>().pixelRect.width;
        m_fViewPortHeight =  GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>().pixelRect.height;

        m_fPageWidth = transformList[0].localPosition.x -
            transformList[1].localPosition.x;
        m_fPageHeight = transformList[1].localPosition.y -
            transformList[0].localPosition.y;

        m_vec3OldCamPos = transformList[0].localPosition;

        if (IsMovePage)
        {
            m_tp = m_myTransform.GetComponentInChildren<TweenPosition>() as TweenPosition;

            m_tp.from = m_myTransform.localPosition;
            m_tp.to = transformList[0].localPosition;
        }
    }

    public int GetCurrentPage()
    {
        return m_currentTransform;
    }

    public void DraggingMove(float length)
    {
        if (IsMovePage)
        {
            if (m_currentTransform == transformList.Count - 1 || m_currentTransform == 0)
            {
                m_tp.enabled = false;

                if (IsHorizontal)
                {
                    m_myTransform.localPosition = m_vec3OldCamPos + new Vector3(length * 0.5f / m_fViewPortWidth * m_fPageWidth, 0, 0);
                }
                else
                {
                    m_myTransform.localPosition = m_vec3OldCamPos + new Vector3(0, length * 0.5f / m_fViewPortHeight * m_fPageHeight, 0);
                }
            }
            else
            {
                m_tp.enabled = false;

                if (IsHorizontal)
                {
                    m_myTransform.localPosition = m_vec3OldCamPos + new Vector3(length / m_fViewPortWidth * m_fPageWidth, 0, 0);
                }
                else
                {
                    m_myTransform.localPosition = m_vec3OldCamPos + new Vector3(0, length / m_fViewPortHeight * m_fPageHeight, 0);
                }
            }
        }
        else
        {
            if (IsHorizontal)
            {
                m_myTransform.localPosition = m_vec3OldCamPos + new Vector3(length / m_fViewPortWidth * m_fPageWidth, 0, 0);
            }
            else
            {

                if (PanelHeight > -m_fPageHeight)
                {
                    m_myTransform.localPosition = m_vec3OldCamPos + new Vector3(0, length / m_fViewPortHeight * m_fPageHeight, 0);

                    if (m_myTransform.localPosition.y <= transformList[0].localPosition.y)
                    {
                        m_myTransform.localPosition = transformList[0].localPosition;
                    }
                    else if (m_myTransform.localPosition.y - PanelHeight > transformList[1].localPosition.y)
                    {
                        m_myTransform.localPosition = transformList[1].localPosition + new Vector3(0, PanelHeight, 0);
                    }
                }               
            }

            //int iconNum = ComposeUIViewManager.Instance.GetIconGridNum();
            //int iconChild = ComposeUIViewManager.Instance.GetIconGridChildNum();

            //LoggerHelper.Debug(iconChild);

            //float maxY = transformList[0].localPosition.y - (0.06f * (iconNum - 1) + (iconChild - 6) * 0.056f);
            //LoggerHelper.Debug(maxY);

           
            //else if (m_myTransform.localPosition.y < MaxY)
            //{
            //    m_myTransform.localPosition = new Vector3(m_myTransform.localPosition.x, MaxY, m_myTransform.localPosition.z);
            //}
            //else if (m_myTransform.localPosition.y < transformList[1].localPosition.y)
            //{
            //    m_myTransform.localPosition = transformList[1].localPosition;
            //}


        }
    }

    public void MovePage(Vector2 v)
    {
        if (IsMovePage)
        {
            if (ListPageDown.Count > 0)
            {
                ListPageDown[m_currentTransform].SetActive(false);
            }

            if (IsHorizontal)
            {
                if (v.x < 0)
                {
                    ++m_currentTransform;
                }
                else if (v.x > 0)
                {
                    --m_currentTransform;
                }
            }
            else
            {
                if (v.y > 0)
                {
                    ++m_currentTransform;
                }
                else if (v.y < 0)
                {
                    --m_currentTransform;
                }
            }

            if (m_currentTransform > transformList.Count - 1)
            {
                m_currentTransform = transformList.Count - 1;
            }
            else if (m_currentTransform < 0)
            {
                m_currentTransform = 0;
            }
            else if (0 <= m_currentTransform && m_currentTransform <= transformList.Count - 1)
            {
                //MoveTo(m_currentTransform);

            }

            MoveTo(m_currentTransform);

            if (ListPageDown.Count > 0)
            {
                ListPageDown[m_currentTransform].SetActive(true);
            }

            m_vec3OldCamPos = transformList[m_currentTransform].localPosition;
        }
        else
        {
            m_vec3OldCamPos = m_myTransform.localPosition;
        }
    }

    public void MoveTo(int pageId)
    {
        m_tp.from = m_myTransform.localPosition;
        m_tp.to = transformList[pageId].localPosition;

        m_tp.Reset();
        m_tp.Play(true);

    }
}
