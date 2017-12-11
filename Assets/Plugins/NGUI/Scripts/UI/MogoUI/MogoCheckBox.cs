using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoCheckBox : MonoBehaviour
{
    GameObject m_bgDown;
    //	GameObject m_bgUp;
    UILabel m_lblText;
    public bool m_checked = false;
    void Start() 
    {
        var ssList = transform.GetComponentsInChildren<UISprite>(true);
        m_bgDown = ssList[1].gameObject;
        m_lblText = transform.GetComponentInChildren<UILabel>();
    }

    void OnClick()
    {
        if (m_checked)
        {
            m_bgDown.SetActive(false);
            m_checked = false;
        }
        else
        {
            m_bgDown.SetActive(true);
            m_checked = true;
        }
    }

    public void SetButtonText(string text)
    {
        m_lblText.text = text;
    }
};
