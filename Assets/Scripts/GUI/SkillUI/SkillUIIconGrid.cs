using UnityEngine;
using System.Collections;
using Mogo.Util;

public class SkillUIIconGrid : MonoBehaviour
{
    public int ID = 0;

    Transform m_myTransform;

    UISprite m_spIconActive;
    UISlicedSprite m_ssIconBGDown;
    UISlicedSprite m_ssIconChooseSign;
    UISprite m_ssIconFG;

    string m_strActiveIconName = "";
    bool m_bIsAcitve;



    void Awake()
    {
        m_myTransform = transform;

       
    }

    void Start()
    {
        m_spIconActive = m_myTransform.Find("SkillIcon" + ID + "Core/" + "SkillIcon" + ID + "Active").GetComponentsInChildren<UISprite>(true)[0];
        m_ssIconBGDown = m_myTransform.Find("SkillIcon" + ID + "Core/" + "SkillIcon" + ID + "BGDown").GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_ssIconChooseSign = m_myTransform.Find("SkillIcon" + ID + "ChooseSign").GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_ssIconFG = m_myTransform.Find("SkillIcon" + ID + "Core/" + "SkillIcon" + ID + "FG").GetComponentsInChildren<UISprite>(true)[0];

        if (m_strActiveIconName != "")
        {
            m_spIconActive.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
            m_spIconActive.spriteName = m_strActiveIconName;

            m_ssIconFG.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
            m_ssIconFG.spriteName = "bb_daojugeguangzhe";


            m_spIconActive.ShowAsWhiteBlack(!m_bIsAcitve);
            m_ssIconFG.ShowAsWhiteBlack(!m_bIsAcitve);
        }
    }


    void OnPress(bool isOver)
    {
        if (!isOver)
        {
            //Camera camera = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
            //BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

            //RaycastHit hit = new RaycastHit();

            //if (bc.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
            //{
                if (SkillUIDict.ButtonTypeToEventUp[transform.name] == null)
                {
                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
                    return;
                }

                SkillUIDict.ButtonTypeToEventUp[transform.name](ID);
            //}
        }
    }

    public void SetIconGridDown(bool isDown)
    {
        if (isDown)
        {
            m_ssIconChooseSign.gameObject.SetActive(true);
            m_ssIconBGDown.gameObject.SetActive(true);
            m_ssIconBGDown.transform.parent.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        }
        else
        {
            m_ssIconChooseSign.gameObject.SetActive(false);
            m_ssIconBGDown.gameObject.SetActive(false);
            m_ssIconBGDown.transform.parent.localScale = new Vector3(1, 1, 1);
        }
    }

    public void SetSkillGridActive(bool isActive)
    {
        //if (isActive)
        //{
        //    //m_spIconActive.gameObject.SetActive(true);
        //    m_spIconActive.ShowAsWhiteBlack(false);
        //}
        //else
        //{
        //    //m_spIconActive.gameObject.SetActive(false);
        //    m_spIconActive.ShowAsWhiteBlack(true);
        //}

        if (m_spIconActive != null)
        {
            m_spIconActive.ShowAsWhiteBlack(!isActive);

        }
        else
        {
            m_bIsAcitve = isActive;
        }
    }

    public void SetSkillGridIcon(string imgName)
    {
        if (m_spIconActive != null)
        {
            m_spIconActive.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
            m_spIconActive.spriteName = imgName;
        }
        else
        {
            m_strActiveIconName = imgName;
        }
    }
}
