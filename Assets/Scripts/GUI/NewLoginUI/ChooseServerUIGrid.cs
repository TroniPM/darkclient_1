using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;

public class ChooseServerUIGrid : MonoBehaviour
{
    public int Id = -3;

    UILabel m_lblServerName;
    string m_strServerStatus = "(new)";

    bool m_bIsDragging = false;

    Transform m_myTransform;

    void Awake()
    {
        m_myTransform = transform;

        m_lblServerName = m_myTransform.Find("ChooseServerUIServerBtnText").GetComponentsInChildren<UILabel>(true)[0];

    }

    void Start()
    {
        m_lblServerName.text = m_strServerName + m_strServerStatus;
    }

    string m_strServerName;

    public string ServerName
    {
        get { return m_strServerName; }
        set
        { 
            m_strServerName = value;

            if (m_lblServerName != null)
            {
                m_lblServerName.text = m_strServerName + m_strServerStatus;
            }
        }
    }

    ServerType m_iServerStatus;   //0---�� 1---�� 2---�� 3---ά��

    public ServerType ServerStatus
    {
        get { return m_iServerStatus; }
        set 
        {
            m_iServerStatus = value;

            if (m_lblServerName != null)
            {
                switch (m_iServerStatus)
                {
                    case ServerType.Close:
                        m_strServerStatus = Mogo.GameData.LanguageData.GetContent(200024);
                        break;

                    case ServerType.Recommend:
                        m_strServerStatus = Mogo.GameData.LanguageData.GetContent(200021);
                        break;

                    case ServerType.Hot:
                        m_strServerStatus = Mogo.GameData.LanguageData.GetContent(200022);
                        break;

                    case ServerType.Maintain:
                        m_strServerStatus = Mogo.GameData.LanguageData.GetContent(200023);
                        break;
                }

                m_lblServerName.text = m_strServerName + m_strServerStatus;
            }
        }
    }


    void OnClick()
    {
        if (!m_bIsDragging)
        {
            EventDispatcher.TriggerEvent<int>(NewLoginUILogicManager.NewLoginUIEvent.CHOOSESERVERGRIDUP, Id);
        }

        m_bIsDragging = false;
    }

    void OnDrag(Vector2 v)
    {
        m_bIsDragging = true;
    }

}
