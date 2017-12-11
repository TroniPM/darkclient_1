using UnityEngine;
using System.Collections;
using Mogo.Util;

public class InsetUIButton : MonoBehaviour
{

    public int ID;
    bool m_bStartCount = false;
    float m_fTime = 0;
    const float CLICKELISP = 0.2f;

    Camera m_relatedCam;
    BoxCollider m_relatedCollider;
    RaycastHit m_rayHit;

    Transform m_oldClick;

    public bool NeedDoubleClick = true;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_InsetUI;
        m_relatedCollider = transform.GetComponentInChildren<BoxCollider>();

        m_rayHit = new RaycastHit();
    }

    void Start()
    {
        m_relatedCam = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
    }

    void OnPress(bool isOver)
    {
        if (isOver)
        {
        }
        else
        {
            if (NeedDoubleClick)
            {
                if (m_bStartCount)
                {
                    m_bStartCount = false;

                    if (m_fTime <= CLICKELISP)
                    {
                        if (m_relatedCollider.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f))
                        {
                            if (InsetUIDict.ButtonTypeToEventUp.ContainsKey(transform.name + "Double"))
                            {
                                if (InsetUIDict.ButtonTypeToEventUp[transform.name + "Double"] == null)
                                {
                                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
                                    return;
                                }

                                InsetUIDict.ButtonTypeToEventUp[transform.name + "Double"](ID);
                            }
                        }
                    }
                }
                else
                {
                    m_bStartCount = true;
                }


                m_fTime = 0;
                m_oldClick = transform;

                //Camera camera = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
                //BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

                //RaycastHit hit = new RaycastHit();

                //if (bc.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
                //{
                //    if (InsetUIViewManager.ButtonTypeToEventUp[transform.name] == null)
                //    {
                //        LoggerHelper.Error("No ButtonTypeToEventUp Info");
                //        return;
                //    }

                //    InsetUIViewManager.ButtonTypeToEventUp[transform.name](ID);
                //}
            }
            else
            {
                if (InsetUIDict.ButtonTypeToEventUp[transform.name] == null)
                {
                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
                    return;
                }

                InsetUIDict.ButtonTypeToEventUp[transform.name](ID);
            }
        }

    }

    void Update()
    {
        if (m_bStartCount)
        {
            m_fTime += Time.deltaTime;

            if (m_fTime > CLICKELISP)
            {
                m_bStartCount = false;
                m_fTime = 0;


                //if (m_relatedCollider.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f))
                {
                    if (InsetUIDict.ButtonTypeToEventUp[transform.name] == null)
                    {
                        LoggerHelper.Error("No ButtonTypeToEventUp Info");
                        return;
                    }

                    InsetUIDict.ButtonTypeToEventUp[transform.name](ID);
                }
            }
        }
    }

    public void FakePress(bool isPressed)
    {
        if (InsetUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        InsetUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
