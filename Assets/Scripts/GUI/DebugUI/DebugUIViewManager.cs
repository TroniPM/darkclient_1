using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class DebugUIViewManager : MonoBehaviour 
{

    //public static Dictionary<string, string> ButtonTypeToEventUp = new Dictionary<string, string>();

    UILabel m_lblPosInfo;
    UILabel m_lblFPS;

    float m_fFPS;
    float m_fDeltaTime;
    float m_fFrameCount;

    void OnBattleMainUIUp()
    {
        MogoUIManager.Instance.ShowCurrentUI(false);
    }

    void OnNormalMainUIUp()
    {
        MogoUIManager.Instance.ShowCurrentUI(true);
    }

    void Awake()
    {
        DebugUIDict.ButtonTypeToEventUp.Clear();
        DebugUIDict.ButtonTypeToEventUp.Add("BattleMainUI", "DebugUI_BattleMainUIUp");
        DebugUIDict.ButtonTypeToEventUp.Add("NormalMainUI", "DebugUI_NormalMainUIUp");

        EventDispatcher.AddEventListener("DebugUI_BattleMainUIUp", OnBattleMainUIUp);
        EventDispatcher.AddEventListener("DebugUI_NormalMainUIUp", OnNormalMainUIUp);

        m_lblPosInfo = transform.Find("DebugUIPos").GetComponentsInChildren<UILabel>(true)[0];
        m_lblFPS = transform.Find("DebugUIFPS").GetComponentsInChildren<UILabel>(true)[0];
    }

    void Update()
    {
        //if (Camera.mainCamera)
        //{
        //    m_lblPosInfo.text = Camera.mainCamera.transform.position.ToString();
        //}

        m_fDeltaTime += Time.deltaTime;
        ++m_fFrameCount;

        if (m_fDeltaTime > 1.0f)
        {

            m_lblFPS.text = string.Concat("FPS:", m_fFrameCount.ToString());
            m_fDeltaTime = 0;
            m_fFrameCount = 0;
        }


    }
}
