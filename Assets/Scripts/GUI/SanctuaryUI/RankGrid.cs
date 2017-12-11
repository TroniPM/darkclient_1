using UnityEngine;
using System.Collections;

public class RankGrid : MonoBehaviour
{

    public int Id = -1;

    UILabel m_lblRank;
    UILabel m_lblName;
    UILabel m_lblAchieve;


    string m_strRank;

    public string Rank
    {
        get { return m_strRank; }
        set
        { 
            m_strRank = value;

            if (m_lblRank != null)
            {
                m_lblRank.text = m_strRank;
            }
        }
    }
    string m_strName;

    public string Name
    {
        get { return m_strName; }
        set 
        {
            m_strName = value;

            if (m_lblName != null)
            {
                m_lblName.text = m_strName;
            }
        }
    }
    string m_strAchieve;

    public string Achieve
    {
        get { return m_strAchieve; }
        set
        {
            m_strAchieve = value;

            if (m_lblAchieve != null)
            {
                m_lblAchieve.text = m_strAchieve;
            }
        }
    }

    bool m_bIsHighLight;

    public bool IsHighLight
    {
        get { return m_bIsHighLight; }
        set 
        {
            m_bIsHighLight = value;

            
        }
    }


    void Awake()
    {
        m_lblAchieve = transform.Find("WeekRankDialogBodyGridAchieve").GetComponentsInChildren<UILabel>(true)[0];
        m_lblName = transform.Find("WeekRankDialogBodyGridName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblRank = transform.Find("WeekRankDialogBodyGridRank").GetComponentsInChildren<UILabel>(true)[0];
    }

    void Start()
    {
        m_lblAchieve.text = m_strAchieve;
        m_lblName.text = m_strName;
        m_lblRank.text = m_strRank;

        if (m_lblRank != null)
        {
            if (m_bIsHighLight)
            {
                m_lblRank.color = new Color(1, 1, 1);
                m_lblName.color = new Color(1, 1, 1);
                m_lblAchieve.color = new Color(1, 1, 1);
            }
            else
            {
                m_lblRank.color = new Color(0, 0, 0);
                m_lblName.color = new Color(0, 0, 0);
                m_lblAchieve.color = new Color(0, 0, 0);
            }
        }
    }


}
