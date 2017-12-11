using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateCharacterDetailUIJobAttr : MonoBehaviour
{
    UILabel m_lblAttrName;
    UISprite[] m_arrSpSignFG = new UISprite[6];

    Transform m_myTransform;

    void Awake()
    {
        m_myTransform = transform;

        m_lblAttrName = m_myTransform.Find("CreateCharacterDetailUIJobAttrText").GetComponentsInChildren<UILabel>(true)[0];

        for (int i = 0; i < 6; ++i)
        {
            m_arrSpSignFG[i] = m_myTransform.Find("CreateCharacterDetailUIJobAttrSignList/CreateCharacterDetailUIJobAttrSign" + i + "FG").GetComponentsInChildren<UISprite>(true)[0];
        }
    }

    string m_strAttrName;

    public string AttrName
    {
        get { return m_strAttrName; }
        set 
        {
            m_strAttrName = value;

            if (m_lblAttrName != null)
            {
                m_lblAttrName.text = m_strAttrName;
            }
        }
    }

    int m_iSignNum;

    public int SignNum
    {
        get { return m_iSignNum; }
        set
        {
            m_iSignNum = value;

            if (m_arrSpSignFG[0] != null)
            {
                for (int i = 0; i < 6; ++i)
                {
                    if (i < m_iSignNum)
                    {
                        m_arrSpSignFG[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        m_arrSpSignFG[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
