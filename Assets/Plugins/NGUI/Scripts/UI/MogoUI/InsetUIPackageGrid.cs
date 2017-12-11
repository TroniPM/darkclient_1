using UnityEngine;
using System.Collections;
using Mogo.Util;

public class InsetUIPackageGrid : MonoBehaviour {

    public int ID;

    Transform m_myTransform;

    Camera m_relatedCam;
    BoxCollider m_relatedCollider;
    RaycastHit m_rayHit;

    bool m_bIsDragging = false;
    bool m_bIsDragBegin = false;

    int m_iDragStartID = -1;

    void Awake()
    {
        m_relatedCam = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
        m_relatedCollider = transform.GetComponentInChildren<BoxCollider>();

        m_rayHit = new RaycastHit();
    }


    void OnDrag()
    {
        m_bIsDragging = true;

        if (!m_bIsDragBegin)
        {
            EventDispatcher.TriggerEvent<int>("InsetPackageGridDragBegin", ID);
            m_bIsDragBegin = true;
        }
    }

    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            if (!m_bIsDragging)
            {
                InsetUIDict.INSETUIPACKAGEGRIDUP(ID);
            }
            else
            {
                if (Physics.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f))
                {
                    if (m_rayHit.transform.GetComponentsInChildren<InsetUIButton>(true).Length > 0)
                    {
                        int id = m_rayHit.transform.GetComponentsInChildren<InsetUIButton>(true)[0].ID;

                        EventDispatcher.TriggerEvent<int, int>("InsetUIPackageGridDrag", id, m_iDragStartID);
                    }
                }
            }
            m_bIsDragging = false;
            m_bIsDragBegin = false;
        }
        else
        {
            m_iDragStartID = ID;
        }
    }
}
