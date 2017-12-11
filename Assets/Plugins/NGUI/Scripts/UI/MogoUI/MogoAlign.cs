using UnityEngine;
using System.Collections;
using Mogo.Util;
public class MogoAlign : MonoBehaviour
{
    Transform m_transform;
    public bool IsContainer = false;
    public bool IsBackground = false;
    public bool IsAlign = false;
    public bool XCenter = false;
    public int TopMargin = -1;
    public int ButtomMargin = -1;
    public bool YCenter = false;
    public int LeftMargin = -1;
    public int RightMargin = -1;
    public Vector2 bgSize;
    private Vector3 m_mySize;
    // Use this for initialization
    void Awake()
    {
        m_transform = transform;
        if (IsContainer)
        {
            m_transform.localScale.Normalize();
            bgSize = new Vector2(Screen.width, Screen.height);
            foreach (var aligner in m_transform.GetComponentsInChildren<MogoAlign>())
            {
                if (aligner.IsBackground)
                {
                    bgSize = new Vector2(aligner.transform.localScale.x, aligner.transform.localScale.y);
                }
            }
        }
        if (IsAlign)
        {
            bgSize = m_transform.parent.GetComponent<MogoAlign>().bgSize;
            BoxCollider tempBC = m_transform.GetComponent<BoxCollider>();
            if (tempBC != null)
            {
                m_mySize = tempBC.size;
            }
            else
            {
                m_mySize = m_transform.localScale;
            }
            if (XCenter)
            {
                LoggerHelper.Debug(m_transform.name + "111");
                m_transform.localPosition= new Vector3(0, m_transform.localPosition.y, 0);
            }
            else
            {
                if (LeftMargin >= 0)
                {
                    m_transform.localPosition = new Vector3(-bgSize.x / 2 + LeftMargin + m_mySize.x / 2, m_transform.localPosition.y, 0);
                }
                else if (RightMargin >= 0)
                {
                    m_transform.localPosition = new Vector3(bgSize.x / 2 - RightMargin - m_mySize.x / 2, m_transform.localPosition.y, 0);
                }
            }
            if (YCenter)
            {
                m_transform.localPosition = new Vector3(m_transform.localPosition.x, 0, 0);
            }
            else
            {
                if (TopMargin >= 0)
                {
                    m_transform.localPosition = new Vector3(m_transform.localPosition.x, bgSize.y / 2 - TopMargin - m_mySize.y / 2, 0);
                }
                else if (ButtomMargin >= 0)
                {
                    m_transform.localPosition = new Vector3(m_transform.localPosition.x, -bgSize.y / 2 + ButtomMargin + m_mySize.y / 2, 0);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
