using UnityEngine;
using System.Collections;
/// <summary>
/// standard grid script writing
/// by zq
/// </summary>
public class VIPInfoGrid : MonoBehaviour
{

    private Transform m_transform;
    private UILabel m_lblText;
    private UISprite m_iconImg;
    private string m_IconName = "";
    public string IconName
    {
        set
        {
            m_IconName = value;
            if (m_iconImg != null)
            {
                m_iconImg.spriteName = m_IconName; 
            }
        }
    }
    private string m_Desc = "";
    public string Desc
    {
        set
        {
            m_Desc = value;
            if (m_lblText != null)
            {
                m_lblText.text = m_Desc;
            }
        }
    }
    // Use this for initialization
    void Awake()
    {
        m_transform = transform;
        m_lblText = m_transform.Find("Label").GetComponent<UILabel>();
        m_iconImg = m_transform.Find("icoReward").GetComponent<UISprite>();
    }
    void Start()
    {
        m_iconImg.spriteName = m_IconName;
        m_lblText.text = m_Desc;
    }
}
