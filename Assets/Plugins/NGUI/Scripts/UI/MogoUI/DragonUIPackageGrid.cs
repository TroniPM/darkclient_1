using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DragonUIPackageGrid : MonoBehaviour
{

    public int ID;
    bool m_bStartCount = false;
    float m_fTime = 0;
    const float CLICKELISP = 0.3f;

    Camera m_relatedCam;
    BoxCollider m_relatedCollider;
    RaycastHit m_rayHit;

    bool m_bIsDragging = false;
    bool m_bIsDragBegin = false;
    bool m_bIsDragged = false;

    int m_iDragStartID = -1;

    Transform m_oldClick;

    void Awake()
    {
        m_relatedCam = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
        m_relatedCollider = transform.GetComponentInChildren<BoxCollider>();

        m_rayHit = new RaycastHit();
    }

    void OnDrag(Vector2 v)
    {
        m_bIsDragging = true;
        m_bIsDragged = true;

        //if (m_relatedCollider.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f))
        //{
        //    if (DragonUIViewManager.ButtonTypeToEventUp[transform.name + "Double"] == null)
        //    {
        //        LoggerHelper.Error("No ButtonTypeToEventUp Info");
        //        return;
        //    }

        //    DragonUIViewManager.ButtonTypeToEventUp[transform.name + "Double"](ID);
        //}

        if (!m_bIsDragBegin)
        {
            EventDispatcher.TriggerEvent<int>("DragonUIDragBegin", ID);
            m_bIsDragBegin = true;
        }

    }

    void OnPress(bool isOver)
    {
        if (isOver)
        {

            Physics.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f);
            m_iDragStartID = m_rayHit.transform.GetComponentsInChildren<DragonUIPackageGrid>(true)[0].ID;

            EventDispatcher.TriggerEvent<int>("DragonUIPackageGridDown", ID);
        }
        else
        {
            if (m_bStartCount)
            {
                m_bStartCount = false;

                if (m_fTime <= CLICKELISP)
                {
                    if (m_relatedCollider.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f))
                    {
                        if (DragonUIDict.ButtonTypeToEventUp[transform.name + "Double"] == null)
                        {
                            LoggerHelper.Error("No ButtonTypeToEventUp Info");
                            return;
                        }

                        DragonUIDict.ButtonTypeToEventUp[transform.name + "Double"](ID);
                    }
                }
            }
            else
            {
                m_bStartCount = true;
            }


            m_fTime = 0;
            m_oldClick = transform;

            if (m_bIsDragging)
            {
                m_bIsDragging = false;


                if (Physics.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f) &&
                      (m_rayHit.transform.GetComponentsInChildren<DragonUIPackageGrid>(true).Length > 0))
                {
                    int id = m_rayHit.transform.GetComponentsInChildren<DragonUIPackageGrid>(true)[0].ID;

                    EventDispatcher.TriggerEvent<int, int>("DragonUIPackageGridDrag", id, m_iDragStartID);


                    // m_iDragStartID = id;
                }

                else
                {
                    EventDispatcher.TriggerEvent("DragonUIPackageGridDragOutside");
                }

            }

            m_bIsDragBegin = false;
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

                if(!m_bIsDragged)
                {

                    if (DragonUIDict.ButtonTypeToEventUp[transform.name] == null)
                    {
                        LoggerHelper.Error("No ButtonTypeToEventUp Info");
                        return;
                    }

                    DragonUIDict.ButtonTypeToEventUp[transform.name](ID);
                }

                m_bIsDragged = false;
            }
        }
    }
}
