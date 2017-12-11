using UnityEngine;
using System.Collections;
using Mogo.Util;

public class AssistantUISkillGrid : MonoBehaviour
{

    public int Id = -1;
    bool m_bIsDragging = false;
    bool m_bIsDragBegin = false;

    Camera m_relatedCam;
    BoxCollider m_relatedCollider;
    RaycastHit m_rayHit;

    UISprite m_spIconFG;
    UISprite m_spIconLock;

    void OnDrag(Vector2 v)
    {
        m_bIsDragging = true;

        //if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
        float angle = Mathf.Abs(Mathf.Acos(Vector2.Dot(v.normalized,new Vector2(1,0)))) / Mathf.PI * 180f;

        if (angle > 5 && angle < 175)
        {
            EventDispatcher.TriggerEvent<int>(Events.AssistantEvent.SkillGridDragBegin, Id);
            m_bIsDragBegin = true;
        }
    }

    void OnPress(bool isOver)
    {
        
            if (isOver)
            {
            }
            else
            {
                if (!m_bIsDragging)
                {
                    EventDispatcher.TriggerEvent("AssistantUISkillGridUp", Id);
                }
                else
                {
                    if (Physics.Raycast(m_relatedCam.ScreenPointToRay(Input.mousePosition), out m_rayHit, 10000.0f) &&
                      (m_rayHit.transform.GetComponentsInChildren<AssistantUIButton>(true).Length > 0))
                    {
                        AssistantUIButton btn = m_rayHit.transform.GetComponentsInChildren<AssistantUIButton>(true)[0];

                        if (btn.ID > -1)
                        {
                            EventDispatcher.TriggerEvent<int, int>(Events.AssistantEvent.SkillGridDragToBodyGrid, Id, btn.ID);
                            LoggerHelper.Debug(Id + " " +  btn.ID);
                        }
                    }

                    else
                    {
                        if (m_bIsDragBegin)
                        {
                            EventDispatcher.TriggerEvent(Events.AssistantEvent.SkillGridDragOutside);
                        }
                    }
                }
                

                m_bIsDragging = false;
                m_bIsDragBegin = false;
            }
        
    }



    public void SetEnable(bool isEnable)
    {
        if (!isEnable)
        {
            m_spIconLock.color = new Color32(128, 128, 128, 255);
        }
        else
        {
            m_spIconLock.color = new Color32(255, 255, 255, 255);
        }
    }

    void Start()
    {
        m_relatedCam = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
        m_relatedCollider = transform.GetComponentInChildren<BoxCollider>();

        m_rayHit = new RaycastHit();
    }

    void Awake()
    {
        m_spIconFG = transform.Find("AssistantUIGridIcon").GetComponentsInChildren<UISprite>(true)[0];
        m_spIconLock = transform.Find("AssistantUIGridUnLock").GetComponentsInChildren<UISprite>(true)[0];

    }

    public void SetGridIconName(string iconName)
    {
        m_spIconFG.spriteName = iconName;
    }

    public void ShowGridIconFG(bool isShow)
    {
        m_spIconFG.gameObject.SetActive(isShow);
    }
}
